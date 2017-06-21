using System;
using HabitableZone.Common;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.Electricity
{
	/// <summary>
	///    Represents electricity generating component.
	/// </summary>
	public sealed class ElectricityProducer : ElectricityComponent
	{
		public ElectricityProducer(ElectricityProducerData data, Equipment ownerEquipment)
			: base(data, ownerEquipment)
		{
			MinPower = data.MinPower;
			OptimalPower = data.OptimalPower;
			MaxPower = data.MaxPower;

			TargetProducingPower = data.TargetProducingPower;
		}

		public ElectricityProducerData GetSerializationData()
		{
			return new ElectricityProducerData(this);
		}

		public readonly Int64 MinPower, OptimalPower, MaxPower;

		/// <summary>
		///    Currently generating power.
		/// </summary>
		public Int64 ProducingPower
		{
			get { return _producingPower; }
			private set
			{
				Assert.IsTrue(value >= 0, "ProducingPower can't be less then zero.");
				Assert.IsTrue(OwnerEquipment.Enabled || value == 0,
					"Detected non zero delta of output/input power with disabled equipment.");
				Assert.IsTrue(value >= MinPower || value == 0, "ProducingPower can't be less then minimal power.");
				Assert.IsTrue(value <= MaxPower, "ProducingPower can't be greater then maximal power.");

				Int64 oldValue = _producingPower;
				_producingPower = value;

				ProducingPowerChanged?.Invoke(this, new PowerValueChangedEventArgs(oldValue, value));
			}
		}

		/// <summary>
		///    Target producing power.
		/// </summary>
		public Int64 TargetProducingPower
		{
			get { return _targetProducingPower; }
			set
			{
				if (value == _targetProducingPower) return;

				_targetProducingPower = value;

				TargetProducingPowerChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		///    Occurs when the value of ProducingPower changes.
		/// </summary>
		public event CEventHandler<ElectricityProducer, PowerValueChangedEventArgs> ProducingPowerChanged;

		/// <summary>
		///    Occurs when TargetProducingPower has changed.
		/// </summary>
		public event SEventHandler<ElectricityProducer, Int64> TargetProducingPowerChanged;

		protected override void OnNodePowerConfigurationChanged(EquipmentNetwork.Node sender)
		{
			ProducingPower = sender.OutputPower - sender.InputPower;
			Assert.IsTrue(ProducingPower >= 0);
		}

		private Int64 _targetProducingPower;
		private Int64 _producingPower;
	}

	[Serializable]
	public class ElectricityProducerData : EquipmentComponentData
	{
		public ElectricityProducerData() { }

		public ElectricityProducerData(ElectricityProducer component) : base(component)
		{
			MinPower = component.MinPower;
			OptimalPower = component.OptimalPower;
			MaxPower = component.MaxPower;

			TargetProducingPower = component.ProducingPower;
		}

		public Int64 MinPower, OptimalPower, MaxPower, TargetProducingPower;
	}
}