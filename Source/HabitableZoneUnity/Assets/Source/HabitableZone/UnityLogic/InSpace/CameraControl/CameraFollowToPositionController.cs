using System;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.CameraControl
{
	/// <summary>
	///    Контроллирует поведение камеры в состоянии CameraControlState.FollowToPosition.
	/// </summary>
	/// <remarks>
	///    Должен быть выключен в инспекторе, CameraController включит при необходимости.
	/// </remarks>
	public class CameraFollowToPositionController : MonoBehaviour
	{
		public const Single Duration = 1.5f;

		[NonSerialized] public Vector2 TargetPos; //Да, публичное поле. Нет, не совестно.

		private void OnEnable()
		{
			_startTime = Time.time;
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			Single normalizedTimer = (Time.time - _startTime) / Duration;

			if (normalizedTimer <= 1)
				_transform.position = (Vector3) Vector2.Lerp(_transform.position, TargetPos, normalizedTimer) +
											 10 * Vector3.back;
			else
				GetComponent<CameraController>().SetFree(); //Не будем городить события
		}

		private Single _startTime;
		private Transform _transform;
	}
}