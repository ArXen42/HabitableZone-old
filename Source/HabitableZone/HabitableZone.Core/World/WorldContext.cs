using System;
using System.IO;
using HabitableZone.Common;
using HabitableZone.Core.World.Society;
using HabitableZone.Core.World.Universe;

namespace HabitableZone.Core.World
{
	/// <summary>
	///    Game world's root. Provides access to core facilities: space objects and star system collections, WorldCtl and so
	///    on.
	/// </summary>
	public class WorldContext
	{
		/// <summary>
		///    Deserializes world from given stream using project's json settings.
		/// </summary>
		public static WorldContext DeserializeFrom(Stream stream)
		{
			var data = Serialization.DeserializeDataFromJson<WorldContextData>(stream);
			var worldContext = data.GetInstanceFromData();

			return worldContext;
		}

		public WorldContext(WorldContextData data)
		{
			_creationDate = DateTime.Now;

			WorldCtl = data.WorldCtlData.GetInstanceFromData();
			StarSystems = data.StarSystemsData.GetInstanceFromData(this);
			SpaceObjects = data.SpaceObjectsData.GetInstanceFromData(this);

			StarSystems.InitializeFromData(data.StarSystemsData);
			SpaceObjects.InitializeFromData(data.SpaceObjectsData);

			Captains = data.CaptainsData.GetInstanceFromData(this);
		}

		public WorldContextData GetSerializationData()
		{
			return new WorldContextData(this);
		}

		/// <summary>
		///    Serializes world to given stream using project's json settings.
		/// </summary>
		public void SerializeTo(Stream stream)
		{
			Serialization.SerializeDataToJson(GetSerializationData(), stream);
		}

		public readonly Captains Captains;
		public readonly SpaceObjects SpaceObjects;
		public readonly StarSystems StarSystems;

		public readonly WorldCtl WorldCtl;
		private readonly DateTime _creationDate;
	}

	/// <summary>
	///    World serialization data. Other core classes serializes using the same pattern.
	/// </summary>
	public struct WorldContextData
	{
		public WorldContextData(WorldContext worldContext)
		{
			WorldCtlData = worldContext.WorldCtl.GetSerializationData();
			StarSystemsData = worldContext.StarSystems.GetSerializationData();
			SpaceObjectsData = worldContext.SpaceObjects.GetSerializationData();
			CaptainsData = worldContext.Captains.GetSerializationData();
		}

		public WorldContext GetInstanceFromData()
		{
			return new WorldContext(this);
		}

		public WorldCtlData WorldCtlData;
		public StarSystemsData StarSystemsData;
		public SpaceObjectsData SpaceObjectsData;
		public CaptainsData CaptainsData;
	}

	/// <summary>
	///    Defines property to access the WorldContext.
	/// </summary>
	public interface IWorldContextProvider
	{
		WorldContext WorldContext { get; }
	}
}