using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe.CelestialBodies;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	public class AsteroidFieldWatcher : SpaceObjectWatcher
	{
		public override SpaceObject SpaceObject => AsteroidField;
		public AsteroidField AsteroidField;

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void Start()
		{
			GetComponent<ParticleSystem>().Stop();
			_transform.localScale = new Vector3(1, 1, 1) * Units.MetersToUnityUnits(AsteroidField.Radius);
			_transform.position = Units.MetersPositionToUnityUnits(AsteroidField.Position);
			GetComponent<ParticleSystem>().Play();
		}

		private void Update()
		{
			_transform.eulerAngles = new Vector3(0, 0, AsteroidField.Rotation);
		}

		private Transform _transform;
	}
}