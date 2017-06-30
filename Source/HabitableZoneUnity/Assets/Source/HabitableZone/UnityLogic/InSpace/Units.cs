using System;
using System.Globalization;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace
{
	/// <summary>
	///    Defines constants and methods to convert units.
	/// </summary>
	public class Units
	{
		public static readonly Int32 Multiplier = 10; //Столько млн км содержится в 1 единице transform

		public static Single UnityUnitsToMeters(Single uu)
		{
			return uu * Multiplier * 1e9f;
		}

		public static Vector2 UnityUnitsPositionToMeters(Vector2 uuPosition)
		{
			return uuPosition * Multiplier * 1e9f;
		}

		public static Single MetersToUnityUnits(Single meters)
		{
			return meters / (Multiplier * 1e9f);
		}

		public static Vector2 MetersPositionToUnityUnits(Vector2 metersPosition)
		{
			return metersPosition / (Multiplier * 1e9f);
		}

		public static String GetMegawattsString(Int64 watts)
		{
			return Math.Round(watts / 1e6f, 1).ToString(CultureInfo.InvariantCulture);
		}

		public const Int64 AuToMeters = 149597870700;
		public const Single MetersToAu = 1.0f / AuToMeters;
	}
}