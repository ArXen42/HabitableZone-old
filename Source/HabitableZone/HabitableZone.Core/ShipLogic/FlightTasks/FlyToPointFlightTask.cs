using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using HabitableZone.Core.World;
using UnityEngine;
using HabitableZone.Common;

namespace HabitableZone.Core.ShipLogic.FlightTasks
{
	/// <summary>
	///    Just enter the destination point. FlyToPointFlightTask will do the rest.
	/// </summary>
	/// <remarks>
	///    Can be used both independently and as supporting (inner) FlightTask inside other FlightTask.
	/// </remarks>
	public sealed partial class FlyToPointFlightTask : FlightTask
	{
		/// <summary>
		///    Constructs new FlyToPointFlightTask with given ship and destination point.
		/// </summary>
		public FlyToPointFlightTask(Ship ship, Vector2 targetPosition) : base(ship)
		{
			Assert.IsTrue(ship.Acceleration > 0, "Attempted to set fly to point task with zero acceleration.");

			TargetPosition = targetPosition;
			CalculateTrajectory();

			ship.ElectricitySubsystem.PowerStateChanged += OnAccelerationChanged;
		}

		public override FlightTaskData GetSerializationData()
		{
			return new FlyToPointTaskData(this);
		}

		public override Vector2 Position
		{
			get
			{
				Int32 turnEndPointIndex = _trajectoryPoints.Count > SavedPointsPerTurnCount
					? SavedPointsPerTurnCount - 1
					: _trajectoryPoints.Count - 1;

				return _trajectoryPoints[(Int32) (WorldContext.WorldCtl.NormalizedTurnElapsedTime * turnEndPointIndex)].Position;
			}
		}

		public override Vector2 Velocity
		{
			get
			{
				Int32 turnEndPointIndex = _trajectoryPoints.Count > SavedPointsPerTurnCount
					? SavedPointsPerTurnCount - 1
					: _trajectoryPoints.Count - 1;

				return _trajectoryPoints[(Int32) (WorldContext.WorldCtl.NormalizedTurnElapsedTime * turnEndPointIndex)].Velocity;
			}
		}

		public override Single Rotation
		{
			get
			{
				Int32 turnEndPointIndex = _trajectoryPoints.Count > SavedPointsPerTurnCount
					? SavedPointsPerTurnCount - 1
					: _trajectoryPoints.Count - 1;

				return _trajectoryPoints[(Int32) (WorldContext.WorldCtl.NormalizedTurnElapsedTime * turnEndPointIndex)].Rotation;
			}
		}

		public Vector2 TargetPosition { get; }

		protected override void OnTurnStopped(WorldCtl sender)
		{
			if (_trajectoryPoints.Count <= SavedPointsPerTurnCount)
				InvokeComplete();
			else
			{
				CalculateTrajectory();
				InvokeUpdated();
			}
		}

		protected override void OnInvalidation()
		{
			Ship.ElectricitySubsystem.PowerStateChanged -= OnAccelerationChanged;
		}

		private void OnAccelerationChanged(ElectricitySubsystem sender)
		{
			CalculateTrajectory();
			InvokeUpdated();
		}
	}

	[Serializable]
	public class FlyToPointTaskData : FlightTaskData
	{
		public FlyToPointTaskData() { }

		public FlyToPointTaskData(FlyToPointFlightTask flightTask) : base(flightTask)
		{
			TargetPosition = flightTask.TargetPosition;
		}

		public override FlightTask GetInstanceFromData(Ship ship)
		{
			return new FlyToPointFlightTask(ship, TargetPosition);
		}

		public Vector2 TargetPosition;
	}
}