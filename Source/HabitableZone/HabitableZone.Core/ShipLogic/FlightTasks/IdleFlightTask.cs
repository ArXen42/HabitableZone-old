using System;
using HabitableZone.Core.World;
using UnityEngine;

namespace HabitableZone.Core.ShipLogic.FlightTasks
{
	/// <summary>
	///    Represents flight task in which the ship takes no actions and moves by inertia.
	/// </summary>
	public sealed class IdleFlightTask : FlightTask
	{
		public IdleFlightTask(Ship ship, Vector2 startPosition, Vector2 velocity, Single rotation) : base(ship)
		{
			_startPosition = startPosition;
			_velocity = velocity;
			_rotation = rotation;
		}

		public override FlightTaskData GetSerializationData() => new IdleFlightTaskData();

		public override Vector2 Position => _startPosition + Velocity * WorldContext.WorldCtl.NormalizedTurnElapsedTime * 86400;

		public override Vector2 Velocity => _velocity;

		public override Single Rotation => _rotation;

		public override TrajectoryPoint[] VisibleTrajectoryPoints
		{
			get
			{
				var points = new TrajectoryPoint[VisibleTrajectoryPointsPerTurnCount];
				for (Int32 i = 0; i < points.Length; i++)
				{
					Single factor = (Single) i / (points.Length - 1);
					points[i] = new TrajectoryPoint(_startPosition + Velocity * factor * 86400, _velocity, _rotation);
				}

				return points;
			}
		}

		protected override void OnTurnStopped(WorldCtl sender) => InvokeComplete();

		private readonly Vector2 _startPosition;
		private readonly Single _rotation;
		private readonly Vector2 _velocity;
	}

	public class IdleFlightTaskData : FlightTaskData
	{
		public override FlightTask GetInstanceFromData(Ship ship)
		{
			return new IdleFlightTask(ship, ship.Position, ship.Velocity, ship.Rotation);
		}
	}
}