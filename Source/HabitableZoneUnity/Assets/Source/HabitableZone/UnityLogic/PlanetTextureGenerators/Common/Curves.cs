using System;
using Random = UnityEngine.Random;

namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Common
{
	/// <summary>
	///    Представляет генератор случайных одномерных кривых
	/// </summary>
	public static class Curves
	{
		/// <summary>
		///    Проверяет вхождение значения в диапазон
		/// </summary>
		/// <param name="value">Значение</param>
		/// <param name="min">Левая граница</param>
		/// <param name="max">Правая граница</param>
		/// <returns></returns>
		public static Boolean CheckInRange(Int32 value, Int32 min, Int32 max)
		{
			return value > min && value < max;
		}

		/// <summary>
		///    Создает новую случайную кривую с заданными параметрами
		/// </summary>
		/// <param name="size">Длина</param>
		/// <param name="start">Значение первой точки</param>
		/// <param name="roughness">Коэффициент перепадов значений</param>
		/// <param name="end">Значение последней точки, приравнивается значению первой, если не задано явно</param>
		/// <returns>Кривая</returns>
		public static Int32[] GenerateCurve(Int32 size, Int32 start, Single roughness, Int32 end = -1)
		{
			var curve = new Int32[size];
			end = end < 0 ? start : end; //Если конечное значение не задано сделать равным начальному

			curve[0] = start;
			curve[size - 1] = end;

			MidPointDisplacement1D(ref curve, 0, size - 1, roughness);
			return curve;
		}

		private static void MidPointDisplacement1D(ref Int32[] curve, Int32 l, Int32 r, Single roughness)
		{
			if (r - l > 1)
			{
				curve[(l + r) / 2] = (curve[l] + curve[r]) / 2 + (Int32) Random.Range(-(r - l) * roughness, (r - l) * roughness);
				MidPointDisplacement1D(ref curve, l, (l + r) / 2, roughness);
				MidPointDisplacement1D(ref curve, (l + r) / 2, r, roughness);
			}
		}
	}
}