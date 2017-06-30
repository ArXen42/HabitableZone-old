using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HabitableZone.Common;
using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.World.Society;
using HabitableZone.Core.World.Universe.CelestialBodies;
using UnityEngine;
using Random = System.Random;

namespace HabitableZone.Core.World.Universe
{
	public static class UniverseGeneration
	{
		public static WorldContext GenerateWorld()
		{
			var textBytes = Resources.Load<TextAsset>("Spacecrafts/IPDK/IPDK").bytes;
			using (var playerShipStream = new MemoryStream(textBytes))
			{
				var playerShipData = Serialization.DeserializeDataFromJson<ShipData>(playerShipStream);
				return GenerateWorld(playerShipData);
			}
		}

		public static WorldContext GenerateWorld(ShipData playerShipData)
		{
			const Int32 starSystemsCount = 16;
			var positions = GenerateStarSystemsPositions(starSystemsCount, 10);

			var starSystemsData = new StarSystemsData {StarSystems = new StarSystemData[starSystemsCount]};
			var spaceObjects = new List<SpaceObjectData>();

			for (Int32 i = 0; i < starSystemsCount; i++)
			{
				var starSystem = new StarSystemData
				{
					ID = Guid.NewGuid(),
					UniverseMapPosition = positions[i]
				};

				starSystemsData.StarSystems[i] = starSystem;

				var star = GenerateStar(starSystem);
				spaceObjects.Add(star);
				spaceObjects.AddRange(GeneratePlanetsSystem(star));
			}

			spaceObjects.Add(playerShipData);
			playerShipData.LocationID = starSystemsData.StarSystems.First().ID;

			var captainsData = new CaptainsData
			{
				PlayerData = new CaptainData
				{
					CurrentShipID = playerShipData.ID
				}
			};

			var worldContext = new WorldContext(new WorldContextData
			{
				WorldCtlData = new WorldCtlData {Date = new DateTime(3042, 1, 1)},
				StarSystemsData = starSystemsData,
				SpaceObjectsData = new SpaceObjectsData {SpaceObjects = spaceObjects.ToArray()},
				CaptainsData = captainsData
			});

			return worldContext;
		}

		private static StarData GenerateStar(StarSystemData starSystemData)
		{
			//Пока генерируем только главную последовательность
			Single mass = Mathf.Pow(Random.Range(0.54f, 1.3f), 2f) * Constants.SolMass;
			var type = StarType.Yellow;
			Single surfaceTemp = -1f;
			{
				if (mass >= 0.3 * Constants.SolMass && mass <= 0.8 * Constants.SolMass) //M, красный карлик
				{
					type = StarType.Red;
					surfaceTemp = Random.Range(2000f, 5000f);
				}
				else if (mass > 0.8 * Constants.SolMass && mass <= 1.7 * Constants.SolMass) //Типа солнца
				{
					type = StarType.Yellow;
					surfaceTemp = Random.Range(3500f, 7500f);
				}
				else if (mass > 1.7 * Constants.SolMass && mass <= 18 * Constants.SolMass) //Белая или полуголубая звезда
				{
					type = StarType.White;
					surfaceTemp = Random.Range(6000f, 30000f);
				}
				else if (mass > 18 * Constants.SolMass) //Почти гигант, голубая звезда
				{
					type = StarType.Blue;
					surfaceTemp = Random.Range(10000f, 60000f);
				}
			}

			String name = type + " starData";

			Single luminosity = Mathf.Pow(mass / Constants.SolMass, 3.9f) * Constants.SolLuminosity;

			Single radius = Mathf.Sqrt(luminosity) * Constants.SolRadius * (Constants.SolTemperature * Constants.SolTemperature)
								 / (surfaceTemp * surfaceTemp * Mathf.Sqrt(Constants.SolLuminosity)); //L=(R^2)*(T^4)

			return new StarData
			{
				LocationID = starSystemData.ID,
				Mass = mass,
				Name = name,
				Type = type,
				Radius = radius,
				SurfaceTemperature = surfaceTemp,
				Luminosity = luminosity
			};
		}

		private static List<SpaceObjectData> GeneratePlanetsSystem(StarData starData)
		{
			Single deltaMass = Random.Range(0.004f, 0.04f) * Constants.SolMass;
			Single metallicity = Random.Range(0.5f, 0.9f); //Масса тяжелых элементов (тяжелее гелия) в % от общей массы

			var planets = new List<PlanetData>();
			//Это начальный вариант системы, много планет будет удалено в ходе эволюции
			var asteroidFields = new List<AsteroidFieldData>();

			for (Int32 i = 0; i < MaxPlanetCount && deltaMass > 0; i++)
				planets.Add(GeneratePlanet(starData, metallicity, i));

			for (Int32 iterator = 0; iterator < EvolutionIterationsCount; iterator++) //Эволюция в три приема
			for (Int32 i = 0; i < planets.Count - 1; i++) //Идем парами по орбитам
			{
				if (i < 0) continue;
				//Т.к. мы работаем со списком, то можем удалять элементы, но не забываем уменьшать значение итератора
				var planet1 = planets[i];
				var planet2 = planets[i + 1];

				if (!(Mathf.Abs(planet1.OrbitRadius - planet2.OrbitRadius) < EvolutionRange)) continue;

				Single ratio = planet1.Mass / planet2.Mass;

				if (ratio < 0.1f)
				{
					if (Random.Range(0, 2) == 1)
					{
						planet2.Mass += planet1.Mass * Random.Range(0f, 1f); //Вторая ест первую
						planet2.RingsMass = 0.01f * planet1.Mass; //Будем подбирать
					}
					planets.Remove(planet1);
					i--;
				}

				if (ratio <= 10f && ratio >= 0.1f) //Взаимно поделились на ноль
				{
					if (Random.Range(0, 2) == 1 && planet1.Type != PlanetType.Gas && planet2.Type != PlanetType.Gas)
					{
						var asteroidField =
							new AsteroidFieldData
							{
								LocationID = starData.LocationID,
								ParentStarID = starData.ID,
								Mass = planet1.Mass + planet2.Mass,
								Radius = (planet1.OrbitRadius + planet2.OrbitRadius) / 2,
								Name = "AstFieldFrom(" + planet1.Name + ")(" + planet2.Name + ")"
							};
						asteroidFields.Add(asteroidField);
					}

					planets.Remove(planet1);
					planets.Remove(planet2);
					i -= 2;
				}

				if (ratio > 10)
				{
					if (Random.Range(0, 2) == 1)
					{
						planet1.Mass += planet2.Mass * Random.Range(0f, 1f); //Первая ест вторую
						planet1.RingsMass = 0.01f * planet2.Mass;
					}

					planets.Remove(planet2);
					i--;
				}
			}

			for (Int32 i = 0; i < asteroidFields.Count - 1; i++)
			{
				if (Mathf.Abs(asteroidFields[i].Radius - asteroidFields[i + 1].Radius) > AsteroidBeltMergeRange) continue;

				asteroidFields[i].Radius = (asteroidFields[i].Radius + asteroidFields[i + 1].Radius) / 2;
				asteroidFields[i].Mass += asteroidFields[i + 1].Mass;

				var removingAsteroidField = asteroidFields[i + 1];
				asteroidFields.Remove(removingAsteroidField);
				i--;
			}

			var resultList = new List<SpaceObjectData>();
			resultList.AddRange(planets.ConvertAll(p => (SpaceObjectData) p));
			resultList.AddRange(asteroidFields.ConvertAll(a => (SpaceObjectData) a));

			return resultList;
		}

		private static PlanetData GeneratePlanet(StarData starData, Single metallicity, Int32 num)
		{
			String name = "PLANET" + num;
			Single orbitRadius = Mathf.Sqrt(num + 1) * 50e9f + Random.Range(-3e9f, 3e9f);
			//Корень добавлен для создания эффекта протопланетарного диска с уплотнением в середине
			var orbitDegree = new TimeSpan(Random.Next(30, 320), 0, 0, 0);
			Single rotationSpeed = Random.Range(-0.7f, -0.2f);
			Single albedo = Random.Range(0.05f, 0.7f);
			//Статистика по sol из этой статьи подсказывает: https://ru.wikipedia.org/wiki/%D0%90%D0%BB%D1%8C%D0%B1%D0%B5%D0%B4%D0%BE
			Single temperature = starData.SurfaceTemperature * Mathf.Pow(1.0f - albedo, 0.25f) *
										Mathf.Sqrt(starData.Radius / orbitRadius * 2f);

			PlanetType type;
			Single mass;
			Boolean solid = Random.Range(0f, 1f) < metallicity; //Твердая ли планета
			if (solid)
			{
				//Сгенерировать твердую планету
				mass = Random.Range(0.1f, 10f) * Constants.EarthMass;

				if (temperature > 213.0f && temperature <= 340.0f)
					type = PlanetType.Terra;
				else
					type = PlanetType.Desert;
			}
			else
			{
				//Сгенерировать газ. гигант
				mass = Random.Range(10f, 1000f) * Constants.EarthMass;
				type = PlanetType.Gas;
			}

			var planetData = new PlanetData
			{
				LocationID = starData.LocationID,
				ParentStarID = starData.ID,
				Name = name,
				Mass = mass,
				OrbitRadius = orbitRadius,
				OrbitDegree = orbitDegree,
				RotationSpeed = rotationSpeed,
				Type = type,
				Temperature = temperature
			};

			return planetData;
		}

		private static Vector2[] GenerateStarSystemsPositions(Int32 count, Single multiplier)
		{
			Assert.IsTrue(count > 4);

			var positions = new Vector2[count];

			Int32 horizontalCount = (Int32) Mathf.Sqrt(count);

			for (Int32 i = 0; i < count; i++)
			{
				// ReSharper disable once PossibleLossOfFraction
				positions[i] = new Vector2(i % horizontalCount, i / horizontalCount);

				for (Int32 rndOffsetCount = 2; rndOffsetCount <= 6; rndOffsetCount++)
					positions[i] += new Vector2(Random.Range(-1f, 1f), Random.Range(-0.6f, 0.6f)) / rndOffsetCount;
			}

			Single minX = positions.Select(p => p.x).Min();
			Single minY = positions.Select(p => p.y).Min();

			Single deltaX = minX < 0 ? -minX : 0;
			Single deltaY = minY < 0 ? -minY : 0;

			for (Int32 i = 0; i < positions.Length; i++)
			{
				positions[i] += new Vector2(deltaX, deltaY);
				positions[i] *= multiplier;
			}

			return positions;
		}

		private static readonly Random Random = new Random(DateTime.Now.Millisecond);

		private const Single EvolutionRange = 30e9f; //Расстояние взаимодействия планет
		private const Int32 EvolutionIterationsCount = 3; //Количество проходов эволюции
		private const Single AsteroidBeltMergeRange = 50e9f; //Расстояние слияния астероидных поясов

		private const Int32 MaxPlanetCount = 20; //TODO: отрегулировать
	}
}