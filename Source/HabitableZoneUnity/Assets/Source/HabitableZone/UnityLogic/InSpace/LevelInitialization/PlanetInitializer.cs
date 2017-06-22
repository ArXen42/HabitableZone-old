using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using HabitableZone.UnityLogic.PlanetTextureGenerators;
using HabitableZone.UnityLogic.PlanetTextureGenerators.Generators;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.LevelInitialization
{
	public class PlanetInitializer : MonoBehaviour
	{
		public GameObject InitializePlanet(Planet planet)
		{
			PlanetTextureGenerator texGen = null;
			GameObject planetGO = null;

			var planetData = (PlanetData) planet.GetSerializationData();
			switch (planetData.Type)
			{
				case PlanetType.Terra:
					planetGO = Instantiate(_terraPrefab, Vector2.zero, Quaternion.identity);
					texGen = new Terra(PlanetTextureGenerator.DefaultResolution, planetData);
					break;

				case PlanetType.Gas:
					planetGO = Instantiate(_gasPrefab, Vector2.zero, Quaternion.identity);
					texGen = new Gas(PlanetTextureGenerator.DefaultResolution, planetData);
					break;

				case PlanetType.Desert:
					planetGO = Instantiate(_desertPrefab, Vector2.zero, Quaternion.identity);
					texGen = new Desert(PlanetTextureGenerator.DefaultResolution, planetData);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			planetGO.name = planetData.Name;

			var planetTexture = new Texture2D(texGen.XSize, texGen.YSize);
			planetTexture.Resize(texGen.XSize, texGen.YSize, TextureFormat.RGB565, true);
			planetTexture.SetPixels(texGen.TextureColors);
			planetTexture.Apply();

			var meshes = planetGO.GetComponentsInChildren<MeshRenderer>();
			//0 - планета, 1 - облака, 2 - кольца, осторожнее с иерархией в префабе
			meshes[0].material.mainTexture = planetTexture;

			if (planetData.RingsMass > 0)
			{
				var rings = new PlanetRings(PlanetTextureGenerator.DefaultResolution / 2, planetData);
				var ringsTexture = new Texture2D(rings.XSize, rings.YSize);
				ringsTexture.SetPixels(rings.TextureColors);
				ringsTexture.Apply();

				meshes[2].material.mainTexture = ringsTexture;
			}
			else
				meshes[2].gameObject.SetActive(false);

			planetGO.GetComponentInChildren<PlanetRotator>().RotationSpeed = planetData.RotationSpeed;
			planetGO.AddComponent<PlanetWatcher>().Planet = planet;

			return planetGO;
		}

		[SerializeField] private GameObject _desertPrefab;
		[SerializeField] private GameObject _gasPrefab;
		[SerializeField] private GameObject _terraPrefab;
	}
}