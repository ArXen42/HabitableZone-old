using System;
using System.Collections.Generic;
using System.Linq;
using HabitableZone.Common;
using UnityEngine;

namespace HabitableZone.Core.ShipLogic.FlightTasks
{
	public sealed partial class FlyToPointFlightTask
	{
		/// <summary>
		///    Trajectory calculation iterations per turn count. More iterations - better precision.
		/// </summary>
		public static readonly Int32 IterationsPerTurnCount = 2048;

		/// <summary>
		///    Determines how much calculated points is being saved.
		/// </summary>
		public static readonly Int32 SavedPointsPerTurnCount = 128;

		/// <summary>
		///    If calculated at some iteraion TrajectoryPoint
		/// </summary>
		public static readonly Single SqrTargetDistanceTolerance = 1e17f;

		private static Single Sqrt(Single num)
		{
			return Mathf.Sqrt(num);
		}

		private static Single Sqrt4(Single num)
		{
			return Mathf.Sqrt(Mathf.Sqrt(num));
		}

		private static Single Sqr(Single num)
		{
			return num * num;
		}

		public override TrajectoryPoint[] VisibleTrajectoryPoints => _visibleTrajectoryPoints;

		private void CalculateTrajectory()
		{
			Vector2 spos = Ship.Position, svel = Ship.Velocity, tgt = TargetPosition;
			Single a = Ship.Acceleration;
			Vector2 pos = spos, vel = svel;
			Single deltaTime = 86400f / IterationsPerTurnCount;

			Int32 savedPointsInVisible = SavedPointsPerTurnCount / VisibleTrajectoryPointsPerTurnCount;
			Int32 iterationsInSavedPoints = IterationsPerTurnCount / SavedPointsPerTurnCount;
			_trajectoryPoints.Clear();

			Int32 pointNum = 0;
			while (true)
			{
				Single sqrt4Denom = Sqrt4(Sqr(tgt.x - pos.x) + Sqr(tgt.y - pos.y));
				Single xNumenator = Sqrt(a) * (tgt.x - pos.x) / sqrt4Denom - vel.x;
				Single yNumenator = Sqrt(a) * (tgt.y - pos.y) / sqrt4Denom - vel.y;
				Single bigDenominator = Sqrt(Sqr(xNumenator) + Sqr(yNumenator));

				if (pointNum % iterationsInSavedPoints == 0)
					_trajectoryPoints.Add(
						new TrajectoryPoint(pos, vel, 0) //Rotations will be calculated separately
					);

				Boolean targetReached = (tgt - pos).sqrMagnitude < SqrTargetDistanceTolerance;
				if (targetReached)
					break;

				Single velx = vel.x + deltaTime * a * xNumenator / bigDenominator;
				Single vely = vel.y + deltaTime * a * yNumenator / bigDenominator;

				Single posx = pos.x + deltaTime * vel.x;
				Single posy = pos.y + deltaTime * vel.y;

				pos = new Vector2(posx, posy);
				vel = new Vector2(velx, vely);

				++pointNum;
			}
			_trajectoryPoints.Add(new TrajectoryPoint(tgt, Vector2.zero, 0));

			CalculateRotations();

			// Creation of visible points array.
			// TODO: get rid of unnecessary allocation
			var visiblePoints = new List<TrajectoryPoint>();
			for (Int32 i = 0; i < _trajectoryPoints.Count - 1; i++)
				if (i % savedPointsInVisible == 0)
					visiblePoints.Add(_trajectoryPoints[i]);

			visiblePoints.Add(_trajectoryPoints.Last());
			_visibleTrajectoryPoints = visiblePoints.ToArray();
		}

		private void CalculateRotations()
		{
			_trajectoryPoints[0].Rotation = Ship.Rotation;

			for (Int32 i = 1; i < _trajectoryPoints.Count; i++)
			{
				var deltaVel = _trajectoryPoints[i].Velocity - _trajectoryPoints[i - 1].Velocity;
				_trajectoryPoints[i].Rotation = Geometry.EulerAngleOfVector(deltaVel);
			}
		}

		private readonly List<TrajectoryPoint> _trajectoryPoints = new List<TrajectoryPoint>();

		private TrajectoryPoint[] _visibleTrajectoryPoints;
	}
}