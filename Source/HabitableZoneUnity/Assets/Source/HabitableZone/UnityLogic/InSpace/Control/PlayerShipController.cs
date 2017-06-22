using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.ShipLogic.FlightTasks;
using HabitableZone.Core.World;
using HabitableZone.UnityLogic.InSpace.GUI.Screens;
using HabitableZone.UnityLogic.InSpace.LevelInitialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HabitableZone.UnityLogic.InSpace.Control
{
	/// <summary>
	///    Handles player ship-controlling input.
	///    Also handles turn stop on FlightTask completion and controls StarSystemViewController.ObservingSystem.
	/// </summary>
	/// <remarks>
	///    Not very Single Responsibility.
	/// </remarks>
	public class PlayerShipController : MonoBehaviour
	{
		private void Start()
		{
			GUIScreensManager.HUDScreen.Enabled += (sender, e) => enabled = true;
			GUIScreensManager.HUDScreen.Disabled += (sender, e) => enabled = false;
		}

		private void OnEnable()
		{
			_worldContext = _starSystemViewController.WorldContext;
			var player = _worldContext.Captains.Player;
			player.CurrentShip.CurrentTaskComplete += OnPlayerCurrentTaskCompleteOrCancelled;
			player.CurrentShip.CurrentTaskCancelled += OnPlayerCurrentTaskCompleteOrCancelled;
			player.CurrentShip.LocationChanged += OnPlayerLocationChanged;
		}

		private void OnDisable()
		{
			var player = _worldContext.Captains.Player;

			player.CurrentShip.CurrentTaskComplete -= OnPlayerCurrentTaskCompleteOrCancelled;
			player.CurrentShip.CurrentTaskCancelled -= OnPlayerCurrentTaskCompleteOrCancelled;
			player.CurrentShip.LocationChanged -= OnPlayerLocationChanged;
		}

		private void Update()
		{
			var worldCtl = _worldContext.WorldCtl;
			if (!worldCtl.IsTurnActive)
				HandleInput();
		}

		private void HandleInput()
		{
			if (!Input.GetButtonDown("MouseClick") || EventSystem.current.IsPointerOverGameObject())
				return;

			var targetPosition = Units.UnityUnitsPositionToMeters(
				Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(9.7f)
			);

			var player = _worldContext.Captains.Player;

			if (player.CurrentShip.Acceleration > 0)
			{
				player.CurrentShip.CurrentFlightTask =
					new FlyToPointFlightTask(player.CurrentShip, targetPosition);
			}
			else
			{
				Debug.LogWarning("No thrust. Enable thrusters in order to have moving abilities.");
			}
		}

		private void OnPlayerCurrentTaskCompleteOrCancelled(Ship ship, FlightTask flightTask)
		{
			_turnSwitchController.CallTurnStop();
		}

		private void OnPlayerLocationChanged(SpaceObject sender)
		{
			var worldContext = _worldContext;
			if (worldContext.Captains.Player.CurrentShip.Location == worldContext.StarSystems.Void)
			{
				_turnSwitchController.FastTurnSwitchModeEnabled = true;
				_turnSwitchController.AutoStartNextTurnEnabled = true;
			}
			else
			{
				_turnSwitchController.FastTurnSwitchModeEnabled = false;
				_starSystemViewController.ReloadScene();
			}
		}

		[SerializeField] private TurnSwitchController _turnSwitchController;
		[SerializeField] private StarSystemViewController _starSystemViewController;

		private WorldContext _worldContext;
	}
}