using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.ElectricityConsumerGroup
{
	[RequireComponent(typeof(Text))]
	public sealed class RealConsumingPowerValueTextController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			var electricityConsumer = SelectedHardpointEquipment.GetComponent<ElectricityConsumer>();

			electricityConsumer.ConsumingPowerChanged += OnConsumingPowerChanged;
			UpdateText(electricityConsumer.ConsumingPower);
		}

		protected override void OnDisableAction()
		{
			SelectedHardpointEquipment.GetComponent<ElectricityConsumer>().ConsumingPowerChanged -= OnConsumingPowerChanged;
		}

		private void OnConsumingPowerChanged(ElectricityConsumer sender, PowerValueChangedEventArgs args)
		{
			UpdateText(sender.ConsumingPower);
		}

		private void UpdateText(Int64 value)
		{
			GetComponent<Text>().text = Units.GetMegawattsString(value);
		}
	}
}
