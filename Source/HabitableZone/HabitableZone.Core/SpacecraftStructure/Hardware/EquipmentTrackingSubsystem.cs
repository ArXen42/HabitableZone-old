using System;
using System.Collections;
using System.Collections.Generic;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	/// <summary>
	///    Tracks equipment (de)installation and provides observable
	///    readonly collection of equipment installed on the spacecraft.
	/// </summary>
	public sealed class EquipmentTrackingSubsystem : IEnumerable<Equipment>
	{
		public EquipmentTrackingSubsystem(Spacecraft spacecraft)
		{
			Spacecraft = spacecraft;
			_installedEquipment = new List<Equipment>();

			Assert.IsTrue(Spacecraft.Hardpoints.Count == 0,
				"Tried to initialize EquipmentTrackingSubsystem after spacecraft initialization." +
				"Don't do that or add appropriate initialization code.");

			Spacecraft.Hardpoints.HardpointMounted += OnHardpointMounted;
			Spacecraft.Hardpoints.HardpointUnmounted += OnHardpointUnmounted;
		}

		public IEnumerator<Equipment> GetEnumerator()
		{
			return _installedEquipment.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///    Occurs when equipment is installed in some hardpoint.
		/// </summary>
		public event SEventHandler<Equipment> EquipmentConnected;

		/// <summary>
		///    Occurs when equipment is uninstalled from some hardpoint.
		/// </summary>
		public event SEventHandler<Equipment> EquipmentDisconnected;

		public Int32 Count => _installedEquipment.Count;

		/// <summary>
		///    Spacecraft this subsystem is attached to.
		/// </summary>
		public readonly Spacecraft Spacecraft;

		private void OnHardpointMounted(Hardpoints sender, Hardpoint hardpoint)
		{
			if (hardpoint.IsEquipmentInstalled)
				OnSomeEquipmentInstalled(hardpoint.InstalledEquipment);

			hardpoint.EquipmentInstalled += OnSomeEquipmentInstalled;
			hardpoint.EquipmentUninstalled += OnSomeEquipmentUninstalled;
		}

		private void OnHardpointUnmounted(Hardpoints sender, Hardpoint hardpoint)
		{
			hardpoint.EquipmentInstalled -= OnSomeEquipmentInstalled;
			hardpoint.EquipmentUninstalled -= OnSomeEquipmentUninstalled;
		}

		private void OnSomeEquipmentInstalled(Equipment equipment)
		{
			_installedEquipment.Add(equipment);
			EquipmentConnected?.Invoke(equipment);
		}

		private void OnSomeEquipmentUninstalled(Equipment equipment)
		{
			_installedEquipment.Remove(equipment);
			EquipmentDisconnected?.Invoke(equipment);
		}

		private readonly List<Equipment> _installedEquipment;
	}
}