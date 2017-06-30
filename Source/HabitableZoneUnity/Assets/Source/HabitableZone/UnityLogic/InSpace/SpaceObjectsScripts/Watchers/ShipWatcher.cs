using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.World;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	public class ShipWatcher : SpaceObjectWatcher
	{
		public override SpaceObject SpaceObject => Ship;
		public Ship Ship;

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