using System;
using HabitableZone.Common;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	public partial class Equipment
	{
		/// <summary>
		///    Occurs when this equipment was installed somewhere.
		/// </summary>
		public event SEventHandler<Equipment, Hardpoint> Installed;

		/// <summary>
		///    Occurs when this equipment was uninstalled from it's hardpoint.
		/// </summary>
		public event SEventHandler<Equipment, Hardpoint> Uninstalled;

		/// <summary>
		///    Owner hardpoint.
		/// </summary>
		public Hardpoint Hardpoint { get; private set; }

		/// <summary>
		///    Simplified access to owner spacecraft.
		/// </summary>
		public Spacecraft Spacecraft => IsInstalled ? Hardpoint.Spacecraft : null;

		/// <summary>
		///    Returns whether is installed in some hardpoint.
		/// </summary>
		public Boolean IsInstalled => Hardpoint != null;

		/// <summary>
		///    Internal method that is called from Hardpoint.InstallEquipment. Do not call from anywhere else!
		/// </summary>
		internal void HandleInstallation(Hardpoint hardpoint)
		{
			Assert.IsFalse(IsInstalled, "Tried to install equipment that is already installed somewhere.");
			Assert.IsFalse(Enabled, "Tried to install enabled equipment.");

			Hardpoint = hardpoint;

			Installed?.Invoke(this, hardpoint);

			if (TargetEnabled)
				TryToEngage();
		}

		/// <summary>
		///    Internal method that is called from Hardpoint.UninstallEquipment. Do not call from anywhere else!
		/// </summary>
		internal void HandleUninstallation()
		{
			var hardpoint = Hardpoint;

			Disengage();
			Hardpoint = null;

			Uninstalled?.Invoke(this, hardpoint);
		}
	}
}