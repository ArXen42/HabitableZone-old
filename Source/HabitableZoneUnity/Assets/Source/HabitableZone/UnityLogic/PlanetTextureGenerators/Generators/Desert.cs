using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.PlanetTextureGenerators.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Generators
{
	public class Desert : PlanetTextureGenerator
	{
		private static Color GenerateColdColors()
		{
			Color clr;

			Int32 rnd = Random.Range(0, 2);
			switch (rnd)
			{
				case 0:
					clr = new Color(183f / 255, 114f / 255, 47f / 255) / 4; //Здравствуй Марс
					break;

				case 1:
					clr = new Color(100f / 255, 105f / 255, 120f / 255) / 2; //TODO...
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return clr;
		}

		public Desert(Int32 ySize, PlanetData planetData, Single roughness = 1.5f, Single snowRoughness = 0.03f)
			: base(ySize, planetData)
		{
			SnowRoughness = snowRoughness;

			_heighmapGen = new HeighmapGenerator(YSize + 1, 1.5f);
			Roughness = roughness;
		}

		public Single Roughness
		{
			get { return _heighmapGen.Roughness; }
			set
			{
				if (value > 0)
				{
					_heighmapGen.Roughness = value;
					ParametersChanged = true;
				}
				else
				{
					throw new ArgumentException("Roughness should be greater then zero!");
				}
			}
		}

		public Single SnowRoughness
		{
			get { return _snowRoughness; }
			set
			{
				if (value > 0)
				{
					_snowRoughness = value;
					ParametersChanged = true;
				}
				else
				{
					throw new ArgumentException("Snow roughness should be greater then zero");
				}
			}
		}


		protected override Color[] GenerateTextureColors()
		{
			_heighmapGen.GenerateDiamondSquareHeighmap();
			_heighmapGen.Sqr();

			var colors = new Color[YSize * XSize];

			var baseColor1 = GenerateColdColors();
			var baseColor2 = baseColor1 * 3f;

			_snowEdge = YSize / 5;
			GenerateSnowCorners();

			Int32 counter = 0;
			for (Int32 y = 0; y < YSize; y++)
			for (Int32 x = 0; x < XSize; x++)
			{
				if (_planetData.Temperature < 310 &&
					 (y < _southSnow[x] + Random.Range(-10, 10) || y > _northSnow[x] + Random.Range(-10, 10))) //Делаем снег
				{
					colors[counter] = new Color(Random.Range(0.8f, 0.85f), Random.Range(0.8f, 0.85f), Random.Range(0.85f, 0.9f));
					colors[counter] += baseColor1 / 10;
				}
				else
				{
					Single height = _heighmapGen.Heighmap[x, y];
					colors[counter] = Color.Lerp(baseColor1, baseColor2, height + Random.Range(-0.05f, 0.05f));
				}

				counter++;
			}

			return colors;
		}

		protected override Color[] GenerateHeighmapColors()
		{
			var colors = new Color[YSize * XSize];

			Int32 counter = 0;
			for (Int32 y = 0; y < YSize; y++)
			for (Int32 x = 0; x < XSize; x++)
			{
				Single height = _heighmapGen.Heighmap[x, y];
				if (height > 1.1f)
					height = 1f;

				colors[counter] = new Color(height, height, height);

				counter++;
			}

			return colors;
		}

		private void GenerateSnowCorners()
		{
			_northSnow = Curves.GenerateCurve(XSize, XSize / 2 - _snowEdge, _snowRoughness);
			_southSnow = Curves.GenerateCurve(XSize, _snowEdge, _snowRoughness);
		}

		private Color GenerateHotColors()
		{
			Color clr;

			Int32 rnd = Random.Range(0, 2);
			switch (rnd)
			{
				case 0:
					clr = new Color(112f / 255, 65f / 255, 31f / 255) / 2;
					break;

				case 1:
					clr = new Color(61f / 255, 25f / 255, 6f / 255); //TODO...
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return clr;
		}

		private readonly HeighmapGenerator _heighmapGen;
		private Int32[] _northSnow;

		private Int32 _snowEdge;

		private Single _snowRoughness;
		private Int32[] _southSnow;
	}
}