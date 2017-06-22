using System;
using System.Collections.Generic;
using HabitableZone.Core.ShipLogic;
using HabitableZone.UnityLogic.InSpace.LevelInitialization;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.CameraControl
{
	/// <summary>
	///    Контроллер состояний камеры.
	/// </summary>
	public class CameraController : MonoBehaviour
	{
		public CameraControlState State
		{
			get { return _state; }
			private set
			{
				if (_state == value) return;

				_exitingStateActions[_state].Invoke();
				_enteringStateActions[value].Invoke();
				_state = value;
			}
		}

		public void SetFree()
		{
			State = CameraControlState.Free;
		}

		public void SetFollowToPosition(Vector2 targetPos)
		{
			_followToPositionController.TargetPos = targetPos;
			State = CameraControlState.FollowToPosition;
		}

		public void SetFollowShip(Ship ship)
		{
			_followShipController.Target = ship;
			State = CameraControlState.FollowShip;
		}

		private void OnEnable()
		{
			_followToPositionController = GetComponent<CameraFollowToPositionController>();
			_followShipController = GetComponent<CameraFollowShipController>();

			_enteringStateActions = new Dictionary<CameraControlState, Action>
			{
				{CameraControlState.Free, () => { }},
				{CameraControlState.FollowToPosition, () => _followToPositionController.enabled = true},
				{CameraControlState.FollowShip, () => _followShipController.enabled = true}
			};
			_exitingStateActions = new Dictionary<CameraControlState, Action>
			{
				{CameraControlState.Free, () => { }},
				{CameraControlState.FollowToPosition, () => _followToPositionController.enabled = false},
				{CameraControlState.FollowShip, () => _followShipController.enabled = false}
			};
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				var worldContext = _starSystemViewController.ObservingStarSystem.WorldContext;

				SetFollowToPosition(_starSystemViewController
					.GetSpaceObjectRepresentation(worldContext.Captains.Player.CurrentShip)
					.transform.position);
			}
		}

		private CameraFollowToPositionController _followToPositionController;
		private CameraFollowShipController _followShipController;
		[SerializeField] private StarSystemViewController _starSystemViewController;

		private Dictionary<CameraControlState, Action> _enteringStateActions, _exitingStateActions;
		private CameraControlState _state;
	}

	public enum CameraControlState
	{
		Free,
		FollowShip,
		FollowToPosition
	}
}