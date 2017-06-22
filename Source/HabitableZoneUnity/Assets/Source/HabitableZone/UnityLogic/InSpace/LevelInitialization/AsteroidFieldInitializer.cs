using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.LevelInitialization
{
	public class AsteroidFieldInitializer : MonoBehaviour
	{
		public GameObject InitializeAsteroidField(AsteroidField asteroidField)
		{
			var asteroidFieldGO = Instantiate(_asteroidFieldPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			asteroidFieldGO.name = asteroidField.Name;
			asteroidFieldGO.AddComponent<AsteroidFieldWatcher>().AsteroidField = asteroidField;

			return asteroidFieldGO;
		}

		[SerializeField] private GameObject _asteroidFieldPrefab;
	}
}