using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HabitableZone.Core.World.Universe
{
	/// <summary>
	///    Provides a bunch of stars. Universe.
	/// </summary>
	public class StarSystems : IWorldContextProvider
	{
		public StarSystems(WorldContext worldContext, StarSystemsData data)
		{
			WorldContext = worldContext;
			_starSystemsDictionary = new Dictionary<Guid, StarSystem>();

			VoidID = Guid.Empty;
		}

		public WorldContext WorldContext { get; }

		/// <summary>
		///    Пустая система, служащая временным буфером.
		/// </summary>
		public StarSystem Void { get; private set; }

		public IEnumerable<StarSystem> All => _starSystemsDictionary.Values.Except(new[] {Void});

		public StarSystemsData GetSerializationData()
		{
			return new StarSystemsData(this);
		}

		/// <summary>
		///    Возвращает звездную cистему с заданным ID.
		/// </summary>
		/// <param name="starSystemID">ID системы.</param>
		/// <returns>Звездная система с заданным ID.</returns>
		public StarSystem ByID(Guid starSystemID)
		{
			return _starSystemsDictionary[starSystemID];
		}

		/// <summary>
		///    Добавляет новую ЗС в словарь.
		/// </summary>
		/// <param name="starSystem">Звездная система.</param>
		public void Add(StarSystem starSystem)
		{
			_starSystemsDictionary.Add(starSystem.ID, starSystem);
		}

		/// <summary>
		///    Пустая система, служащая временным буфером.
		/// </summary>
		public readonly Guid VoidID;

		/// <summary>
		///    Called immediately after constructor. Separated because initialization of WorldContext fields is required.
		/// </summary>
		internal void InitializeFromData(StarSystemsData data)
		{
			if (Void != null) throw new InvalidOperationException("Already initialized.");

			Void = new StarSystem(WorldContext, new StarSystemData {ID = VoidID, UniverseMapPosition = Vector2.zero});

			foreach (var starSystemData in data.StarSystems)
				starSystemData.GetInstanceFromData(WorldContext);
		}

		private readonly Dictionary<Guid, StarSystem> _starSystemsDictionary;
	}

	[Serializable]
	public struct StarSystemsData
	{
		public StarSystemsData(StarSystems starSystems)
		{
			StarSystems = starSystems.All.Select(s => s.GetSerializationData()).ToArray();
		}

		public StarSystems GetInstanceFromData(WorldContext worldContext)
		{
			return new StarSystems(worldContext, this);
		}

		public StarSystemData[] StarSystems;
	}
}