using System;
using HabitableZone.Common;
using HabitableZone.Core.ShipLogic.FlightTasks;
using HabitableZone.Core.SpacecraftStructure;
using HabitableZone.Core.World;
using UnityEngine;

namespace HabitableZone.Core.ShipLogic
{
	/// <summary>
	///    Rerpresents a spaceship (as for now - manned or unmanned spacecraft with some moving capabilities).
	/// </summary>
	/// <remarks>
	///    The ship is always has an assigned CurrentFlightTask. Sorry, the code might be a bit tricky.
	///    The new FlightTask can be assigned only between the turns (or when the turn is starting).
	/// </remarks>
	public class Ship : Spacecraft
	{
		public Ship(WorldContext worldContext, ShipData data) : base(worldContext, data)
		{
			_position = data.Position;
			_rotation = data.Rotation;
			_velocity = data.Velocity;

			CurrentFlightTask = data.FlightTaskData.GetInstanceFromData(this);
		}

		public override SpaceObjectData GetSerializationData()
		{
			return new ShipData(this);
		}

		public override Vector2 Position => Location == WorldContext.StarSystems.Void
			? Geometry.NaN2
			: (WorldContext.WorldCtl.IsTurnActive ? CurrentFlightTask.Position : _position);

		/// <summary>
		///    Aggregates FlightTask.Complete
		/// </summary>
		public event SEventHandler<Ship, FlightTask> CurrentTaskComplete;

		/// <summary>
		///    Aggregates FlightTask.Cancelled
		/// </summary>
		public event SEventHandler<Ship, FlightTask> CurrentTaskCancelled;

		/// <summary>
		///    Aggregates FlightTask.Updated and changing CurrentFlightTask.
		/// </summary>
		public event SEventHandler<Ship, FlightTask> CurrentTaskUpdatedOrChanged;

		/// <summary>
		///    Flight task that is currently being executed by the ship.
		/// </summary>
		/// <remarks>
		///    Little trick: if the task is complete or cancelled at the end of the turn,
		///    the ClearCurrentFlightTask() will be invoked and _currentFlightTask will become null.
		///    But once the getter will be invoked with _currentFlightTask set to null, the new IdleFlightTask will be
		///    instantiated.
		/// </remarks>
		public FlightTask CurrentFlightTask
		{
			get
			{
				if (_currentFlightTask == null)
					CurrentFlightTask = new IdleFlightTask(this, _position, _velocity, _rotation);

				return _currentFlightTask;
			}
			set
			{
				Assert.IsFalse(WorldContext.WorldCtl.IsTurnActive, "Attempted to assign new flight task during the active turn.");

				ClearCurrentFlightTask();

				_currentFlightTask = value;
				_currentFlightTask.Complete += OnTaskComplete;
				_currentFlightTask.Cancelled += OnTaskCancelled;
				_currentFlightTask.Updated += OnCurrentTaskUpdatedOrChanged;

				OnCurrentTaskUpdatedOrChanged(value);
			}
		}

		public Single Rotation => Location == WorldContext.StarSystems.Void
			? Single.NaN
			: (WorldContext.WorldCtl.IsTurnActive ? CurrentFlightTask.Rotation : _rotation);

		public Vector2 Velocity => Location == WorldContext.StarSystems.Void
			? Geometry.NaN2
			: (WorldContext.WorldCtl.IsTurnActive ? CurrentFlightTask.Velocity : _velocity);

		protected override void OnTurnStarted(WorldCtl sender)
		{
			Assert.IsNotNull(CurrentFlightTask);
			Assert.IsTrue(CurrentFlightTask.State == FlightTaskState.Active);
		}

		protected override void OnTurnStopped(WorldCtl sender)
		{
			if (Location == WorldContext.StarSystems.Void)
			{
				_position = Geometry.NaN2;
				_rotation = Single.NaN;
				_velocity = Geometry.NaN2;
			}
			else
			{
				_position = CurrentFlightTask.Position;
				_rotation = CurrentFlightTask.Rotation;
				_velocity = CurrentFlightTask.Velocity;
			}
		}

		private void OnTaskComplete(FlightTask flightTask)
		{
			Assert.IsTrue(flightTask == _currentFlightTask);
			CurrentTaskComplete?.Invoke(this, flightTask);

			ClearCurrentFlightTask();
		}

		private void OnTaskCancelled(FlightTask flightTask)
		{
			Assert.IsTrue(flightTask == _currentFlightTask);
			CurrentTaskCancelled?.Invoke(this, flightTask);

			ClearCurrentFlightTask();
		}

		private void OnCurrentTaskUpdatedOrChanged(FlightTask flightTask)
		{
			Assert.IsTrue(flightTask == _currentFlightTask);
			CurrentTaskUpdatedOrChanged?.Invoke(this, flightTask);
		}

		private void ClearCurrentFlightTask()
		{
			if (_currentFlightTask == null) return;

			if (_currentFlightTask.State == FlightTaskState.Active)
				_currentFlightTask.Cancel();

			_currentFlightTask = null;
		}

		private FlightTask _currentFlightTask;

		private Vector2 _position;
		private Single _rotation;
		private Vector2 _velocity;
	}

	[Serializable]
	public class ShipData : SpacecraftData
	{
		public ShipData() { }

		public ShipData(Ship ship) : base(ship)
		{
			Position = ship.Position;
			Velocity = ship.Velocity;
			Rotation = ship.Rotation;
			FlightTaskData = ship.CurrentFlightTask.GetSerializationData();
		}

		public override SpaceObject GetInstanceFromData(WorldContext worldContext)
		{
			return new Ship(worldContext, this);
		}

		public FlightTaskData FlightTaskData;
		public Vector2 Position;
		public Single Rotation;
		public Vector2 Velocity;
	}
}