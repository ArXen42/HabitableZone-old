using System;
using HabitableZone.Common;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;

namespace HabitableZone.Core.World.Universe.CelestialBodies
{
	public enum PlanetType
	{
		Terra,
		Desert,
		Gas
	}

	public class Planet : SpaceObject
	{
		public Planet(WorldContext worldContext, PlanetData data) : base(worldContext, data)
		{
			Type = data.Type;
			ParentStarID = data.ParentStarID;
			Mass = data.Mass;
			OrbitRadius = data.OrbitRadius;
			RotationSpeed = data.RotationSpeed;
			Temperature = data.Temperature;
			RingsMass = data.RingsMass;
			OrbitDegree = data.OrbitDegree;
		}

		public override SpaceObjectData GetSerializationData()
		{
			return new PlanetData(this);
		}

		public override Vector2 Position => PositionAtDate(WorldContext.WorldCtl.Date);

		public Single Mass { get; set; }

		/// <summary>
		///    Звезда, вокруг которой обращается планета (задачу трех тел пока не решаем).
		/// </summary>
		public Star ParentStar
		{
			get { return (Star) WorldContext.SpaceObjects.ByID(ParentStarID); }
			set { ParentStarID = value.ID; }
		}

		public Guid ParentStarID
		{
			get { return _parentStarID; }
			set
			{
				_parentStarID = value;
				RecalculateOrbitPeriod();
			}
		}

		/// <summary>
		///    Радиус орбиты, м.
		/// </summary>
		public Single OrbitRadius
		{
			get { return _radius; }
			set
			{
				_radius = value;
				RecalculateOrbitPeriod();
			}
		}

		/// <summary>
		///    To be replaced: тип планеты.
		/// </summary>
		public PlanetType Type { get; set; }

		/// <summary>
		///    Средняя температура поверхности (будет улучшено).
		/// </summary>
		public Single Temperature { get; set; }

		/// <summary>
		///    Смещение орбиты относительно нулевого дня.
		/// </summary>
		/// <remarks>
		///    Именно 00.00.00, не WorldCtl.InitialDate.
		/// </remarks>
		public TimeSpan OrbitDegree { get; set; }

		/// <summary>
		///    На данный момент чисто декоративный параметр, отвечающий за скорость вращения вокруг своей оси.
		/// </summary>
		// TODO: заменить на TimeSpan RotationPeriod.
		public Single RotationSpeed { get; set; }

		/// <summary>
		///    (WIP) Масса колец, если присутствуют.
		/// </summary>
		public Single RingsMass { get; set; }

		/// <summary>
		///    Орбитальный период этой планеты.
		/// </summary>
		public TimeSpan OrbitPeriod { get; private set; }

		/// <summary>
		///    Возвращает позицию планеты в указанный момент времени.
		/// </summary>
		public Vector2 PositionAtDate(DateTime date)
		{
			Single angle = (date + OrbitDegree).Ticks % OrbitPeriod.Ticks * 360 / (Single) OrbitPeriod.Ticks;
			return Geometry.VertexOnLine(ParentStar.Position, angle, OrbitRadius);
		}

		/// <summary>
		///    Пересчитывает OrbitPeriod в соответствии со звездой и радиусом орбиты.
		/// </summary>
		private void RecalculateOrbitPeriod()
		{
			OrbitPeriod = new TimeSpan(0, 0, 0,
				(Int32) (2 * Math.PI * Mathf.Sqrt(_radius * _radius * _radius /
															 (Constants.GravitationalConstant * ParentStar.Mass))));
		}

		private Guid _parentStarID;

		private Single _radius;
	}

	[Serializable]
	public class PlanetData : SpaceObjectData
	{
		public PlanetData() { }

		public PlanetData(Planet planet) : base(planet)
		{
			Type = planet.Type;
			ParentStarID = planet.ParentStarID;
			Mass = planet.Mass;
			OrbitRadius = planet.OrbitRadius;
			RotationSpeed = planet.RotationSpeed;
			Temperature = planet.Temperature;
			RingsMass = planet.RingsMass;
			OrbitDegree = planet.OrbitDegree;
		}


		public override SpaceObject GetInstanceFromData(WorldContext worldContext)
		{
			return new Planet(worldContext, this);
		}

		[JsonIgnore]
		public TimeSpan OrbitDegree
		{
			get
			{
				TimeSpan result;
				TimeSpan.TryParse(OrbitDegreeString, out result);
				return result;
			}
			set { OrbitDegreeString = value.ToString(); }
		}

		public Single Mass;
		public String OrbitDegreeString;
		public Single OrbitRadius;
		public Guid ParentStarID;
		public Single RingsMass;
		public Single RotationSpeed;
		public Single Temperature;
		public PlanetType Type;
	}
}