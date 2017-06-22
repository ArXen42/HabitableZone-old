using System;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.CameraControl
{
	/// <summary>
	///    Контроллирует перемещение камеры.
	/// </summary>
	public class CameraMovingController : MonoBehaviour
	{
		public static Single SpeedFactor = 16f; //Скорость прокрутки камеры

		private void Update()
		{
			var translateVector = new Vector2(
				Input.GetAxis("Horizontal"),
				Input.GetAxis("Vertical")
			);

			if (Input.mousePosition != Vector3.zero)
			{
				if ((Input.mousePosition.x < 5) && (Input.mousePosition.x > -5))
					translateVector += Vector2.left;

				if ((Input.mousePosition.x > Screen.width - 5) && (Input.mousePosition.x < Screen.width + 5))
					translateVector += Vector2.right;

				if ((Input.mousePosition.y < 5) && (Input.mousePosition.y > -5))
					translateVector += Vector2.down;

				if ((Input.mousePosition.y > Screen.height - 5) && (Input.mousePosition.y < Screen.height + 5))
					translateVector += Vector2.up;
			}

			if (translateVector != Vector2.zero)
				GetComponent<CameraController>().SetFree();
			_transform.Translate(translateVector * Time.deltaTime * SpeedFactor);
		}

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private Transform _transform;
	}
}