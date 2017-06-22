using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.PlanetTextureGenerators.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Generators
{
	/// <summary>
	///    Представляет генератор планетарных колец.
	/// </summary>
	public class PlanetRings : PlanetTextureGenerator
	{
		/// <summary>
		///    Создает новый экземпляр генератора колец планет.
		/// </summary>
		/// <param name="ySize">Размер текстуры</param>
		/// <param name="planetData">Информация о планете</param>
		public PlanetRings(Int32 ySize, PlanetData planetData) : base(ySize, planetData) { }

		/// <summary>
		///    Разрешение по горизонтали, равно вертикальному.
		/// </summary>
		public override Int32 XSize
		{
			get { return YSize; }
		}

		/// <summary>
		///    Коэффициент перепадов цветов и прозрачности колец
		/// </summary>
		public Single CurvesRoughness
		{
			get { return _curvesRoughness; }
			set
			{
				if (value >= 0)
				{
					_curvesRoughness = value;
					ParametersChanged = true;
				}
				else
					throw new ArgumentException("CurvesRoughness should be greater then zero.");
			}
		}

		/// <summary>
		///    Радиус внутренней области
		/// </summary>
		public Single InnerRadius
		{
			get { return _innerRadius; }
			set
			{
				if (value >= 0 && value <= 1)
				{
					_innerRadius = value;
					ParametersChanged = true;
				}
				else
					throw new ArgumentException("InnerRadius should be in the [0, 1] range.");
			}
		}

		/// <summary>
		///    Радиус внешней области
		/// </summary>
		public Single OuterRadius
		{
			get { return _outerRadius; }
			set
			{
				if (value >= 0 && value <= 1)
				{
					_outerRadius = value;
					ParametersChanged = true;
				}
				else
					throw new ArgumentException("OuterRadius should be in the [0, 1] range.");
			}
		}

		private static Color[] RandomRingsColors()
		{
			Color[] resultColor;
			switch (Random.Range(0, 2))
			{
				case 0:
					resultColor = new[]
					{
						new Color(69f / 255, 60f / 255, 53f / 255), new Color(74f / 255, 71f / 255, 72f / 255)
					};
					break;

				case 1:
					resultColor = new[]
					{
						new Color(69f / 255, 66f / 255, 61f / 255), new Color(74f / 255, 74f / 255, 76f / 255)
					};
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return resultColor;
		}

		protected override Color[] GenerateTextureColors()
		{
			Int32 ringsCount = (Int32) ((OuterRadius - InnerRadius) * YSize / 2);

			var transparencyMap = Array.ConvertAll(
				Curves.GenerateCurve(ringsCount, Random.Range(0, 255), _curvesRoughness, Random.Range(0, 255)),
				dot => (Single) dot / 255
			); //Определяет спектр прозрачности

			var colorMap = Array.ConvertAll(
				Curves.GenerateCurve(ringsCount, Random.Range(0, 255), _curvesRoughness, Random.Range(0, 255)),
				dot => (Single) dot / 255
			); //Определяет спектр цветов

			var baseColors = RandomRingsColors();
			var rings = new Color[ringsCount];
			for (Int32 i = 0; i < rings.Length; i++)
			{
				rings[i] = Color.Lerp(baseColors[0], baseColors[1], colorMap[i]);
				rings[i].a = transparencyMap[i];
			}

			var center = new Vector2(YSize / 2, YSize / 2);
			var colors = new Color[YSize * YSize];

			Int32 counter = 0;
			for (Int32 x = 0; x < YSize; x++)
			for (Int32 y = 0; y < XSize; y++)
			{
				var posit = new Vector2(x, y);
				Single dist = Vector2.Distance(posit, center);
				colors[counter] = dist < OuterRadius * YSize / 2 && dist > InnerRadius * YSize / 2
					? rings[(Int32) (dist * 2 * ringsCount / YSize)]
					: new Color(0, 0, 0, 0);

				counter++;
			}

			return colors;
		}

		protected override Color[] GenerateHeighmapColors()
		{
			return null;
		}

		private Single _curvesRoughness = 0.5f;

		private Single _innerRadius = 0.7f;

		private Single _outerRadius = 1.0f;
	}
}