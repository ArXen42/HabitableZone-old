using System;
using HabitableZone.Core.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.Shared.GUI
{
	/// <summary>
	///    Sets localized string in the UnityEngine.UI.Text.text.
	/// </summary>
	public class TextLocalizer : MonoBehaviour
	{
		private void OnEnable()
		{
			LocalizationManager.LocalizationLanguageChanged += SetText;
			SetText(LocalizationManager.LocalizationLanguage);
		}

		private void OnDisable()
		{
			LocalizationManager.LocalizationLanguageChanged -= SetText;
		}

		private void SetText(SystemLanguage language)
		{
			if (language != SystemLanguage.Unknown)
				GetComponent<Text>().text = LocalizationManager.GetLocalizationString(_localizationKey).Value;
		}

		[SerializeField] private String _localizationKey;
	}
}