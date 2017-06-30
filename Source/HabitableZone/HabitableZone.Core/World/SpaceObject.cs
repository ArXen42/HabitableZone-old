using System;
using HabitableZone.Common;
using HabitableZone.Core.World.Universe;
using UnityEngine;

namespace HabitableZone.Core.World
{
	/// <summary>
	///    Base class for all objects that can be in space.
	/// </summary>
	public abstract class SpaceObject : IWorldContextProvider
	{
		/// <summary>
		///    Creates new instance of SpaceObjects and adds it to given world.
		/// </summary>
		/// <remarks>
		///    Does not restore coordinates from data.
		///    Although all SpaceObjects have coordinates, not all of them need to store it.
		/// </remarks>
		protected SpaceObject(WorldContext worldContext, SpaceObjectData data)
		{
			WorldContext = worldContext;

			ID = data.ID;
			worldContext.SpaceObjects.Add(this);

			LocationID = data.LocationID;

			Name = data.Name;

			worldContext.WorldCtl.TurnStarted += OnTurnStarted;
			worldContext.WorldCtl.TurnStopped += OnTurnStopped;
		}

		public WorldContext WorldContext { get; private set; }

		public override Int32 GetHashCode()
		{
			return ID.GetHashCode();
		}

		/// <summary>
		///    Occurs when this SpaceObject changes location.
		/// </summary>
		public event CEventHandler<SpaceObject> LocationChanged;

		/// <summary>
		///    Occurs when this SpaceObject is destroyed.
		/// </summary>
		public event CEventHandler<SpaceObject> Destroyed;

		/// <summary>
		///    ID of StarSystem in which that object currently is.
		/// </summary>
		public Guid LocationID
		{
			get { return _location.ID; }
			set { Location = WorldContext.StarSystems.ByID(value); }
		}

		/// <summary>
		///    The StarSystem in which that object currently is.
		/// </summary>
		public StarSystem Location
		{
			get { return WorldContext.StarSystems.ByID(LocationID); }
			set
			{
				Assert.IsNotNull(value, "Attempted to set null Location.");

				var oldLocation = _location;
				_location = value;

				oldLocation?.RemoveSpaceObject(this);
				_location.AddSpaceObject(this);

				LocationChanged?.Invoke(this);
			}
		}

		/// <summary>
		///    The name of this SpaceObject.
		/// </summary>
		public String Name { get; set; } //TODO: localize

		/// <summary>
		///    Coordinates of this object, meters.
		/// </summary>
		public abstract Vector2 Position { get; }

		public abstract SpaceObjectData GetSerializationData();

		/// <summary>
		///    Removes this SpaceObject from the world.
		/// </summary>
		public void Destroy()
		{
			Destroyed?.Invoke(this);

			WorldContext.SpaceObjects.Remove(this);
			Location.RemoveSpaceObject(this);
			WorldContext = null;
		}

		/// <summary>
		///    The unique identifier of this SpaceObject. Can't be changed.
		/// </summary>
		public readonly Guid ID;

		protected virtual void OnTurnStarted(WorldCtl sender) { }

		protected virtual void OnTurnStopped(WorldCtl sender) { }

		protected virtual void OnDestroyed() { }

		private StarSystem _location;
	}

	[Serializable]
	public abstract class SpaceObjectData
	{
		protected SpaceObjectData()
		{
			ID = Guid.NewGuid();
			Name = String.Empty;
		}

		protected SpaceObjectData(SpaceObject so)
		{
			ID = so.ID;
			LocationID = so.LocationID;
			Name = so.Name;
		}

		public abstract SpaceObject GetInstanceFromData(WorldContext worldContext);

		public Guid ID;
		public Guid LocationID;
		public String Name;
	}
}