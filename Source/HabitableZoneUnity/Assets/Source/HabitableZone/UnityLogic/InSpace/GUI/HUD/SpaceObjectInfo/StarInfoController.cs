using System;
using System.Globalization;
using System.Linq;
using HabitableZone.Core.Localization;
using HabitableZone.Core.World.Universe;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.HUD.SpaceObjectInfo
{
	public class StarInfoController : MonoBehaviour
	{
		private void Awake()
		{
			GetComponentInParent<SpaceObjectInfoController>().WatcherUnderCursorChanged += OnWatcherUnderCursorChanged;
			gameObject.SetActive(false);
		}

		private void OnWatcherUnderCursorChanged(SpaceObjectWatcher watcher)
		{
			var starWatcher = watcher as StarWatcher;
			if (starWatcher == null)
				gameObject.SetActive(false);
			else
			{
				UpdateValues(starWatcher.Star);
				gameObject.SetActive(true);
			}
		}

		private void UpdateValues(Star star)
		{
			_starTypeText.text = LocalizationManager.GetLocalizationString("UI.Space.HUD.StarTypes." + star.Type).Value;
			_massValueText.text = Math.Round(star.Mass / Constants.SolMass, 2).ToString(CultureInfo.InvariantCulture);
			_planetsCountText.text = star.Planets.Count().ToString();
		}

		[SerializeField] private Text _starTypeText;
		[SerializeField] private Text _massValueText;
		[SerializeField] private Text _planetsCountText;
	}
}