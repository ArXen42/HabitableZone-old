using System;

namespace HabitableZone.Localization.Common
{
	/// <summary>
	///    Represents string localized to some language.
	/// </summary>
	[Serializable]
	public class LocalizationString
	{
		/// <summary>
		///    Constructs empty LocalizationString.
		/// </summary>
		public LocalizationString() { }

		/// <summary>
		///    Constructs LocalizationString with given description and localized value.
		/// </summary>
		public LocalizationString(String description, String value)
		{
			Description = description;
			Value = value;
		}

		public override String ToString()
		{
			return Value;
		}

		/// <summary>
		///    Description of the string.
		/// </summary>
		public String Description { get; internal set; }

		/// <summary>
		///    Translation of the string to desired language.
		/// </summary>
		public String Value { get; internal set; }
	}
}