using System;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure
{
	public partial class Hardpoint
	{
		/// <summary>
		///    Owner spacecraft.
		/// </summary>
		public Spacecraft Spacecraft { get; private set; }

		/// <summary>
		///    Returns whether this hardpoint installed in (attached to) some spacecraft.
		/// </summary>
		public Boolean IsAttachedToSpacecraft => Spacecraft != null;

		/// <summary>
		///    Called from Hardpoints.Mount. Do not call from anywhere else!
		/// </summary>
		public void HandleMountToSpacecraft(Spacecraft spacecraft)
		{
			Assert.IsNotNull(spacecraft);
			Assert.IsFalse(IsEquipmentInstalled);

			Spacecraft = spacecraft;

			if (_equipmentPendingInstallation != null)
			{
				InstallEquipment(_equipmentPendingInstallation);
				_equipmentPendingInstallation = null;
			}
		}

		/// <summary>
		///    Called from Hardpoints.Unmount. Do not call from anywhere else!
		/// </summary>
		public void HandleUnmountFromSpacecraft()
		{
			if (IsEquipmentInstalled)
				UninstallEquipment();

			Spacecraft = null;
		}
	}
}