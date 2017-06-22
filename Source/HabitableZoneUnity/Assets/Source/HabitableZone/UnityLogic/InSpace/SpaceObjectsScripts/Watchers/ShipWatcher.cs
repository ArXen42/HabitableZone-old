using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.World;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	public class ShipWatcher : SpaceObjectWatcher
	{
		public Ship Ship;

		public override SpaceObject SpaceObject => Ship;

		private void Start()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			_transform.position = Units.MetersPositionToUnityUnits(Ship.Position);
			_transform.eulerAngles = new Vector3(0, 0, Ship.Rotation);
		}

		private Transform _transform;
	}
}