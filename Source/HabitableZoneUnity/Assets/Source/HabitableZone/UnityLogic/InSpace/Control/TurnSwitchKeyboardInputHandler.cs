using HabitableZone.UnityLogic.InSpace.GUI.Screens;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.Control
{
	public class TurnSwitchKeyboardInputHandler : MonoBehaviour
	{
		private void Start()
		{
			_turnSwitchController = GetComponentInParent<TurnSwitchController>();

			GUIScreensManager.HUDScreen.Enabled += (sender, e) => enabled = true;
			GUIScreensManager.HUDScreen.Disabled += (sender, e) => enabled = false;
		}

		private void Update()
		{
			if (Input.GetButtonDown("TurnSwitch"))
				_turnSwitchController.CallTurnToggle();
		}

		private TurnSwitchController _turnSwitchController;
	}
}