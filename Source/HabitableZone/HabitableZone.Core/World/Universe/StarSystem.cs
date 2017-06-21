using System;
using System.Collections.Generic;
using System.Linq;
using HabitableZone.Common;
using UnityEngine;

namespace HabitableZone.Core.World.Universe
{
	/// <summary>
	///    Represents star system. All action is here.
	/// </summary>
	public sealed class StarSystem : IWorldContextProvider
	{
		/// <summary>
		///    Constructs new StarSystem instance from given data and adds it into world.
		/// </summary>
		public StarSystem(WorldContext worldContext, StarSystemData data)
		{
			WorldContext = worldContext;

			ID = data.ID;
			UniverseMapPosition = data.UniverseMapPosition;
			worldContext.StarSystems.Add(this);
		}

		public StarSystemData GetSerializationData()
		{
			return new StarSystemData(this);
		}

		public WorldContext WorldContext { get; }

		/// <summary>
		///    The unique identifier of this system. Can't be changed.
		/// </summary>
		public readonly Guid ID;

		/// <summary>
		///    Position on the universe map.
		/// </summary>
		/// <remarks>
		///    Units? I have no idea.
		/// </remarks>
		public readonly Vector2 UniverseMapPosition;

		/// <summary>
		///    Returns all SpaceObjects in this system.
		/// </summary>
		public IEnumerable<SpaceObject> AllSpaceObjects => _spaceObjects.Values;

		/// <summary>
		///    Returns all SpaceObjects of desired type in this system.
		/// </summary>
		public IEnumerable<T> SpaceObjectsOfType<T>() where T : SpaceObject
		{
			return _spaceObjects.Values.Select(so => so as T).Where(so => so != null);
		}

		/// <summary>
		///    Occurs when some SpaceObject is added into this system.
		/// </summary>
		public event SEventHandler<StarSystem, SpaceObject> SpaceObjectAdded;

		/// <summary>
		///    Occurs when some SpaceObject is removed from this system.
		/// </summary>
		public event SEventHandler<StarSystem, SpaceObject> SpaceObjectRemoved;

		/// <summary>
		///    Internal method used by SpaceObject.LocationID. It shouldn't be used from anywhere else.
		/// </summary>
		internal void AddSpaceObject(SpaceObject spaceObject)
		{
			_spaceObjects.Add(spaceObject.ID, spaceObject);

			SpaceObjectAdded?.Invoke(this, spaceObject);
		}

		/// <summary>
		///    Internal method used by SpaceObject.LocationID. It shouldn't be used from anywhere else.
		/// </summary>
		public void RemoveSpaceObject(SpaceObject spaceObject)
		{
			_spaceObjects.Remove(spaceObject.ID);

			SpaceObjectRemoved?.Invoke(this, spaceObject);
		}

		private readonly Dictionary<Guid, SpaceObject> _spaceObjects = new Dictionary<Guid, SpaceObject>();
	}

	[Serializable]
	public struct StarSystemData
	{
		public StarSystemData(StarSystem starSystem)
		{
			ID = starSystem.ID;
			UniverseMapPosition = starSystem.UniverseMapPosition;
		}

		public StarSystem GetInstanceFromData(WorldContext worldContext)
		{
			return new StarSystem(worldContext, this);
		}

		public Guid ID;
		public Vector2 UniverseMapPosition;
	}
}