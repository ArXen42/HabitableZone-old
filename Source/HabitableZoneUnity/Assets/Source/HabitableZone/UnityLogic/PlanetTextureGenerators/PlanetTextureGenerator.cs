using System;
using HabitableZone.Core.World.Universe.CelestialBodies;
using UnityEngine;

/// <summary>
///Представляет генераторы текстур планет.
/// </summary>
namespace HabitableZone.UnityLogic.PlanetTextureGenerators
{
	/// <summary>
	///    Базовый класс для генераторов планет.
	/// </summary>
	public abstract class PlanetTextureGenerator //TODO: Rework after restructuring celestials model
	{
		public static Int32 DefaultResolution = 256; //Разрешение текстур планет

		/// <summary>
		///    Создает новый экземпляр генератора с заданным разрешением (если оно удовлетворяет ограничениям) и зерном случайных чисел.
		/// </summary>
		/// <param name="ySize">Вертикальное разрешение</param>
		/// <param name="planetData">Ссылка на планету.</param>
		protected PlanetTextureGenerator(Int32 ySize, PlanetData planetData)
		{
			PlanetData = planetData;
			YSize = ySize;
		}

		public virtual PlanetData PlanetData
		{
			get { return _planetData; }
			set
			{
				if (value != null)
					_planetData = value;
				else
					throw new NullReferenceException("Planet data should exists.");
			}
		}

		public virtual Int32 YSize
		{
			get { return ySize; }
			set
			{
				if (value % 2 == 0)
				{
					ySize = value;
					ParametersChanged = true;
				}
			}
		}

		/// <summary>
		///    Разрешение по горизонтали. По умолчанию равно удвоенному вертикальному.
		/// </summary>
		public virtual Int32 XSize
		{
			get { return YSize * 2; }
		}

		/// <summary>
		///    Возвращает текстуру, сгенерированную с текущими параметрами
		/// </summary>
		/// <returns>Текстура в виде массива Color</returns>
		public virtual Color[] TextureColors
		{
			get
			{
				if (ParametersChanged)
					GenerateAllTextures();

				return textureColors;
			}
		}

		/// <summary>
		///    Возвращает карту высот, сгенерированную с текущими параметрами
		/// </summary>
		/// <returns>Карта высот в виде массива Color</returns>
		public virtual Color[] HeighmapColors
		{
			get
			{
				if (ParametersChanged)
					GenerateAllTextures();

				return heighmapColors;
			}
		}

		/// <summary>
		///    Возвращает карту нормалей, сгенерированную с текущими параметрами
		/// </summary>
		/// <returns>Карта нормалей в виде массива Color</returns>
		public virtual Color[] NormalmapColors
		{
			get
			{
				if (ParametersChanged)
					GenerateAllTextures();

				return normalmapColors;
			}
		}

		protected abstract Color[] GenerateTextureColors();

		protected abstract Color[] GenerateHeighmapColors();

		protected virtual Color[] GenerateNormalmapColors() //TODO: сделать virtual методом, преобразующим карту высот
		{
			return null;
		}

		/// <summary>
		///    Выполняет перегенерацию всех текстур и сброс флага parametersChanged
		/// </summary>
		protected void GenerateAllTextures()
		{
			textureColors = GenerateTextureColors();
			heighmapColors = GenerateHeighmapColors();
			normalmapColors = GenerateNormalmapColors();

			ParametersChanged = false;
		}

		/// <summary>
		///    Если true, то при следующем вызове будет произведена перегенерация всех трех текстур
		/// </summary>
		protected Boolean ParametersChanged;

		protected PlanetData _planetData;

		protected Color[] textureColors, heighmapColors, normalmapColors;

		/// <summary>
		///    Разрешение по вертикали (в пикселах). По умолчанию должно быть кратно двум.
		/// </summary>
		protected Int32 ySize;
	}
}