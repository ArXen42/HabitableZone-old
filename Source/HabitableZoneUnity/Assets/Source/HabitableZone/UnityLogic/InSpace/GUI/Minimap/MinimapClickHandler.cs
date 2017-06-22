using HabitableZone.Common;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.Minimap
{
	public class MinimapClickHandler : MonoBehaviour
	{
		private void MinimapClickHandle()
		{
			Ray ray;
			UIHelp.ForwardRaycastThrowUIObject(_minimapScreen, _minimapCamera, Input.mousePosition, out ray);
			Vector2 position = ray.GetPoint(9.7f);

			_mainCameraTransform.position = (Vector3) position - Vector3.forward * 10;
		}

		private void Update()
		{
			if (Input.GetButtonDown("Fire1"))
				if (UIHelp.Contains(_minimapScreen, Input.mousePosition))
					MinimapClickHandle();
		}

		private void OnEnable()
		{
			_minimapCamera = GetComponentInChildren<Camera>();
			//Мы ведь не будем делать больше одной камеры для миникарты?
			_minimapScreen = GameObject.Find("MinimapScreen").GetComponent<RectTransform>();
			_mainCameraTransform = Camera.main.gameObject.transform;
		}

		private Transform _mainCameraTransform;

		private Camera _minimapCamera;
		private RectTransform _minimapScreen;
	}
}