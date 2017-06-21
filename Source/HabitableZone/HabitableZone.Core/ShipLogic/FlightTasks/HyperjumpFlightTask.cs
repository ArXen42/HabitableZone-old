using System;
using HabitableZone.Common;
using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe;
using UnityEngine;

namespace HabitableZone.Core.ShipLogic.FlightTasks
{
	/// <summary>
	///    Represents complex hyperjump flight task: from moving to entry point to exiting hyperspace.
	/// </summary>
	public sealed class HyperjumpFlightTask : FlightTask
	{
		public HyperjumpFlightTask(Ship ship, StarSystem targetStarSystem) : base(ship)
		{
			if (ship.Hyperdrive == null)
				throw new InvalidOperationException("Failed to initialize HyperjumpFlightTask: missing hyperdrive.");
			if (!ship.Hyperdrive.IsJumpPossible(targetStarSystem))
				throw new InvalidOperationException("Failed to initialize HyperjumpFlightTask: can't jump from current star system.");

			TargetStarSystem = targetStarSystem;

			if (ship.Location == WorldContext.StarSystems.Void) //We need to take care of ships serialized in hyperspace
			{
				Assert.IsNotNull(ship.Hyperdrive.CurrentHyperjumpInfo);
				SetWaitingHyperjumpExit();
			}
			else if (Ship.Location == TargetStarSystem)
			{
				SetExitingHyperspace();
			}
			else
			{
				SetMovingToEntryPoint();
			}
		}

		public override FlightTaskData GetSerializationData()
		{
			return new HyperjumpFlightTaskData(this);
		}

		public readonly StarSystem TargetStarSystem;

		public override Vector2 Position => InnerFlightTask.Position;

		public override Vector2 Velocity => InnerFlightTask.Velocity;

		public override Single Rotation => InnerFlightTask.Rotation;

		public override TrajectoryPoint[] VisibleTrajectoryPoints => InnerFlightTask.VisibleTrajectoryPoints;

		/// <summary>
		///    Nested FlightTask. Hyperspace jump is practically chain of inner FlightTasks incapsulated in this one.
		/// </summary>
		public FlightTask InnerFlightTask
		{
			get { return _innerFlightTask; }
			private set
			{
				_innerFlightTask = value;
				_innerFlightTask.Cancelled += (sender) =>
				{
					ClearInnerFlightTask();
					if (!IsInvalidating) Cancel();
				};
				_innerFlightTask.Complete += (sender) => ClearInnerFlightTask();
				_innerFlightTask.Updated += (sender) => InvokeUpdated();

				InvokeUpdated();
			}
		}

		protected override void OnInvalidation()
		{
			InnerFlightTask?.Cancel();
		}

		private void SetMovingToEntryPoint()
		{
			Vector2 hyperspaceEntryPoint = Ship.Hyperdrive.GetHyperspaceEntryPoint(TargetStarSystem);
			InnerFlightTask = new FlyToPointFlightTask(Ship, hyperspaceEntryPoint);

			InnerFlightTask.Complete += (sender) => SetEnteringHyperspace();
		}

		private void SetEnteringHyperspace()
		{
			if (!Ship.Hyperdrive.IsJumpPossible(TargetStarSystem))
			{
				Cancel();
				return;
			}

			InnerFlightTask = new EnterHyperspaceFlightTask(Ship, Ship.Hyperdrive.GetHyperspaceEntryPoint(TargetStarSystem));
			InnerFlightTask.Complete += sender =>
			{
				Ship.Hyperdrive.Jump(TargetStarSystem);
				SetWaitingHyperjumpExit();
			};
		}

		private void SetWaitingHyperjumpExit()
		{
			Ship.LocationChanged += OnTargetStarSystemReached;
		}

		private void OnTargetStarSystemReached(SpaceObject sender)
		{
			Ship.LocationChanged -= OnTargetStarSystemReached;

			Assert.IsFalse(Ship.Location == WorldContext.StarSystems.Void);
			SetExitingHyperspace();
		}

		private void SetExitingHyperspace()
		{
			var hyperdrive = Ship.Hyperdrive;
			var hyperspaceExitPoint = hyperdrive.GetHyperspaceExitPoint(hyperdrive.CurrentHyperjumpInfo.TargetSystem);

			InnerFlightTask = new ExitingHyperspaceFlightTask(Ship, hyperspaceExitPoint);
			InnerFlightTask.Complete += sender => InvokeComplete();
		}

		private void ClearInnerFlightTask()
		{
			_innerFlightTask = null;
		}

		private FlightTask _innerFlightTask;
	}

	public class HyperjumpFlightTaskData : FlightTaskData
	{
		public HyperjumpFlightTaskData() { }

		public HyperjumpFlightTaskData(HyperjumpFlightTask flightTask) : base(flightTask)
		{
			TargetStarSystemID = flightTask.TargetStarSystem.ID;
		}

		public override FlightTask GetInstanceFromData(Ship ship)
		{
			return new HyperjumpFlightTask(ship, ship.WorldContext.StarSystems.ByID(TargetStarSystemID));
		}

		public Guid TargetStarSystemID;
	}

	public sealed class EnterHyperspaceFlightTask : FlightTask
	{
		public EnterHyperspaceFlightTask(Ship ship, Vector2 entryPoint) : base(ship)
		{
			_jumpAngle = Geometry.EulerAngleOfVector(entryPoint);
			_entryPoint = entryPoint;
			_firstStagePoint = Geometry.VertexOnLine(entryPoint, _jumpAngle, LowSpeedStageMovingDistance);
			_secondStagePoint = Geometry.VertexOnLine(entryPoint, _jumpAngle, HighSpeedStageMovingDistance);
		}

		public override FlightTaskData GetSerializationData()
		{
			throw new InvalidOperationException("This temporary task should not be serialized.");
		}

		public const Single LowSpeedStageDurationFactor = 0.6f;
		public const Single LowSpeedStageMovingDistance = 4e9f;
		public const Single HighSpeedStageMovingDistance = 500e9f;
		public const Single HighSpeedStageDurationFactor = 1 - LowSpeedStageDurationFactor;

		public override Vector2 Position
		{
			get
			{
				Single turnFactor = WorldContext.WorldCtl.NormalizedTurnElapsedTime;
				if (turnFactor < LowSpeedStageDurationFactor)
					return Vector2.Lerp(_entryPoint, _firstStagePoint, turnFactor / LowSpeedStageDurationFactor);
				return Vector2.Lerp(_firstStagePoint, _secondStagePoint,
					(turnFactor - LowSpeedStageDurationFactor) / HighSpeedStageDurationFactor);
			}
		}

		public override Vector2 Velocity => Vector2.zero;

		public override Single Rotation => _jumpAngle;

		public override TrajectoryPoint[] VisibleTrajectoryPoints => new TrajectoryPoint[0];

		protected override void OnTurnStopped(WorldCtl sender) => InvokeComplete();

		private readonly Single _jumpAngle;
		private readonly Vector2 _entryPoint, _firstStagePoint, _secondStagePoint;
	}

	public sealed class ExitingHyperspaceFlightTask : FlightTask
	{
		public ExitingHyperspaceFlightTask(Ship ship, Vector2 hyperspaceExitPoint) : base(ship)
		{
			_hyperspaceExitPoint = hyperspaceExitPoint;
			_jumpExitAngle = Geometry.EulerAngleOfVector(hyperspaceExitPoint);
			Rotation = _jumpExitAngle + 180;
			_highSpeedStagePoint = Geometry.VertexOnLine(hyperspaceExitPoint, _jumpExitAngle, LowSpeedStageMovingDistance);
		}

		public override FlightTaskData GetSerializationData()
		{
			throw new InvalidOperationException("This temporary task should not be serialized.");
		}

		public const Single HighSpeedStageDurationFactor = 0.4f;
		public const Single HighSpeedStageMovingDistance = 400e9f;
		public const Single LowSpeedStageMovingDistance = 4e9f;
		public const Single LowSpeedStageDurationFactor = 1 - HighSpeedStageDurationFactor;

		public override Vector2 Position
		{
			get
			{
				if (WorldContext.WorldCtl.NormalizedTurnElapsedTime < HighSpeedStageDurationFactor)
				{
					Single factor = WorldContext.WorldCtl.NormalizedTurnElapsedTime / HighSpeedStageDurationFactor;
					return Geometry.VertexOnLine(_highSpeedStagePoint, _jumpExitAngle, (1 - factor) * HighSpeedStageMovingDistance);
				}
				else
				{
					Single factor = (WorldContext.WorldCtl.NormalizedTurnElapsedTime - HighSpeedStageDurationFactor) / LowSpeedStageDurationFactor;
					return Vector2.Lerp(_highSpeedStagePoint, _hyperspaceExitPoint, factor);
				}
			}
		}

		public override Vector2 Velocity => Vector2.zero;

		public override Single Rotation { get; }

		public override TrajectoryPoint[] VisibleTrajectoryPoints => new TrajectoryPoint[0];

		protected override void OnTurnStopped(WorldCtl sender) => InvokeComplete();

		private readonly Vector2 _hyperspaceExitPoint, _highSpeedStagePoint;
		private readonly Single _jumpExitAngle;
	}
}