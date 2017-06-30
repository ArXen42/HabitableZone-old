using System;
using System.Collections.Generic;
using System.Linq;
using HabitableZone.Core.World.Universe.CelestialBodies;

namespace HabitableZone.Core.World.Universe
{
	/// <summary>
	///    Provides access to world-wide collection of SpaceObjects. Stars, planets, ships, wreckage - everything.
	/// </summary>
	public class SpaceObjects : IWorldContextProvider
	{
		public SpaceObjects(WorldContext worldContext, SpaceObjectsData data)
		{
			WorldContext = worldContext;
			_spaceObjectsDictionary = new Dictionary<Guid, SpaceObject>();
		}

		public WorldContext WorldContext { get; }

		/// <summary>
		///    Returns collection of all objects in the world.
		/// </summary>
		public IEnumerable<SpaceObject> All => _spaceObjectsDictionary.Values;

		public SpaceObjectsData GetSerializationData()
		{
			return new SpaceObjectsData(this);
		}

		/// <summary>
		///    Returns collection of all objects in the world of given type.
		/// </summary>
		public IEnumerable<T> SpaceObjectsOfType<T>() where T : SpaceObject
		{
			return _spaceObjectsDictionary.Values.Select(so => so as T).Where(so => so != null);
		}

		/// <summary>
		///    Returns SpaceObject with given ID.
		/// </summary>
		public SpaceObject ByID(Guid id)
		{
			return _spaceObjectsDictionary[id];
		}

		/// <summary>
		///    Internal method used from SpaceObject constructor. Don't use from anywhere else.
		/// </summary>
		public void Add(SpaceObject spaceObject)
		{
			_spaceObjectsDictionary.Add(spaceObject.ID, spaceObject);
		}

		/// <summary>
		///    Internal method used from SpaceObject.Destroy. Don't use from anywhere else.
		/// </summary>
		public void Remove(SpaceObject spaceObject)
		{
			_spaceObjectsDictionary.Remove(spaceObject.ID);
		}

		/// <summary>
		///    Called immediately after constructor. Separated because initialization of WorldContext fields first is required.
		/// </summary>
		internal void InitializeFromData(SpaceObjectsData data)
		{
			foreach (var spaceObjectData in data.SpaceObjects.Where(soData => soData is StarData))
				spaceObjectData.GetInstanceFromData(WorldContext);

			foreach (var spaceObjectData in data.SpaceObjects.Where(soData => !(soData is StarData)))
				spaceObjectData.GetInstanceFromData(WorldContext);
		}

		private readonly Dictionary<Guid, SpaceObject> _spaceObjectsDictionary;
	}

	[Serializable]
	public struct SpaceObjectsData
	{
		public SpaceObjectsData(SpaceObjects spaceObjects)
		{
			SpaceObjects = spaceObjects.All.Select(so => so.GetSerializationData()).ToArray();
		}

		public SpaceObjects GetInstanceFromData(WorldContext worldContext)
		{
			return new SpaceObjects(worldContext, this);
		}

		public SpaceObjectData[] SpaceObjects;
	}
}