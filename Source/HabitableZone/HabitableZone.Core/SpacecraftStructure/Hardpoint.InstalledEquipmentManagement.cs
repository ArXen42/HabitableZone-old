using System;
using HabitableZone.Common;
using HabitableZone.Core.SpacecraftStructure.Hardware;

namespace HabitableZone.Core.SpacecraftStructure
{
	public partial class Hardpoint
	{
		/// <summary>
		///    Occurs when some equipment is installed into this hardpoint.
		/// </summary>
		public event Action<Equipment> EquipmentInstalled;

		/// <summary>
		///    Occurs when some equipment is uninstalled (detached) from this hardpoint.
		/// </summary>
		public event Action<Equipment> EquipmentUninstalled;

		/// <summary>
		///    Returns currently installed equipment or null if not installed.
		/// </summary>
		public Equipment InstalledEquipment { get; private set; }

		/// <summary>
		///    Is equipment installed.
		/// </summary>
		public Boolean IsEquipmentInstalled => InstalledEquipment != null;

		/// <summary>
		///    Detaches installed equipment from this hardpoint.
		/// </summary>
		public Equipment UninstallEquipment()
		{
			var equipment = InstalledEquipment;

			equipment.MassChanged -= OnInstalledEquipmentMassChanged;

			InstalledEquipment = null;
			equipment.HandleUninstallation();

			RecalculateMass();
			EquipmentUninstalled?.Invoke(equipment);

			return equipment;
		}

		/// <summary>
		///    Installs equipment into this hardpoint.
		///    It should be disabled and detached, previous equipment should be uninstalled.
		/// </summary>
		public Equipment InstallEquipment(Equipment equipment)
		{
			Assert.IsNotNull(equipment, "Tried to install null equipment.");
			Assert.IsFalse(IsEquipmentInstalled, "Some equipment is already installed.");

			InstalledEquipment = equipment;
			InstalledEquipment.HandleInstallation(this);

			InstalledEquipment.MassChanged += OnInstalledEquipmentMassChanged;

			RecalculateMass();
			EquipmentInstalled?.Invoke(InstalledEquipment);

			return InstalledEquipment;
		}

		//Equipment that will be installed right after this hardpoint will be attached to spacecraft.
		private Equipment _equipmentPendingInstallation;
	}
}