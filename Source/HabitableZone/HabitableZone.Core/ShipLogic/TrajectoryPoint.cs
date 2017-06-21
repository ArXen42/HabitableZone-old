using System;
using UnityEngine;

namespace HabitableZone.Core.ShipLogic
{
	/// <summary>
	///    Represents a point of somebody's trajectory.
	/// </summary>
	public class TrajectoryPoint
	{
		public TrajectoryPoint(Vector2 position, Vector2 velocity, Single rotation)
		{
			Position = position;
			Velocity = velocity;
			Rotation = rotation;
		}

		public readonly Vector2 Position;
		public readonly Vector2 Velocity;
		public Single Rotation; //Am I Evil?
	}
}