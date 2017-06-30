using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.PlanetTextureGenerators.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Generators
{
	public sealed class Terra : PlanetTextureGenerator
	{
		private const Single SnowRoughness = 0.03f; //TODO: при необходимости - добавить возможность изменения
		private const Single GreenRoughness = 0.1f;

		public Terra(Int32 ySize, PlanetData planetData, Single waterLevel = 0.25f, Single roughness = 1.5f)
			: base(ySize, planetData)
		{
			_heighmapGen = new HeighmapGenerator(YSize + 1, 1.5f);

			Roughness = roughness;
			WaterLevel = waterLevel;
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

		protected override Color[] GenerateTextureColors()
		{
			_heighmapGen.GenerateDiamondSquareHeighmap();
			_heighmapGen.Sqr();

			_snowEdge = YSize / 6;
			_greenEdge = YSize / 2;
			GenerateBiomes();

			var colors = new Color[YSize * XSize];

			Int32 counter = 0;
			for (Int32 y = 0; y < YSize; y++)
			for (Int32 x = 0; x < XSize; x++)
			{
				Single height = _heighmapGen.Heighmap[x, y];

				if (height < WaterLevel)
				{
					colors[counter] = OceanColor(height);
				}
				else
				{
					if (Curves.CheckInRange(y + Random.Range(-10, 10), _southGreen[x], _northGreen[x]))
					{
						colors[counter] = DesertColor(height);
						if (height < WaterLevel + 0.1f + Random.Range(-0.1f, 0.1f))
							colors[counter] = GreenColor(height);
					}
					else
					{
						colors[counter] = GreenColor(height);
					}

					if (height > 0.82f + Random.Range(-0.05f, 0.05f))
						colors[counter] = MountainsColor(height);
				}
				if (y < _southSnow[x] + Random.Range(-10, 10) || y > _northSnow[x] + Random.Range(-10, 10))
					colors[counter] = SnowColor(height);

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

		private Single WaterLevel
		{
			get { return _waterLevel; }
			set
			{
				if (value > 0)
				{
					_waterLevel = value;
					ParametersChanged = true;
				}
				else
				{
					throw new ArgumentException("Water level should be greater then zero!");
				}
			}
		}

		private void GenerateBiomes()
		{
			_northGreen = Curves.GenerateCurve(XSize, XSize / 2 - _greenEdge, GreenRoughness);
			_southGreen = Curves.GenerateCurve(XSize, _greenEdge, GreenRoughness);
			_northSnow = Curves.GenerateCurve(XSize, XSize / 2 - _snowEdge, SnowRoughness);
			_southSnow = Curves.GenerateCurve(XSize, _snowEdge, SnowRoughness);
		}

		private Color SnowColor(Single height)
		{
			if (height < WaterLevel + Random.Range(-0.04f, 0.04f))
				return new Color(Random.Range(0.8f, 0.85f), Random.Range(0.8f, 0.85f), Random.Range(0.85f, 0.9f));
			return new Color(
				0.005f / height + Random.Range(0.8f, 0.85f),
				0.005f / height + Random.Range(0.8f, 0.85f),
				0.01f / height + Random.Range(0.8f, 0.85f)
			);
		}

		private Color OceanColor(Single height)
		{
			return new Color(
				height / 5,
				height / 5,
				0.2f + height / 2 + Random.Range(-0.02f, 0.02f)
			);
		}

		private Color GreenColor(Single height)
		{
			var color = new Color();
			color.g = 0.1f / height + 0.05f + Random.Range(-0.04f, 0.04f);

			if (height < WaterLevel + 0.1f)
				color.g -= WaterLevel + 0.1f - height;

			if (color.g > 0.5f)
				color.g = 0.5f / height + 0.05f + Random.Range(-0.04f, 0.04f);

			color.r = 0;
			color.b = 0;

			return color;
		}

		private Color DesertColor(Single height)
		{
			return new Color(
				Random.Range(0.7f, 0.85f) + height / 2 - 0.35f,
				Random.Range(0.8f, 0.95f) + height / 2 - 0.35f,
				Random.Range(0.25f, 0.35f) + height / 2 - 0.35f
			);
		}

		private Color MountainsColor(Single height)
		{
			Single rnd = Random.Range(-0.03f, 0.03f);
			if (height > 1.1f)
				height = 1.1f + Random.Range(-0.05f, 0.05f);

			return new Color(
				height * height / 2 + rnd - 0.1f,
				height * height / 2 + rnd - 0.1f,
				height * height / 2 + rnd - 0.05f
			);
		}


		private readonly HeighmapGenerator _heighmapGen;
		private Int32 _greenEdge;

		private Int32[] _northGreen;

		private Int32[] _northSnow;

		private Int32 _snowEdge;
		private Int32[] _southGreen;
		private Int32[] _southSnow;

		private Single _waterLevel;
	}
}