using HabitableZone.Core.ShipLogic;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.LevelInitialization
{
	public class ShipInitializer : MonoBehaviour
	{
		public GameObject InitializeShip(Ship ship)
		{
			var shipMainGO = Instantiate(_shipPrefab);
			shipMainGO.GetComponent<ShipWatcher>().Ship = ship;

			var shipGO = (GameObject) Instantiate(Resources.Load("Spacecrafts/IPDK/IPDK", typeof(GameObject)));
			shipGO.transform.parent = shipMainGO.transform;

			shipMainGO.AddComponent<SpacecraftStructureController>();

			return shipMainGO;
		}

		[SerializeField] private GameObject _shipPrefab;
	}
}