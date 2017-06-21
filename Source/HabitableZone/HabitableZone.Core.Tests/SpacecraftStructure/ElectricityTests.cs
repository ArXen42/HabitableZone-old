using System;
using NUnit.Framework;
using HabitableZone.Core.SpacecraftStructure.Hardware.EquipmentTypes;

namespace HabitableZone.Core.Tests.SpacecraftStructure
{
	/// <summary>
	///    Contains tests for electricity-related things.
	/// </summary>
	public class ElectricityTests
	{
		[Test]
		public void EquipmentNetwork_IsInitialStringCorrect()
		{
			Assert.AreEqual(
				SpacecraftStructureHelper.GetTestSpacecraft().ElectricitySubsystem.EquipmentNetwork.ToString(),
				InitialEquipmentNetworkString);
		}

		[TestCase("Reactor1", ExpectedResult = "0 - EngineInlet1 - 0 - EngineInlet2 - 0")]
		[TestCase("EngineInlet1", ExpectedResult = "0 - Reactor1 - 120000000 - EngineInlet2 - 70000000")]
		[TestCase("EngineInlet2", ExpectedResult = "0 - Reactor1 - 120000000 - EngineInlet1 - 55000000")]
		public String EquipmentNetwork_EquipmentDisengagement_ProperNodeRemoval(String equipmentName)
		{
			var spacecraft = SpacecraftStructureHelper.GetTestSpacecraft();
			var equipment = spacecraft.AllEquipment.Find(eq => eq.Name == equipmentName);

			equipment.RequestDisengagement();

			return spacecraft.ElectricitySubsystem.EquipmentNetwork.ToString();
		}

		[TestCase("Hyperdrive")]
		[TestCase("Reactor1")]
		[TestCase("EngineInlet1")]
		[TestCase("EngineInlet2")]
		public void EquipmentNetwork_DisableAndEnableEquipment_DoesNoChanges(String equipmentName)
		{
			var spacecraft = SpacecraftStructureHelper.GetTestSpacecraft();
			var equipment = spacecraft.AllEquipment.Find(eq => eq.Name == equipmentName);

			equipment.RequestDisengagement();
			equipment.RequestEngagement();

			String equipmentNetworkString = spacecraft.ElectricitySubsystem.EquipmentNetwork.ToString();
			Assert.AreEqual(equipmentNetworkString, InitialEquipmentNetworkString);
		}

		[Test]
		public void EquipmentNetwork_ChangePriority_ProperChainChange()
		{
			var spacecraft = SpacecraftStructureHelper.GetTestSpacecraft();
			var equipmentNetwork = spacecraft.ElectricitySubsystem.EquipmentNetwork;
			var engineInlet1 = spacecraft.GetAllEquipment<EngineInlet>().Find(eq => eq.Name == "EngineInlet1");
			var engineInlet2 = spacecraft.GetAllEquipment<EngineInlet>().Find(eq => eq.Name == "EngineInlet2");

			engineInlet2.ElectricityConsumer.Priority = 9;
			Assert.AreEqual(equipmentNetwork.ToString(), InitialEquipmentNetworkString, "Shouldn't have changed");

			engineInlet2.ElectricityConsumer.Priority = 1;
			Assert.AreEqual(equipmentNetwork.ToString(), InitialEquipmentNetworkString, "Shouldn't have changed");

			engineInlet1.ElectricityConsumer.Priority = 2;
			Assert.AreEqual(equipmentNetwork.ToString(),
				"0 - Reactor1 - 120000000 - EngineInlet2 - 70000000 - EngineInlet1 - 5000000");
		}

		[Test]
		public void EquipmentNetwork_DisableReactor_OverallPowerZero()
		{
			var spacecraft = SpacecraftStructureHelper.GetTestSpacecraft();
			var reactor = spacecraft.GetAllEquipment<Reactor>()[0];

			reactor.RequestDisengagement();

			Assert.IsTrue(spacecraft.ElectricitySubsystem.OverallProducingPower == 0);
			Assert.IsTrue(spacecraft.ElectricitySubsystem.OverallConsumingPower == 0);
			Assert.IsTrue(spacecraft.ElectricitySubsystem.AvailablePower == 0);
		}

		[TestCase(120000000, ExpectedResult = InitialEquipmentNetworkString)] //No changes expected
		[TestCase(115000000, ExpectedResult = "0 - Reactor1 - 115000000 - EngineInlet1 - 50000000 - EngineInlet2 - 0")] //Should be enough for everyone
		[TestCase(114000000, ExpectedResult = "0 - Reactor1 - 114000000 - EngineInlet1 - 49000000 - EngineInlet2 - 49000000")] //Second engine should be disabled
		[TestCase(65000000, ExpectedResult = "0 - Reactor1 - 65000000 - EngineInlet1 - 0 - EngineInlet2 - 0")] //First still should be enabled at full power
		[TestCase(60000000, ExpectedResult = "0 - Reactor1 - 60000000 - EngineInlet1 - 0 - EngineInlet2 - 0")] //First still should be enabled, but at 60MWt
		[TestCase(50000000, ExpectedResult = "0 - Reactor1 - 50000000 - EngineInlet1 - 0 - EngineInlet2 - 0")] //First still should be enabled, but at MinPower
		public String EquipmentNetwork_ChangeReactorTargetProducingPower_CheckChanges(Int64 power)
		{
			var spacecraft = SpacecraftStructureHelper.GetTestSpacecraft();
			var reactor = spacecraft.GetAllEquipment<Reactor>()[0];

			reactor.ElectricityProducer.TargetProducingPower = power;

			return spacecraft.ElectricitySubsystem.EquipmentNetwork.ToString();
		}

		[TestCase(65000000)]
		[TestCase(60000000)]
		[TestCase(55000000)]
		[TestCase(50000000)]
		public void EquipmentNetwork_ChangePrioritizedConsumerConsumingPower_ProperChangesToFollowing(Int64 engineInlet1ConsumingPower)
		{
			var spacecraft = SpacecraftStructureHelper.GetTestSpacecraft();
			var reactor = spacecraft.GetAllEquipment<Reactor>()[0];

			var engineInlet1 = spacecraft.GetAllEquipment<EngineInlet>().Find(eq => eq.Name == "EngineInlet1");
			var engineInlet2 = spacecraft.GetAllEquipment<EngineInlet>().Find(eq => eq.Name == "EngineInlet2");

			const Int64 reactorPower = 110000000;
			const Int64 engineInlet2TargetPower = 55000000;

			reactor.ElectricityProducer.TargetProducingPower = reactorPower;
			engineInlet2.ElectricityConsumer.TargetConsumingPower = engineInlet2TargetPower; //To check more variants

			engineInlet1.ElectricityConsumer.TargetConsumingPower = engineInlet1ConsumingPower;
			Assert.AreEqual(engineInlet1.ElectricityConsumer.ConsumingPower, engineInlet1ConsumingPower, "In this test they should be equal");

			Int64 minPower = engineInlet2.ElectricityConsumer.MinPower;
			Int64 delta = reactorPower - engineInlet1ConsumingPower;
			Int64 minCheckedDelta = delta >= minPower ? delta : 0;
			Int64 expectedEngineInlet2Power = minCheckedDelta <= engineInlet2TargetPower ? minCheckedDelta : engineInlet2TargetPower;
			Assert.AreEqual(engineInlet2.ElectricityConsumer.ConsumingPower, expectedEngineInlet2Power);
		}

		[Test]
		public void Dependencies_NonElectricalEquipment_EngagesAndDisengagesProperly()
		{
			var hyperdrive = SpacecraftStructureHelper.GetTestSpacecraft().Hyperdrive;

			Assert.IsTrue(hyperdrive.Enabled); //Might fail if external ingibitor would appear, then change

			hyperdrive.RequestDisengagement();

			Assert.IsFalse(hyperdrive.Enabled);
		}

		/// <summary>
		///    This is what returns from EquipmentNetwork.ToString on the fresh test spacecraft.
		/// </summary>
		private const String InitialEquipmentNetworkString =
			"0 - Reactor1 - 120000000 - EngineInlet1 - 55000000 - EngineInlet2 - 5000000";
	}
}