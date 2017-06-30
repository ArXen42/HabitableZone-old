using System;
using Random = UnityEngine.Random;

namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Common
{
	/// <summary>
	///    Представляет генератор 2D карт высот для планет и операции над ними
	/// </summary>
	public class HeighmapGenerator
	{
		private static Boolean IsPowerOfTwo(Int32 value, Int32 powOfTwo = 1)
		{
			if (value == powOfTwo)
				return true;

			return powOfTwo <= value && IsPowerOfTwo(value, powOfTwo * 2);
		}

		/// <summary>
		///    Создает новый экземпляр генератора с заданными разрешением и величиной перепадов высот.
		/// </summary>
		/// <param name="ySize">Разрешение по вертикали</param>
		/// <param name="roughness">Коэффициент перепадов высот</param>
		public HeighmapGenerator(Int32 ySize, Single roughness)
		{
			YSize = ySize;
			Roughness = roughness;
		}

		/// <summary>
		///    Разрешение по вертикали (в пикселах). Должно соответствовать виду 2^n + 1.
		/// </summary>
		public Int32 YSize
		{
			get { return _ySize; }
			set
			{
				if (IsPowerOfTwo(value - 1))
				{
					_ySize = value;
					WasParametersChanged = true;
				}
				else
				{
					throw new ArgumentException("Size of heighmap should be of the form 2^n + 1");
				}
			}
		}

		/// <summary>
		///    Разрешение по горизонтали (в пикселах). Выводится автоматически.
		/// </summary>
		public Int32 XSize => YSize * 2 - 1;

		/// <summary>
		///    Коэффициент величины перепадов высот.
		/// </summary>
		public Single Roughness { get; set; }


		/// <summary>
		///    Если true, то при следующем вызове карты высот (Heighmap) произойдет ее перегенерация.
		///    Также сбрасывается при ручном вызове метода генерации (полезно для дальнейшей модификации).
		/// </summary>
		public Boolean WasParametersChanged { get; private set; }

		/// <summary>
		///    Карта высот в виде двумерного массива float, сгенерированная с текущими параметрами. По умолчанию используется
		///    алгоримт Diamond-Square.
		/// </summary>
		public Single[,] Heighmap
		{
			get
			{
				if (WasParametersChanged)
					GenerateDiamondSquareHeighmap();

				return _heighmap;
			}
		}

		public void Sqr()
		{
			for (Int32 x = 0; x < XSize; x++)
			for (Int32 y = 0; y < YSize; y++)
				_heighmap[x, y] *= _heighmap[x, y];
		}

		public void Invert()
		{
			for (Int32 x = 0; x < XSize; x++)
			for (Int32 y = 0; y < YSize; y++)
			{
				_heighmap[x, y] = 1 - _heighmap[x, y];
				if (_heighmap[x, y] > 1)
					_heighmap[x, y] = 2 - _heighmap[x, y];

				if (_heighmap[x, y] < 0)
					_heighmap[x, y] = -_heighmap[x, y];
			}
		}

		/// <summary>
		///    Выполняет генерацию карты высот с помощью алгоритма Diamond-Square
		/// </summary>
		public void GenerateDiamondSquareHeighmap()
		{
			_heighmap = new Single[XSize, YSize];
			_heighmap[0, 0] = Random.Range(0.3f, 1.0f);
			_heighmap[0, YSize - 1] = Random.Range(0.3f, 1f);
			_heighmap[XSize - 1, YSize - 1] = Random.Range(0.3f, 1f);
			_heighmap[XSize - 1, 0] = Random.Range(0.3f, 1f);

			_heighmap[YSize - 1, YSize - 1] = Random.Range(0.3f, 1f);
			_heighmap[YSize - 1, 0] = Random.Range(0.3f, 1f);

			for (Int32 l = (YSize - 1) / 2; l > 0; l /= 2)
			for (Int32 x = 0; x < XSize - 1; x += l)
			{
				if (x >= YSize - l)
					lrflag = true;
				else
					lrflag = false;

				for (Int32 y = 0; y < YSize - 1; y += l)
					DiamondSquare(x, y, x + l, y + l);
			}

			WasParametersChanged = false;
		}


		private void Square(Int32 lx, Int32 ly, Int32 rx, Int32 ry)
		{
			Int32 l = (rx - lx) / 2;

			Single a = _heighmap[lx, ly]; //  B--------C
			Single b = _heighmap[lx, ry]; //  |        |
			Single c = _heighmap[rx, ry]; //  |   ce   |
			Single d = _heighmap[rx, ly]; //  |        |
			Int32 cex = lx + l; //  A--------D
			Int32 cey = ly + l;

			_heighmap[cex, cey] = (a + b + c + d) / 4 +
										 Random.Range(-l * 2 * Roughness / YSize, l * 2 * Roughness / YSize);
		}

		private void Diamond(Int32 tgx, Int32 tgy, Int32 l)
		{
			Single a, b, c, d;

			a = tgy - l >= 0
				? _heighmap[tgx, tgy - l]
				: _heighmap[tgx, YSize - l];

			if (tgx - l >= 0)
				b = _heighmap[tgx - l, tgy]; //      C--------
			else //      |        |
			if (lrflag) // B---t g----D  |
				b = _heighmap[XSize - l, tgy]; //      |        |
			else //      A--------
				b = _heighmap[YSize - l, tgy];


			c = tgy + l < YSize
				? _heighmap[tgx, tgy + l]
				: _heighmap[tgx, l];

			if (lrflag)
				d = tgx + l < XSize
					? _heighmap[tgx + l, tgy]
					: _heighmap[l, tgy];
			else if (tgx + l < YSize)
				d = _heighmap[tgx + l, tgy];
			else
				d = _heighmap[l, tgy];

			_heighmap[tgx, tgy] = (a + b + c + d) / 4 +
										 Random.Range(-l * 2 * Roughness / YSize, l * 2 * Roughness / YSize);
		}

		private void DiamondSquare(Int32 lx, Int32 ly, Int32 rx, Int32 ry)
		{
			Int32 l = (rx - lx) / 2;

			Square(lx, ly, rx, ry);

			Diamond(lx, ly + l, l);
			Diamond(rx, ry - l, l);
			Diamond(rx - l, ry, l);
			Diamond(lx + l, ly, l);
		}

		private Single[,] _heighmap; //TODO: перевести в byte для экономии памяти.


		private Int32 _ySize;

		private Boolean lrflag;
	}
}