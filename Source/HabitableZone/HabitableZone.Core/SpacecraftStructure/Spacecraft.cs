using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HabitableZone.Common;
using HabitableZone.Core.SpacecraftStructure.Hardware;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using HabitableZone.Core.SpacecraftStructure.Hardware.EquipmentTypes;
using HabitableZone.Core.World;

namespace HabitableZone.Core.SpacecraftStructure
{
	/// <summary>
	///    Represents internal structure of a spacecraft. Base class for objects like ships, orbital bases and so on.
	/// </summary>
	public abstract partial class Spacecraft : SpaceObject
	{
		protected Spacecraft(WorldContext worldContext, SpacecraftData data) : base(worldContext, data)
		{
			Hardpoints = new Hardpoints(this);
			Hardpoints.HardpointMounted += OnHardpointMounted;
			Hardpoints.HardpointUnmounted += OnHardpointUnmounted;

			EquipmentTrackingSubsystem = new EquipmentTrackingSubsystem(this);
			ElectricitySubsystem = new ElectricitySubsystem(this);

			foreach (var hardpointData in data.HardpointsData)
				Hardpoints.Mount(hardpointData.GetInstanceFromData());
		}

		/// <summary>
		///    Serializes spacecraft to given stream using project's json settings.
		/// </summary>
		public void SerializeTo(Stream stream)
		{
			Serialization.SerializeDataToJson(GetSerializationData(), stream);
		}

		/// <summary>
		///    Deserializes spacecraft from given steam using project's json settings.
		/// </summary>
		public static Spacecraft DeserializeFrom(Stream stream, WorldContext worldContext)
		{
			var data = Serialization.DeserializeDataFromJson<SpacecraftData>(stream);
			return (Spacecraft) data.GetInstanceFromData(worldContext);
		}

		/// <summary>
		///    Provides access to basic electricity subsystem.
		/// </summary>
		public readonly ElectricitySubsystem ElectricitySubsystem;

		/// <summary>
		///    Provides access to EquipmentTrackingSubsystem which is useful if observable list of equipment is needed.
		/// </summary>
		public readonly EquipmentTrackingSubsystem EquipmentTrackingSubsystem;

		/// <summary>
		///    Collection of this spacecraft's hardpoints.
		/// </summary>
		public readonly Hardpoints Hardpoints;

		/// <summary>
		///    Collection of all equipment on this spacecraft.
		/// </summary>
		public List<Equipment> AllEquipment
		{
			get
			{
				return Hardpoints.Where(hardpoint => hardpoint.IsEquipmentInstalled)
					.Select(hardpoint => hardpoint.InstalledEquipment)
					.ToList();
			}
		}

		/// <summary>
		///    Returns first found hyperdrive on this spacecraft or null if it doesn't installed.
		/// </summary>
		public Hyperdrive Hyperdrive => AllEquipment.Find(x => x is Hyperdrive) as Hyperdrive;

		/// <summary>
		///    Calculates acceleration of this spacecraft.
		/// </summary>
		public Single Acceleration => GetAllEquipment<EngineInlet>().Sum(inlet => inlet.Acceleration);

		/// <summary>
		///    Returns all equipment of specified type in this spacecraft.
		/// </summary>
		public List<TEquipmentType> GetAllEquipment<TEquipmentType>() where TEquipmentType : Equipment
		{
			return Hardpoints
				.Select(hardpoint => hardpoint.InstalledEquipment as TEquipmentType)
				.Where(equipment => equipment != null)
				.ToList();
		}

		private List<Hardpoint> _hardpoints;
	}

	[Serializable]
	public abstract class SpacecraftData : SpaceObjectData
	{
		protected SpacecraftData() { }

		protected SpacecraftData(Spacecraft spacecraft) : base(spacecraft)
		{
			HardpointsData = spacecraft.Hardpoints.Select(hp => hp.GetSerializationData()).ToArray();
		}

		public HardpointData[] HardpointsData;
	}
}