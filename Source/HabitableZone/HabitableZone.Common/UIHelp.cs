using System;
using UnityEngine;

namespace HabitableZone.Common
{
	/// <summary>
	///    Возможно часть функций этого класса уже есть в API Unity. А может быть даже все. Скорее всего они лучше.
	///    Но я не нашел нужных мне, возможно они просто глубоко зарыты. А потому сюда буду записывать свои велосипеды.
	/// </summary>
	public static class UIHelp
	{
		/// <summary>
		///    Проверяет, находится ли точка в мировых координатах (т.е. в пиксельных для Screen space - Overlay, для него и
		///    предназначен) внутри заданного RectTransform'а.
		/// </summary>
		/// <param name="tgt">Заданный RectTransform</param>
		/// <param name="point">Проверяемая точка</param>
		/// <returns>True, если целевой Rect содержит точку.</returns>
		public static Boolean Contains(RectTransform tgt, Vector2 point)
		{
			var leftBottom = (Vector2) tgt.position - new Vector2(tgt.rect.width * tgt.pivot.x, tgt.rect.height * tgt.pivot.y);
			var rightTop = leftBottom + tgt.rect.size;

			return point.x > leftBottom.x && point.x < rightTop.x && point.y > leftBottom.y && point.y < rightTop.y;
		}

		/// <summary>
		///    Возвращает нормализованные координаты точки относительно RectTransform (и проверяет, содержится ли точка внутри в
		///    принципе)
		/// </summary>
		/// <param name="tgt">RectTransform</param>
		/// <param name="point">Точка</param>
		/// <param name="result">Нормализованные локальные координаты</param>
		/// <returns>Содержится ли точка внутри</returns>
		public static Boolean GetLocalNormalizedCoords(RectTransform tgt, Vector2 point, out Vector2 result)
		{
			var leftBottom = (Vector2) tgt.position - new Vector2(tgt.rect.width * tgt.pivot.x, tgt.rect.height * tgt.pivot.y);
			var rightTop = leftBottom + tgt.rect.size;

			result = rightTop - point;
			result.x /= tgt.rect.width;
			result.y /= tgt.rect.height;
			result.x = 1 - result.x;
			result.y = 1 - result.y; //В классической системе координат, ноль слева снизу

			return point.x > leftBottom.x && point.x < rightTop.x && point.y > leftBottom.y && point.y < rightTop.y;
		}

		/// <summary>
		///    Пробрасывает экранные координаты через элемент UI, считая его выводом камеры, и возвращет результат в виде луча из
		///    этой камеры.
		/// </summary>
		/// <param name="tgt">Элемент UI (RectTransform)</param>
		/// <param name="cam">Камера</param>
		/// <param name="point">Экранные координаты</param>
		/// <param name="result">Результирующий луч</param>
		/// <returns>Исправить</returns>
		/// <remarks>Рекомендуется использать для проброса клика мыши/тапа через RenderTexture</remarks>
		public static Boolean ForwardRaycastThrowUIObject(RectTransform tgt, Camera cam, Vector2 point, out Ray result)
		{
			Vector2 localCoordinates;
			if (GetLocalNormalizedCoords(tgt, point, out localCoordinates))
			{
				var camCoordinates = new Vector2(localCoordinates.x * cam.pixelWidth, localCoordinates.y * cam.pixelHeight);

				result = cam.ScreenPointToRay(camCoordinates);
				return true;
			}
			Debug.LogWarning("Point isn't over rect, result is a new Ray");
			result = new Ray();
			return false;
		}
	}
}