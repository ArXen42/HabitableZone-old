using System;

namespace HabitableZone.Common
{
	public static class RandomHelpers
	{
		public static Single Range(this Random random, Single min, Single max)
		{
			Single delta = max - min;
			return min + (Single) random.NextDouble()*delta;
		}
	}
}