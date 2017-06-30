using HabitableZone.UnityLogic.Shared;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.UniverseMap
{
	public class UniverseMapCurrentPositionVisualizationController : MonoBehaviour
	{
		private void Start()
		{
			var player = _sharedGOSpawner.WorldContext.Captains.Player;
			_shipIcon.anchoredPosition = player.CurrentShip.Location.UniverseMapPosition *
												  GetComponentInParent<UniverseMapStarsController>().StarPositionsMultiplier;
		}

		[SerializeField] private SharedGOSpawner _sharedGOSpawner;

		[SerializeField] private RectTransform _shipIcon;
	}
}