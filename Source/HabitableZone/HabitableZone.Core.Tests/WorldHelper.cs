using System.IO;
using NUnit.Framework;
using HabitableZone.Core.World;

namespace HabitableZone.Core.Tests
{
	public static class WorldHelper
	{
		public static WorldContext GetTestWorld()
		{
			return WorldContext.DeserializeFrom(
				new FileStream($"{TestContext.CurrentContext.TestDirectory}/TestData/TestWorld.json",FileMode.Open, FileAccess.Read)
			);
		}
	}
}