using System;
using UnityEngine;

namespace HabitableZone.Common
{
	/// <summary>
	///    Представляет функции для операций с векторами, использующиеся в проекте
	/// </summary>
	public static class Geometry
	{
		/// <summary>
		///    NaN Vector2.
		/// </summary>
		public static readonly Vector2 NaN2 = new Vector2(Single.NaN, Single.NaN);

		/// <summary>
		///    Возвращает позицию точки на плоскости OXY, удаленной на заданное расстояние под заданным углом
		/// </summary>
		/// <param name="start">Начальная точка</param>
		/// <param name="angle">Эйлеров угол (0..360 по часовой стрелке)</param>
		/// <param name="length">Удаление от начальной точки</param>
		/// <returns></returns>
		public static Vector2 VertexOnLine(Vector2 start, Single angle, Single length)
		{
			Single angleX = EulerToOXAngleOfVector(angle);
			return new Vector2(
				start.x + length * Mathf.Cos(angleX * Mathf.Deg2Rad),
				start.y + length * Mathf.Sin(angleX * Mathf.Deg2Rad));
			//Геометрия!
		}

		/// <summary>
		///    Проверяет положение точки относительно прямой
		/// </summary>
		/// <param name="l1">Точка прямой 1</param>
		/// <param name="l2">Точка прямой 2</param>
		/// <param name="point">Проверяемая точка</param>
		/// <returns></returns>
		public static Boolean CheckPointRelativeLine(Vector2 l1, Vector2 l2, Vector2 point)
		{
			Single result = (l2.x - l1.x) * (point.y - l1.y) - (l2.y - l1.y) * (point.x - l1.x);
			return result <= 0;
			//http://www.quickworks.ru/?p=443
		}

		/// <summary>
		///    Возвращает точку на кривой Безье первого порядка (прямой)
		/// </summary>
		/// <param name="start">Начало</param>
		/// <param name="end">Конец</param>
		/// <param name="t">Позиция на прямой 0..1</param>
		/// <returns>Точка на кривой Безье прямой</returns>
		public static Vector2 Bezier1(Vector2 start, Vector2 end, Single t)
		{
			return (1 - t) * start + t * end;
		}

		/// <summary>
		///    Возвращает эйлеровский угол, который данная вектор составляет с вертикалью
		/// </summary>
		public static Single EulerAngleOfVector(Vector2 vec)
		{
			Single angle = Vector2.Angle(Vector2.up, vec);
			if (vec.x > 0)
				angle = 360 - angle;
			return angle; //TODO: передавать просто сразу вектор направления (end - start)
		}

		/// <summary>
		///    Возвращает угол между прямой и осью OX
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static Single OXAngleOfVector(Vector2 start, Vector2 end)
		{
			Single angle = Vector2.Angle(Vector2.right, end - start);
			return end.y < start.y ? -angle : angle;
		}

		/// <summary>
		///    Переводит заданный эйлеровский угол в угол с осью OX
		/// </summary>
		/// <param name="euler">Исходный эйлеровский угол</param>
		/// <returns></returns>
		public static Single EulerToOXAngleOfVector(Single euler)
		{
			Single result = euler + 90;
			if (result > 360)
				result -= 360;

			return result;
		}

		/// <summary>
		///    Переводит заданный эйлеровский угол в угол с осью OY
		/// </summary>
		/// <param name="euler">Исходный эйлеровский угол</param>
		/// <returns></returns>
		public static Single EulerToOYAngleOfVector(Single euler)
		{
			return euler > 180 ? euler - 360 : euler; //Нужно для автопилота в меню
		}

		/// <summary>
		///    Переводит заданную тройку эйлеровских углов в тройку углов с осью OY
		/// </summary>
		/// <param name="eulerAngles"></param>
		/// <returns></returns>
		public static Vector3 EulerToOYVector3(Vector3 eulerAngles)
		{
			return new Vector3
			(
				EulerToOYAngleOfVector(eulerAngles.x),
				EulerToOYAngleOfVector(eulerAngles.y),
				EulerToOYAngleOfVector(eulerAngles.z)
			);
		}
	}
}