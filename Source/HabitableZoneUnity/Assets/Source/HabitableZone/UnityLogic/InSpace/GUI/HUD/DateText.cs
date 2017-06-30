using System;
using HabitableZone.Core.Localization;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.HUD
{
	public class DateText : MonoBehaviour
	{
		private void Update()
		{
			var worldCtl = _sharedGOSpawner.WorldContext.WorldCtl;
			_yearText.text = $"{worldCtl.Date.Year} {LocalizationManager.GetLocalizationString("Shared.Units.Years")}";

			String dateStr = worldCtl.Date.Date.ToLongDateString();
			var splitted = dateStr.Split(' ');
			_dateText.text = splitted[0] + " " + splitted[1];

			_timeText.text = worldCtl.Date.ToShortTimeString();
		}

		[SerializeField] private Text _dateText;

		[SerializeField] private SharedGOSpawner _sharedGOSpawner;
		[SerializeField] private Text _timeText;
		[SerializeField] private Text _yearText;
	}
}