using System.Collections.Generic;
using HabitableZone.Core.SpacecraftStructure;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts
{
	/// <summary>
	///    Maintains Hardpoint --> GameObject linkage.
	/// </summary>
	public class SpacecraftStructureController : MonoBehaviour
	{
		public GameObject GetHardpointGameObject(Hardpoint hardpoint)
		{
			return _gameObjectsDictionary[hardpoint];
		}

		private void OnEnable()
		{
			var hardpointsParent = new GameObject("Hardpoints").transform;
			hardpointsParent.parent = transform;
			hardpointsParent.localPosition = Vector3.zero;

			_spacecraft = GetComponent<ShipWatcher>().Ship;
			_gameObjectsDictionary = new Dictionary<Hardpoint, GameObject>();

			foreach (var hardpoint in _spacecraft.Hardpoints)
			{
				var hardpointPrefab = Resources.Load("Spacecrafts/IPDK/Hardpoints/" + hardpoint.Name, typeof(GameObject));
				var hardpointGO = (GameObject) Instantiate(hardpointPrefab);
				hardpointGO.transform.parent = hardpointsParent.transform;

				_gameObjectsDictionary.Add(hardpoint, hardpointGO); // Hardpoint - GameObject
				hardpointGO.AddComponent<HardpointWatcher>().Init(hardpoint); // GameObject - Hardpoint
			}
		}

		private Spacecraft _spacecraft;
		private Dictionary<Hardpoint, GameObject> _gameObjectsDictionary;
	}
}