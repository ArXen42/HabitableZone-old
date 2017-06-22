using System;
using HabitableZone.Core.ShipLogic;
using HabitableZone.UnityLogic.InSpace.LevelInitialization;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.CameraControl
{
	public class CameraFollowShipController : MonoBehaviour
	{
		public const Single Duration = 1.5f;

		public Ship Target
		{
			get { return _target; }
			set
			{
				_target = value;
				_shipRepresentationTransform = _starSystemViewController.GetSpaceObjectRepresentation(value).transform;
			}
		}

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
			_startTime = Time.time;
		}

		private void Update()
		{
			Single normalizedTimer = (Time.time - _startTime) / Duration;
			Vector2 shipPosition = _shipRepresentationTransform.position;
			_transform.position = (Vector3) (Vector2.Lerp(_transform.position, shipPosition, normalizedTimer)) +
			                      10 * Vector3.back;
		}

		[SerializeField] private StarSystemViewController _starSystemViewController;

		private Transform _transform;
		private Transform _shipRepresentationTransform;
		private Single _startTime;
		private Ship _target;
	}
}