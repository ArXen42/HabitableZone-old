using System;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	/// <summary>
	///    Defines equipment engagement dependency.
	/// </summary>
	public interface IEquipmentEngagementDependency
	{
		/// <summary>
		///    Occurs when engagement is allowed.
		/// </summary>
		event CEventHandler<IEquipmentEngagementDependency> EngagementAllowed;

		/// <summary>
		///    Occurs when engagement is prohibited. Causes equipment's immediate disengagement.
		/// </summary>
		event CEventHandler<IEquipmentEngagementDependency> EngagementProhibited;

		/// <summary>
		///    Dependent equipment.
		/// </summary>
		Equipment Equipment { get; }

		/// <summary>
		///    Determines whether this dependency allows engagement.
		/// </summary>
		Boolean IsEngagementAllowed { get; }
	}
}