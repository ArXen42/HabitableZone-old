using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.ShipLogic.FlightTasks;
using HabitableZone.UnityLogic.InSpace.CameraControl;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts
{
	public sealed class HyperjumpEffectController : MonoBehaviour
	{
		private ShipWatcher _watcher;
		private ParticleSystem _particleSystem;

		private void Start()
		{
			_watcher = GetComponentInParent<ShipWatcher>();
			_particleSystem = GetComponent<ParticleSystem>();

			_watcher.Ship.CurrentTaskUpdatedOrChanged += OnShipFlightTaskChangedOrUpdated;
			_watcher.Ship.CurrentTaskComplete += OnShipCurrentFlightTaskComplete;

			OnShipFlightTaskChangedOrUpdated(null, null);
		}

		private void OnDisable()
		{
			_watcher.Ship.CurrentTaskUpdatedOrChanged -= OnShipFlightTaskChangedOrUpdated;
			_watcher.Ship.CurrentTaskComplete -= OnShipCurrentFlightTaskComplete;
		}

		private void OnShipFlightTaskChangedOrUpdated(Ship ship, FlightTask data)
		{
			var flightTask = _watcher.Ship.CurrentFlightTask as HyperjumpFlightTask;
			if (flightTask != null)
			{
				if (flightTask.InnerFlightTask is EnterHyperspaceFlightTask ||
				    flightTask.InnerFlightTask is ExitingHyperspaceFlightTask)
				{
					_particleSystem.Play();
					Camera.main.GetComponent<CameraController>().SetFree();
				}
				else
					_particleSystem.Stop();
			}
			else
				_particleSystem.Stop();
		}

		private void OnShipCurrentFlightTaskComplete(Ship ship, FlightTask data)
		{
			_particleSystem.Stop();
		}
	}
}