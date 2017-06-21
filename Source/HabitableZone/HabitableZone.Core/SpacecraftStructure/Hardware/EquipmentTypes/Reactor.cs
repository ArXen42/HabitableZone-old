using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.EquipmentTypes
{
	/// <summary>
	///    Represents main power source on most of spacecrafts.
	/// </summary>
	public class Reactor : Equipment
	{
		public Reactor(ReactorData data) : base(data)
		{
			ElectricityProducer = new ElectricityProducer(data.ElectricityProducerData, this);
		}

		public override EquipmentData GetSerializationData()
		{
			return new ReactorData(this);
		}

		public readonly ElectricityProducer ElectricityProducer;
	}

	public class ReactorData : EquipmentData
	{
		public ReactorData() { }

		public ReactorData(Reactor reactor) : base(reactor)
		{
			ElectricityProducerData = reactor.ElectricityProducer.GetSerializationData();
		}

		public override Equipment GetInstanceFromData()
		{
			return new Reactor(this);
		}

		public ElectricityProducerData ElectricityProducerData;
	}
}