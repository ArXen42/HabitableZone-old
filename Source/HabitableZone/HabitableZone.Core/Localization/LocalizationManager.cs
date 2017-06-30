using System;
using System.Collections.Generic;
using System.IO;
using HabitableZone.Common;
using HabitableZone.Localization.Common;
using UnityEngine;

namespace HabitableZone.Core.Localization
{
	/// <summary>
	///    Provides acess to localized strings.
	/// </summary>
	public static class LocalizationManager
	{
		/// <summary>
		///    Initializes LocalizationLanguage as unknown and tries to load default localization.
		/// </summary>
		static LocalizationManager()
		{
			LocalizationLanguage = SystemLanguage.Unknown;
			try
			{
				Load(Application.systemLanguage);
			}
			catch (LocalizationLoadException)
			{
				Debug.Log($"Can't load {Application.systemLanguage}, loading English.");
				Load(SystemLanguage.English);
			}
		}

		/// <summary>
		///    Occurs when LocalizationLanguage is changed.
		/// </summary>
		public static event SEventHandler<SystemLanguage> LocalizationLanguageChanged;

		/// <summary>
		///    Currently loaded localization's language. SystemLanguage.Unknown if nothing is loaded.
		/// </summary>
		public static SystemLanguage LocalizationLanguage { get; private set; }

		/// <summary>
		///    Loads localization for given language from resource files.
		/// </summary>
		/// <remarks>
		///    Simply loads json as a dictionaries hierarchy.
		/// </remarks>
		public static void Load(SystemLanguage language)
		{
			try
			{
				var textAsset = Resources.Load<TextAsset>(@"Localizations\" + language + @"Localization");
				using (var stream = new MemoryStream(textAsset.bytes))
				{
					_localization = Serialization.DeserializeDataFromJson<GameLocalization>(stream);
				}

				LocalizationLanguage = language;

				LocalizationLanguageChanged?.Invoke(language);
			}
			catch (Exception exception)
			{
				throw new LocalizationLoadException(
					$"Can't load localization of language \"{language}\".",
					exception);
			}
		}

		/// <summary>
		///    Returns localized string associated with the specified key.
		/// </summary>
		/// <remarks>
		///    Example: UI_VersionText
		/// </remarks>
		public static LocalizationString GetLocalizationString(String keyString)
		{
			try
			{
				return _localization[keyString];
			}
			catch (Exception exception)
			{
				throw new KeyNotFoundException($"Localization doesn't contains string of key {keyString}.",
					exception);
			}
		}

		private static GameLocalization _localization;
	}

	public class LocalizationLoadException : Exception
	{
		public LocalizationLoadException(String message, Exception innerException) : base(message, innerException) { }
	}
}