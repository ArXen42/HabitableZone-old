using System;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	public partial class Equipment
	{
		/// <summary>
		///    Occurs when mass of the equipment is changed.
		/// </summary>
		public event Action<Equipment, MassChangedEventArgs> MassChanged;

		/// <summary>
		///    Full mass of equipment.
		/// </summary>
		public Single Mass => DryMass;

		/// <summary>
		///    Mass of the inactive (disabled) equipment.
		/// </summary>
		/// <remarks>
		///    As for now, equal to full mass. This behaviour will be of course changed for fuel tank, cargo hold, etc.
		/// </remarks>
		public readonly Single DryMass;

		//TODO: add ways to change mass when it will be needed
	}
}