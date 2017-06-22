using System;
using HabitableZone.Core.Localization;
using HabitableZone.Core.World.Universe;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.HUD.SpaceObjectInfo
{
	public class PlanetInfoController : MonoBehaviour
	{
		private void Awake()
		{
			GetComponentInParent<SpaceObjectInfoController>().WatcherUnderCursorChanged += OnWatcherUnderCursorChanged;
			gameObject.SetActive(false);
		}

		private void OnWatcherUnderCursorChanged(SpaceObjectWatcher watcher)
		{
			var planetWatcher = watcher as PlanetWatcher;
			if (planetWatcher == null)
				gameObject.SetActive(false);
			else
			{
				UpdateValues(planetWatcher.Planet);
				gameObject.SetActive(true);
			}
		}

		private void UpdateValues(Planet planet)
		{
			_planetTypeText.text = LocalizationManager.GetLocalizationString("UI.Space.HUD.PlanetTypes." + planet.Type).Value;
			_planetMassValueText.text = Math.Round(planet.Mass / Constants.EarthMass, 1).ToString();
			_orbitRadiusValueText.text = Math.Round(planet.OrbitRadius * Units.MetersToAu, 2).ToString();
			_orbitPeriodValueText.text = planet.OrbitPeriod.Days.ToString();
			_planetTemperatureText.text = Mathf.RoundToInt(planet.Temperature).ToString();
		}

		[SerializeField] private Text _planetTypeText;
		[SerializeField] private Text _planetMassValueText;
		[SerializeField] private Text _orbitRadiusValueText;
		[SerializeField] private Text _orbitPeriodValueText;
		[SerializeField] private Text _planetTemperatureText;
	}
}