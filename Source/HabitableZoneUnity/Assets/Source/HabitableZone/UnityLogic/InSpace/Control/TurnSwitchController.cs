using System;
using HabitableZone.Core.World;
using HabitableZone.UnityLogic.InSpace.CameraControl;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.Control
{
	/// <summary>
	///    Вызывает проверку таймера в Core.World.WorldCtl и обработку пользовательского вызова хода.
	/// </summary>
	public class TurnSwitchController : MonoBehaviour
	{
		public Boolean AutoStartNextTurnEnabled;
		public Boolean FastTurnSwitchModeEnabled;

		/// <summary>
		///   Если ход неактивен, начинает новый ход,
		///   в противном случае выключает автопродолжение.
		/// </summary>
		public void CallTurnToggle()
		{
			if (_worldContext.WorldCtl.IsTurnActive)
				CallTurnStop();
			else
			{
				CallStartNextTurn();
				Camera.main.GetComponent<CameraController>().SetFollowShip(_worldContext.Captains.Player.CurrentShip);
			}
		}

		/// <summary>
		///   Вызывает начало следующего хода.
		/// </summary>
		public void CallStartNextTurn()
		{
			AutoStartNextTurnEnabled = true;
			_worldContext.WorldCtl.StartTurn();
		}

		/// <summary>
		///    Отключает автоматическое начало следующего хода.
		/// </summary>
		public void CallTurnStop()
		{
			AutoStartNextTurnEnabled = false;
		}

		private void OnEnable()
		{
			_worldContext = _sharedGOSpawner.WorldContext;
		}

		private void Update()
		{
			HandleTurnTimerCheck();
			HandleTurnSwitchMode();
		}

		private void HandleTurnTimerCheck()
		{
			if (_worldContext.WorldCtl.IsTurnActive)
			{
				if (FastTurnSwitchModeEnabled)
					_worldContext.WorldCtl.StopTurn();
				else
					_worldContext.WorldCtl.UpdateAndCheckTimer();
			}
		}

		private void HandleTurnSwitchMode()
		{
			if (AutoStartNextTurnEnabled && !_worldContext.WorldCtl.IsTurnActive)
				CallStartNextTurn();
		}

		[SerializeField] private SharedGOSpawner _sharedGOSpawner;
		private WorldContext _worldContext;
	}
}