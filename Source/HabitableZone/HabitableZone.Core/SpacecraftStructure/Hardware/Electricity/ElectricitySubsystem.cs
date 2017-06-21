using System;
using HabitableZone.Common;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.Electricity
{
	/// <summary>
	///    Carries information about power value change.
 	/// </summary>
	public class PowerValueChangedEventArgs : EventArgs
	{
		public PowerValueChangedEventArgs(Int64 oldValue, Int64 newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public Int64 Delta => NewValue - OldValue;

		public readonly Int64 NewValue;
		public readonly Int64 OldValue;
	}

	/// <summary>
	///    This subsystem controls the power distribution on it's spacecraft.
	/// </summary>
	public sealed class ElectricitySubsystem
	{
		/// <summary>
		///    Constructs new ElectricitySubsystem for given Spacecraft.
		/// </summary>
		public ElectricitySubsystem(Spacecraft spacecraft)
		{
			Spacecraft = spacecraft;
			EquipmentNetwork = new EquipmentNetwork();

			Spacecraft.EquipmentTrackingSubsystem.EquipmentConnected += OnEquipmentConnected;
			Spacecraft.EquipmentTrackingSubsystem.EquipmentDisconnected += OnEquipmentDisconnected;

			EquipmentNetwork.PowerStateChanged += InvokePowerStateChanged;
		}

		/// <summary>
		///    Spacecraft this subsystem belongs to.
		/// </summary>
		public readonly Spacecraft Spacecraft;

		/// <summary>
		///    Actually main part of this subsystem,
		///    contains all equipment with TargetEnabled == true and distributes power between them.
		/// </summary>
		public readonly EquipmentNetwork EquipmentNetwork;

		/// <summary>
		///    Total power production.
		/// </summary>
		public Int64 OverallProducingPower => EquipmentNetwork.LastProducerNode?.OutputPower ?? 0;

		/// <summary>
		///    Total unused power.
		/// </summary>
		public Int64 AvailablePower => EquipmentNetwork.Last?.OutputPower ?? 0;

		/// <summary>
		///    Total power consumption.
		/// </summary>
		public Int64 OverallConsumingPower => OverallProducingPower - AvailablePower;

		/// <summary>
		///    Occurs when power in the electrical network were redistributed.
		/// </summary>
		public event CEventHandler<ElectricitySubsystem> PowerStateChanged;

		private void OnEquipmentConnected(Equipment equipment)
		{
			var producer = equipment.GetComponent<ElectricityProducer>();
			var consumer = equipment.GetComponent<ElectricityConsumer>();

			if (consumer == null && producer == null) return; //We don't care of this equipment

			equipment.TargetEnabledChanged += OnEquipmentTargetEnabledChanged;

			if (consumer != null)
				consumer.PriorityChanged += OnEquipmentPriorityChanged;

			if (equipment.TargetEnabled)
				OnEquipmentTargetEnabledChanged(equipment, true); //Initialization
		}

		private void OnEquipmentDisconnected(Equipment equipment)
		{
			equipment.TargetEnabledChanged -= OnEquipmentTargetEnabledChanged;

			var consumer = equipment.GetComponent<ElectricityConsumer>();
			if (consumer != null)
				consumer.PriorityChanged -= OnEquipmentPriorityChanged;
		}

		private void OnEquipmentTargetEnabledChanged(Equipment sender, Boolean targetEnabled)
		{
			if (targetEnabled)
			{
				Assert.IsFalse(sender.Enabled, "Equipment was enabled but it's TargetEnabled was false.");
				Assert.IsTrue(EquipmentNetwork.Find(sender) == null,
					"Equipment with negative TargetEnabled was connected to EquipmentNetwork.");

				EquipmentNetwork.InsertEquipment(sender);
			}
			else
			{
				Assert.IsFalse(EquipmentNetwork.Find(sender) == null,
					"Equipment with positive TargetEnabled wasn't connected to EquipmentNetwork.");

				EquipmentNetwork.RemoveEquipment(sender);
			}
		}

		private void OnEquipmentPriorityChanged(ElectricityConsumer sender, Int16 priority)
		{
			EquipmentNetwork.RemoveEquipment(sender.OwnerEquipment);
			EquipmentNetwork.InsertEquipment(sender.OwnerEquipment);
		}

		private void InvokePowerStateChanged(EquipmentNetwork sender)
		{
			PowerStateChanged?.Invoke(this);
		}
	}
}