using System;
using UnityEngine;

namespace HabitableZone.Core.World.Universe.CelestialBodies
{
	/// <summary>
	///    Asteroid field.
	/// </summary>
	public class AsteroidField : SpaceObject
	{
		public AsteroidField(WorldContext worldContext, AsteroidFieldData data) : base(worldContext, data)
		{
			ParentStarID = data.ParentStarID;
			Mass = data.Mass;
			Radius = data.Radius;
		}

		public override SpaceObjectData GetSerializationData()
		{
			return new AsteroidFieldData(this);
		}

		public override Vector2 Position => ParentStar.Position;

		public Single Rotation
		{
			get
			{
				var worldCtl = WorldContext.WorldCtl;
				Single dayOfYear = (worldCtl.Date - new DateTime(0)).Days % OrbitPeriod;
				Single angle = dayOfYear * 360 / OrbitPeriod;

				if (worldCtl.IsTurnActive)
					angle += worldCtl.NormalizedTurnElapsedTime * 360 / OrbitPeriod;

				return angle;
			}
		}

		public Single Mass { get; set; }

		public Guid ParentStarID
		{
			get { return _parentStarID; }
			set
			{
				_parentStarID = value;
				OrbitPeriod =
					(Int32)
					(2 * Math.PI *
					 Mathf.Sqrt(_radius * _radius * _radius / (Constants.GravitationalConstant * ParentStar.Mass)) /
					 86400);
			}
		}

		public Star ParentStar
		{
			get { return (Star) WorldContext.SpaceObjects.ByID(ParentStarID); }
			set { _parentStarID = value.ID; }
		}

		public Single Radius
		{
			get { return _radius; }
			set
			{
				_radius = value;
				OrbitPeriod =
					(Int32)
					(2 * Math.PI *
					 Mathf.Sqrt(_radius * _radius * _radius / (Constants.GravitationalConstant * ParentStar.Mass)) /
					 86400);
			}
		}

		public Int32 OrbitPeriod { get; private set; } //TODO: To TimeSpan

		private Guid _parentStarID;
		private Single _radius;
	}

	[Serializable]
	public class AsteroidFieldData : SpaceObjectData
	{
		public AsteroidFieldData() { }

		public AsteroidFieldData(AsteroidField astaroidField) : base(astaroidField)
		{
			ParentStarID = astaroidField.ParentStarID;
			Mass = astaroidField.Mass;
			Radius = astaroidField.Radius;
		}

		public override SpaceObject GetInstanceFromData(WorldContext worldContext)
		{
			return new AsteroidField(worldContext, this);
		}

		public Single Mass;
		public Guid ParentStarID;
		public Single Radius;
	}
}