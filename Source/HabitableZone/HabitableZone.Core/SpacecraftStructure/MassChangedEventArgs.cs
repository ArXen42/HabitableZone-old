using System;

namespace HabitableZone.Core.SpacecraftStructure
{
	/// <summary>
	///    Holds information about someone's mass change.
	/// </summary>
	public class MassChangedEventArgs : EventArgs
	{
		public MassChangedEventArgs(Single previousMass, Single newMass)
		{
			PreviousMass = previousMass;
			NewMass = newMass;
			Delta = newMass - previousMass;
		}

		public readonly Single Delta;
		public readonly Single NewMass;

		public readonly Single PreviousMass;
	}
}