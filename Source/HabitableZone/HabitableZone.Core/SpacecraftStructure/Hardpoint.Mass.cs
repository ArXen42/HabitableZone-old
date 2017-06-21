using System;
using HabitableZone.Common;
using HabitableZone.Core.SpacecraftStructure.Hardware;

namespace HabitableZone.Core.SpacecraftStructure
{
	public partial class Hardpoint
	{
		/// <summary>
		///    Mass of the hardpoint.
		/// </summary>
		public Single Mass
		{
			get { return _mass; }
			private set
			{
				Single oldValue = _mass;

				if (oldValue == value) return;

				_mass = value;
				MassChanged?.Invoke(this, new MassChangedEventArgs(oldValue, value));
			}
		}

		/// <summary>
		///    Occurs when mass of the hardpoint is changed.
		/// </summary>
		public event CEventHandler<Hardpoint, MassChangedEventArgs> MassChanged;

		private void RecalculateMass()
		{
			Mass = IsEquipmentInstalled
				? InstalledEquipment.Mass
				: 0;
		}

		private void OnInstalledEquipmentMassChanged(Equipment sender, MassChangedEventArgs args)
		{
			RecalculateMass(); //TODO: Use args.Delta
		}

		private Single _mass;
	}
}
