using System;
using HabitableZone.Core.SpacecraftStructure.Hardware;

namespace HabitableZone.Core.SpacecraftStructure
{
	/// <summary>
	///    Represents hardpoint (hull structural part and equipment mount point) of spacecraft.
	/// </summary>
	public sealed partial class Hardpoint
	{
		public Hardpoint(HardpointData data)
		{
			Name = data.Name;

			if (data.InstalledEquipmentData != null)
				_equipmentPendingInstallation = data.InstalledEquipmentData.GetInstanceFromData();
		}

		public HardpointData GetSerializationData()
		{
			return new HardpointData(this);
		}

		/// <summary>
		///    Name of the hardpoint.
		/// </summary>
		public readonly String Name; //TODO: localize
	}

	[Serializable]
	public struct HardpointData
	{
		public HardpointData(Hardpoint hardpoint)
		{
			Name = hardpoint.Name;
			InstalledEquipmentData = hardpoint.IsEquipmentInstalled ? hardpoint.InstalledEquipment.GetSerializationData() : null;
		}

		public String Name;
		public EquipmentData InstalledEquipmentData;

		public Hardpoint GetInstanceFromData()
		{
			return new Hardpoint(this);
		}
	}
}