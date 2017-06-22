using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.LevelInitialization
{
	public class StarInitializer : MonoBehaviour
	{
		public GameObject InitializeStar(Star star)
		{
			GameObject starGO = null;

			switch (star.Type)
			{
				case StarType.Yellow:
					starGO = Instantiate(_yellowStarPrefab, new Vector3(0, 0, 5), Quaternion.identity);
					break;

				case StarType.Red:
					starGO = Instantiate(_redStarPrefab, new Vector3(0, 0, 5), Quaternion.identity);
					break;

				case StarType.Blue:
					starGO = Instantiate(_blueStarPrefab, new Vector3(0, 0, 5), Quaternion.identity);
					break;

				case StarType.White:
					starGO = Instantiate(_whiteStarPrefab, new Vector3(0, 0, 5), Quaternion.identity);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			starGO.name = star.Name;
			starGO.AddComponent<StarWatcher>().Star = star;

			return starGO;
		}

		[SerializeField] private GameObject _blueStarPrefab;
		[SerializeField] private GameObject _redStarPrefab;
		[SerializeField] private GameObject _whiteStarPrefab;
		[SerializeField] private GameObject _yellowStarPrefab;
	}
}