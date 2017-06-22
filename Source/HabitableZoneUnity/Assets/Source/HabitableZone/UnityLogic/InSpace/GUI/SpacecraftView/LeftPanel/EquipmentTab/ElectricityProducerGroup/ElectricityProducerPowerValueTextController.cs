using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.ElectricityProducerGroup
{
	[RequireComponent(typeof(Text))]
	public sealed class ElectricityProducerPowerValueTextController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			SelectedHardpointEquipment.GetComponent<ElectricityProducer>().ProducingPowerChanged += OnProducingPowerChanged;
			UpdateValue(SelectedHardpointEquipment.GetComponent<ElectricityProducer>().ProducingPower);
		}

		protected override void OnDisableAction()
		{
			SelectedHardpointEquipment.GetComponent<ElectricityProducer>().ProducingPowerChanged -= OnProducingPowerChanged;
		}

		private void OnProducingPowerChanged(ElectricityProducer sender, PowerValueChangedEventArgs args)
		{
			UpdateValue(sender.ProducingPower);
		}

		private void UpdateValue(Int64 producingPower)
		{
			GetComponent<Text>().text = Units.GetMegawattsString(producingPower);
		}
	}
}