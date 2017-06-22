using System;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts
{
	/// <summary>
	///    Every Unity project should have a rotator, shouldn't it?
	/// </summary>
	public class PlanetRotator : MonoBehaviour
	{
		public Single RotationSpeed = 0.5f;

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			_transform.Rotate(0, RotationSpeed, 0);
		}

		private Transform _transform;
	}
}