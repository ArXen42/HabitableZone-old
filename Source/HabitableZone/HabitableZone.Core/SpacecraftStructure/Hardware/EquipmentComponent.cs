using System;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	/// <summary>
	///    Represents part of equipment basical functionality (such as electrical network node).
	/// </summary>
	/// <remarks>
	///    It is not the same thing as Unity3d components. It's just some kind of multiple inheritance alternative.
	/// </remarks>
	public abstract class EquipmentComponent
	{
		protected EquipmentComponent(EquipmentComponentData data, Equipment ownerEquipment)
		{
			OwnerEquipment = ownerEquipment;
			OwnerEquipment.RegisterComponent(this);

			Assert.IsFalse(OwnerEquipment.Enabled);
		}

		/// <summary>
		///    Equipment that component attached to. Can't be changed.
		/// </summary>
		public readonly Equipment OwnerEquipment;
	}

	[Serializable]
	public abstract class EquipmentComponentData
	{
		protected EquipmentComponentData() { }

		protected EquipmentComponentData(EquipmentComponent component) { }
	}
}