using HabitableZone.Core.ShipLogic.FlightTasks;
using HabitableZone.Core.World.Universe;
using HabitableZone.UnityLogic.InSpace.GUI.Screens;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.UniverseMap
{
	public class UniverseMapJumpButtonController : MonoBehaviour
	{
		private void OnEnable()
		{
			_buttonComponent = GetComponent<Button>();
			_buttonComponent.onClick.AddListener(OnButtonClick);

			_universeMapStarsController.TargetedStarSystemChanged += OnTargetedStarSystemChanged;
			OnTargetedStarSystemChanged(_universeMapStarsController.TargetedStarSystem); //Инициализация
		}

		private void OnDisable()
		{
			_universeMapStarsController.TargetedStarSystemChanged -= OnTargetedStarSystemChanged;
			_buttonComponent.onClick.RemoveAllListeners();
		}

		private void OnTargetedStarSystemChanged(StarSystem starSystem)
		{
			var player = _sharedGOSpawner.WorldContext.Captains.Player;
			_buttonComponent.interactable = starSystem != null &&
													  player.CurrentShip.Hyperdrive.IsJumpPossible(starSystem);
		}

		private void OnButtonClick()
		{
			var player = _sharedGOSpawner.WorldContext.Captains.Player;

			player.CurrentShip.CurrentFlightTask =
				new HyperjumpFlightTask(player.CurrentShip, _universeMapStarsController.TargetedStarSystem);

			GUIScreensManager.UniverseMapScreen.Active = false;
		}

		private Button _buttonComponent;

		[SerializeField] private SharedGOSpawner _sharedGOSpawner;
		[SerializeField] private UniverseMapStarsController _universeMapStarsController;
	}
}