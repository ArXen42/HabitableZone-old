﻿using System;
using HabitableZone.Core.ShipLogic;

namespace HabitableZone.Core.World.Society
{
	/// <summary>
	///    Represents game character. For now - a really simple character.
	/// </summary>
	public sealed class Captain : IWorldContextProvider
	{
		public Captain(WorldContext worldContext, CaptainData data)
		{
			WorldContext = worldContext;
			CurrentShip = data.GetShip(worldContext);
		}

		public WorldContext WorldContext { get; }

		/// <summary>
		///    Ship in which this Captain currently is.
		/// </summary>
		public Ship CurrentShip { get; set; }

		public CaptainData GetSerializationData()
		{
			return new CaptainData(this);
		}
	}

	[Serializable]
	public struct CaptainData
	{
		public CaptainData(Captain captain)
		{
			CurrentShipID = captain.CurrentShip.ID;
		}

		public Captain GetInstanceFromData(WorldContext worldContext)
		{
			return new Captain(worldContext, this);
		}

		public Guid CurrentShipID;

		public Ship GetShip(WorldContext worldContext)
		{
			return (Ship) worldContext.SpaceObjects.ByID(CurrentShipID);
		}

		public void SetShip(Ship ship)
		{
			CurrentShipID = ship.ID;
		}
	}
}