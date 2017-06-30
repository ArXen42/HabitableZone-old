using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe.CelestialBodies;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	public class PlanetWatcher : SpaceObjectWatcher
	{
		public override SpaceObject SpaceObject => Planet;
		public Planet Planet;

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			var vector2Position = Units.MetersPositionToUnityUnits(Planet.Position);
			_transform.position = new Vector3(vector2Position.x, vector2Position.y, 10);
		}

		private Transform _transform;
	}
}