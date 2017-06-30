using System;
using System.IO;
using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe;
using NUnit.Framework;

namespace HabitableZone.Core.Tests
{
	[TestFixture]
	public class WorldSerializationTests
	{
		[Test]
		public void GeneratedWorld_Reserialization_DontChange()
		{
			//Note: this test can't check if some information is missing in ***Data.
			//It only checks correctness of data transfer between WorldContext and WorldContextData and (just in case) Json serialization between it.

			//Serialize generated world to stream1 -> deserialize -> serialize again to stream2 -> compare streams
			var sourceWorld = UniverseGeneration.GenerateWorld((ShipData) SpacecraftStructureHelper.GetTestSpacecraftData());

			var stream1 = new MemoryStream();
			sourceWorld.SerializeTo(stream1);
			stream1.Position = 0;

			var streamReader1 = new StreamReader(stream1);
			String str1 = streamReader1.ReadToEnd();
			stream1.Position = 0;


			var deserializedWorld = WorldContext.DeserializeFrom(stream1); //Stream1 is disposed after this, but we already have str1
			var stream2 = new MemoryStream();
			deserializedWorld.SerializeTo(stream2);
			stream2.Position = 0;

			var streamReader2 = new StreamReader(stream2);
			String str2 = streamReader2.ReadToEnd();

			streamReader1.Close();
			streamReader2.Close();

			Assert.AreEqual(str1, str2);
		}
	}
}