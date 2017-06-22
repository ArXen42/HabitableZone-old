using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe.CelestialBodies;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	public class StarWatcher : SpaceObjectWatcher
	{
		public Star Star;

		public override SpaceObject SpaceObject => Star;

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			var vector2Position = Units.MetersPositionToUnityUnits(Star.Position);
			_transform.position = new Vector3(vector2Position.x, vector2Position.y, 10);
		}

		private Transform _transform;
	}
}