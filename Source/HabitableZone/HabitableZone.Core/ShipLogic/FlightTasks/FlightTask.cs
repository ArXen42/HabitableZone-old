using System;
using HabitableZone.Common;
using HabitableZone.Core.World;
using UnityEngine;

namespace HabitableZone.Core.ShipLogic.FlightTasks
{
	/// <summary>
	///    Base class for the basical flight task that determines how the ship will fly during the turn.
	/// </summary>
	public abstract class FlightTask : IWorldContextProvider
	{
		/// <summary>
		///    Determines how many trajectory points are visible in game.
		/// </summary>
		public static Int32 VisibleTrajectoryPointsPerTurnCount = 24; //TODO: should this be placed in TrajectoryDrawer?

		protected FlightTask(Ship ship)
		{
			Ship = ship;
			WorldContext.WorldCtl.TurnStopped += OnTurnStopped;

			State = FlightTaskState.Active;
			IsInvalidating = false;
		}

		public WorldContext WorldContext => Ship.WorldContext;

		/// <summary>
		///    Occurs when the task is complete.
		/// </summary>
		/// <remarks>
		///    Usually occurs when the turn is stopped. Can't occur twice.
		/// </remarks>
		public event CEventHandler<FlightTask> Complete;

		/// <summary>
		///    Occurs when the task is cancelled.
		/// </summary>
		/// <remarks>
		///    Can't occur twice.
		/// </remarks>
		public event CEventHandler<FlightTask> Cancelled;

		/// <summary>
		///    Occurs when the state of the FlightTask (abstract state, not enum) is changed (for example, after acceleration
		///    changed).
		/// </summary>
		/// <remarks>
		///    Invocation is controlled by inherited flight tasks.
		/// </remarks>
		public event CEventHandler<FlightTask> Updated;

		/// <summary>
		///    Position of ship calculated by the FlightTask.
		/// </summary>
		public abstract Vector2 Position { get; }

		/// <summary>
		///    Velocity of ship calculated by the FlightTask.
		/// </summary>
		public abstract Vector2 Velocity { get; }

		/// <summary>
		///    Rotation of ship calculated by the FlightTask.
		/// </summary>
		public abstract Single Rotation { get; }

		/// <summary>
		///    Trajectory of the ship according to the FlightTask.
		/// </summary>
		public abstract TrajectoryPoint[] VisibleTrajectoryPoints { get; }

		/// <summary>
		///    State of the FlightTask.
		/// </summary>
		public FlightTaskState State { get; private set; }

		public abstract FlightTaskData GetSerializationData();

		/// <summary>
		///    Cancels the FlightTask.
		/// </summary>
		public void Cancel()
		{
			Assert.AreEqual(State, FlightTaskState.Active, "Attempted to cancel inactive FlightTask.");

			State = FlightTaskState.Cancelled;
			Cancelled?.Invoke(this);

			Invalidate();
		}

		/// <summary>
		///    Ship this FlightTask belongs to.
		/// </summary>
		/// <remarks>
		///    Doesn't change Ship directly. Instead, Ship's properties references to corresponding from CurrentFlightTask.
		///    Also, at the TurnStopped, Ship stores values of this properties.
		/// </remarks>
		public readonly Ship Ship;

		protected Boolean IsInvalidating { get; private set; }

		/// <summary>
		///    Invoked by inherited flight tasks when they determine that the FlightTask is complete.
		/// </summary>
		protected void InvokeComplete()
		{
			Assert.AreEqual(State, FlightTaskState.Active, "Attempted to complete inactive FlightTask.");

			State = FlightTaskState.Complete;
			Complete?.Invoke(this);

			Invalidate();
		}

		/// <summary>
		///    Invoked by inherited flight tasks when they need to notify subscribers about an update.
		/// </summary>
		protected void InvokeUpdated()
		{
			Assert.AreEqual(State, FlightTaskState.Active, "Attempted to update inactive FlightTask.");

			Updated?.Invoke(this);
		}

		/// <summary>
		///    Action that is taken at the end of the turn.
		/// </summary>
		protected virtual void OnTurnStopped(WorldCtl sender) { }

		/// <summary>
		///    Invoked when the FlightTask is complete or cancelled.
		/// </summary>
		protected virtual void OnInvalidation() { }

		/// <summary>
		///    Frees managed resources (unsubsrcibes from the TurnStopped etc.)
		/// </summary>
		private void Invalidate()
		{
			IsInvalidating = true;

			Assert.AreNotEqual(State, FlightTaskState.Active,
				"Disposed flight task before completeon or cancellation. Considering it be cancelled.");

			WorldContext.WorldCtl.TurnStopped -= OnTurnStopped;

			OnInvalidation();

			IsInvalidating = false;
		}
	}

	public abstract class FlightTaskData
	{
		protected FlightTaskData() { }

		protected FlightTaskData(FlightTask flightTask) { }

		public abstract FlightTask GetInstanceFromData(Ship ship);
	}

	/// <summary>
	///    The state of the FlightTask.
	/// </summary>
	public enum FlightTaskState
	{
		Active,
		Complete,
		Cancelled
	}
}