using System.IO;
using NUnit.Framework;
using HabitableZone.Common;
using HabitableZone.Core.SpacecraftStructure;

namespace HabitableZone.Core.Tests
{
	/// <summary>
	///    Provides helper methods for SpacecraftStructure tests.
	/// </summary>
	public static class SpacecraftStructureHelper
	{
		/// <summary>
		///    Returns test spacecraft in fresh test world. It is not player ship.
		/// </summary>
		/// <returns></returns>
		public static Spacecraft GetTestSpacecraft()
		{
			var worldContext = WorldHelper.GetTestWorld();

			var spacececraft = Spacecraft.DeserializeFrom(
				new FileStream($"{TestContext.CurrentContext.TestDirectory}/TestData/TestSpacecraft.json", FileMode.Open),
				worldContext);

			return spacececraft;
		}

		/// <summary>
		///    Returns deserialized test spacecraft data.
		/// </summary>
		public static SpacecraftData GetTestSpacecraftData()
		{
			var stream = new FileStream($"{TestContext.CurrentContext.TestDirectory}/TestData/TestSpacecraft.json", FileMode.Open);
			return Serialization.DeserializeDataFromJson<SpacecraftData>(stream);
		}
	}
}