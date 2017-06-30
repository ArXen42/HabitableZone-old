using System;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.Electricity
{
	/// <summary>
	///    Represents electricity consuming component.
	/// </summary>
	public sealed class ElectricityConsumer : ElectricityComponent
	{
		public ElectricityConsumer(ElectricityConsumerData data, Equipment ownerEquipment)
			: base(data, ownerEquipment)
		{
			MinPower = data.MinPower;
			OptimalPower = data.OptimalPower;
			MaxPower = data.MaxPower;

			Priority = data.Priority;
			TargetConsumingPower = data.TargetConsumingPower;
		}

		/// <summary>
		///    Occurs when the value of ConsumingPower changes.
		/// </summary>
		public event CEventHandler<ElectricityConsumer, PowerValueChangedEventArgs> ConsumingPowerChanged;

		/// <summary>
		///    Occurs when TargetConsumingPower has changed.
		/// </summary>
		public event SEventHandler<ElectricityConsumer, Int64> TargetConsumingPowerChanged;

		/// <summary>
		///    Occurs when priority has changed.
		/// </summary>
		public event SEventHandler<ElectricityConsumer, Int16> PriorityChanged;

		/// <summary>
		///    Current power consumption.
		/// </summary>
		public Int64 ConsumingPower
		{
			get { return _consumingPower; }
			private set
			{
				Assert.IsTrue(value >= 0, "Consuming can't be less then zero.");
				Assert.IsTrue(OwnerEquipment.Enabled || value == 0,
					"Detected non zero delta of output/input power with disabled equipment.");
				Assert.IsTrue(value >= MinPower || value == 0, "ConsumingPower can't be less then minimal power.");
				Assert.IsTrue(value <= MaxPower, "ConsumingPower can't be greater then maximal power.");

				Int64 oldValue = _consumingPower;
				_consumingPower = value;

				ConsumingPowerChanged?.Invoke(this, new PowerValueChangedEventArgs(oldValue, value));
			}
		}

		/// <summary>
		///    Target consuming power.
		/// </summary>
		public Int64 TargetConsumingPower
		{
			get { return _targetConsumingPower; }
			set
			{
				if (value == _targetConsumingPower) return;

				Assert.IsFalse(value < MinPower);
				Assert.IsFalse(value > MaxPower);

				_targetConsumingPower = value;
				TargetConsumingPowerChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		///    Priority of this electricity consumer.
		/// </summary>
		public Int16 Priority
		{
			get { return _priority; }
			set
			{
				Assert.IsTrue(value >= 0, "Negative priority is not allowed.");

				if (value == _priority) return;

				_priority = value;
				PriorityChanged?.Invoke(this, value);
			}
		}

		public ElectricityConsumerData GetSerializationData()
		{
			return new ElectricityConsumerData(this);
		}

		public readonly Int64 MinPower, OptimalPower, MaxPower;

		protected override void OnNodePowerConfigurationChanged(EquipmentNetwork.Node sender)
		{
			ConsumingPower = sender.InputPower - sender.OutputPower;
		}

		private Int64 _consumingPower;
		private Int16 _priority;
		private Int64 _targetConsumingPower;
	}

	[Serializable]
	public class ElectricityConsumerData : EquipmentComponentData
	{
		public ElectricityConsumerData() { }

		public ElectricityConsumerData(ElectricityConsumer component) : base(component)
		{
			MinPower = component.MinPower;
			OptimalPower = component.OptimalPower;
			MaxPower = component.MaxPower;

			Priority = component.Priority;
			TargetConsumingPower = component.TargetConsumingPower;
		}

		public Int64 MinPower, OptimalPower, MaxPower, TargetConsumingPower;
		public Int16 Priority;
	}
}