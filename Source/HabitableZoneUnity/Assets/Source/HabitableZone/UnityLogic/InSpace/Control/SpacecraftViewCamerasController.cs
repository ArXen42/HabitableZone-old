using System;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.Control
{
	public class SpacecraftViewCamerasController : MonoBehaviour
	{
		public Camera HullCamera => _hullCamera;
		public Camera StructureCamera => _structureCamera;

		private void OnEnable()
		{
			transform.parent = GameObject.Find("IPDK(Clone)").transform; //Crutch
			transform.localPosition = Vector3.zero;
		}

		private void Update()
		{
			transform.Rotate(_rotateSpeed * Input.GetAxis("Vertical"), 0, 0, Space.Self);
			transform.Rotate(0, 0, _rotateSpeed * Input.GetAxis("Horizontal"), Space.World);
		}

		[SerializeField] private Single _rotateSpeed;
		[SerializeField] private Camera _hullCamera;
		[SerializeField] private Camera _structureCamera;
	}
}