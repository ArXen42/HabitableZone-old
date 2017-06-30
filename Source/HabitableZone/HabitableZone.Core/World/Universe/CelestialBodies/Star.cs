using System;
using System.Collections.Generic;
using UnityEngine;

namespace HabitableZone.Core.World.Universe.CelestialBodies
{
	public enum StarType
	{
		//Промежуточные (K, F, B) классы отсутствуют
		Blue, //O-класс, 60 масс солнца, голубая звезда
		White, //A-класс, 3,1 массы солнца, белая звезда
		Yellow, //G-класс, 1 масса солнца, желтый карлик
		Red //M-класс, 0,3 массы солнца, красный карлик
	}

	/// <summary>
	///    Star. Pending celestials hieracity improvements.
	/// </summary>
	public class Star : SpaceObject
	{
		public Star(WorldContext worldContext, StarData data) : base(worldContext, data)
		{
			Type = data.Type;
			Mass = data.Mass;
			Luminosity = data.Luminosity;
			Radius = data.Radius;
			SurfaceTemperature = data.SurfaceTemperature;
			Position = data.Position;
		}

		public override SpaceObjectData GetSerializationData()
		{
			return new StarData(this);
		}

		public override Vector2 Position { get; }

		public IEnumerable<Planet> Planets => Location.SpaceObjectsOfType<Planet>();

		public StarType Type { get; set; }

		public Single Mass { get; set; }

		public Single SurfaceTemperature { get; set; } //Температура поверхности звезды, K

		public Single Radius { get; set; }

		public Single Luminosity { get; set; }
		//Дабы не пересчитывать каждый раз (4п(R^2)*o*(T^4), см. en.wikipedia.org/wiki/Luminosity)
	}

	[Serializable]
	public class StarData : SpaceObjectData
	{
		public StarData() { }

		public StarData(Star star) : base(star)
		{
			Type = star.Type;
			Mass = star.Mass;
			Luminosity = star.Luminosity;
			Radius = star.Radius;
			SurfaceTemperature = star.SurfaceTemperature;
			Position = star.Position;
		}

		public override SpaceObject GetInstanceFromData(WorldContext worldContext)
		{
			return new Star(worldContext, this);
		}

		public Single Luminosity;
		public Single Mass;
		public Vector2 Position;
		public Single Radius;
		public Single SurfaceTemperature;
		public StarType Type;
	}
}