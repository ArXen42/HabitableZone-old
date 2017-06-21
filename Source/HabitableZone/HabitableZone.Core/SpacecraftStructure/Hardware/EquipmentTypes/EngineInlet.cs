using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.EquipmentTypes
{
	/// <summary>
	///    Represents an engine inlet.
	/// </summary>
	public class EngineInlet : Equipment
	{
		/// <summary>
		///    Global acceleration modifier.
		/// </summary>
		public static Single AccelerationFactor = 3e-3f;

		public EngineInlet(EngineInletData data) : base(data)
		{
			ElectricityConsumer = new ElectricityConsumer(data.ElectricityConsumerData, this);
		}

		public override EquipmentData GetSerializationData()
		{
			return new EngineInletData(this);
		}

		public Single Acceleration
		{
			get { return ElectricityConsumer.ConsumingPower * AccelerationFactor / Mass; } //TODO: rework
		}

		public readonly ElectricityConsumer ElectricityConsumer;
	}

	public class EngineInletData : EquipmentData
	{
		public EngineInletData() { }

		public EngineInletData(EngineInlet engineInlet) : base(engineInlet)
		{
			ElectricityConsumerData = engineInlet.ElectricityConsumer.GetSerializationData();
		}

		public override Equipment GetInstanceFromData()
		{
			return new EngineInlet(this);
		}

		public ElectricityConsumerData ElectricityConsumerData;
	}
}