using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.PlanetTextureGenerators.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Generators
{
	public class Gas : PlanetTextureGenerator
	{
		public Gas(Int32 YSize, PlanetData planetData) : base(YSize, planetData) { }

		protected override Color[] GenerateTextureColors()
		{
			//Когда будет новый генератор, 60 кельвинов посреди кода валяться, конечно же, не будет
			return PlanetData.Temperature > 60
				? GenNormalGigantTex(RandomColor())
				: GenIceGigantTex(RandomColor());
		}

		protected override Color[] GenerateHeighmapColors()
		{
			return null; //WIP
		}

		private static Color RandomColor()
		{
			Color resultColor;

			switch (Random.Range(1, 5))
			{
				case 1:
					resultColor = new Color(93, 92, 70) / 255;
					break;

				case 2:
					resultColor = new Color(75, 87, 146) / 255;
					break;

				case 3:
					resultColor = new Color(111, 63, 30) / 255;
					break;

				case 4:
					resultColor = new Color(42, 84, 42) / 255; //Планета Джул, система Кербол
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return resultColor / 2;
		}

		/// <summary>
		///    Генерирует текстуру газового гиганта с заданными параметрами
		/// </summary>
		/// <param name="curvesCount">Количество полос</param>
		/// <param name="baseColor">Основной цвет</param>
		/// <param name="wideRange">Степень неровности полос</param>
		/// <param name="colorRange">Разброс цветов полос</param>
		/// <param name="bridhnessRange">Разброс яркости полос</param>
		/// <returns></returns>
		private Color[] GenGasTex(Int32 curvesCount, Color baseColor, Int32 wideRange, Single colorRange,
			Single bridhnessRange)
		{
			var curves = new Int32[curvesCount][]; //Создание массива линий

			for (Int32 i = 0; i < curvesCount; i++)
				curves[i] = new Int32[XSize];
			for (Int32 i = 1; i < curvesCount - 1; i++)
				curves[i] = Curves.GenerateCurve(XSize, i * YSize / curvesCount + Random.Range(-wideRange, wideRange),
					(Single) wideRange / 500);
			for (Int32 x = 0; x < XSize; x++)
				curves[curvesCount - 1][x] = YSize - 1;


			var colors = new Color[XSize * YSize];
			var colors2D = new Color[XSize, YSize];

			for (Int32 i = 0; i < curvesCount - 1; i++) //Перебираем промежутки между кривыми и заполняем их
			{
				var currentColor = baseColor;
				currentColor += new Color
				(
					baseColor.r + Random.Range(-colorRange, colorRange),
					baseColor.g + Random.Range(-colorRange, colorRange),
					baseColor.b + Random.Range(-colorRange, colorRange)
				);


				Single rnd = Random.Range(-bridhnessRange, bridhnessRange);
				currentColor += new Color(rnd, rnd, rnd);

				for (Int32 x = 0; x < XSize; x++)
				for (Int32 y = curves[i][x]; y < curves[i + 1][x]; y++)
				{
					var currClr = currentColor;
					currClr += new Color(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));

					colors2D[x, y] = currClr;
				}
			}

			Int32 counter = 0;
			for (Int32 y = 0; y < YSize; y++)
			for (Int32 x = 0; x < XSize; x++)
			{
				colors[counter] = colors2D[x, y];
				counter++;
			}

			return colors;
		}

		private Color[] GenIceGigantTex(Color baseColor)
		{
			return GenGasTex(10, baseColor, 2, 0.01f, 0.08f);
		}

		private Color[] GenNormalGigantTex(Color baseColor)
		{
			return GenGasTex(15, baseColor, 5, 0.02f, 0.2f);
		}
	}
}