using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.ElectricityConsumerGroup
{
	public class TargetConsumingPowerValueTextController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			var electricityConsumer = SelectedHardpointEquipment.GetComponent<ElectricityConsumer>();

			electricityConsumer.TargetConsumingPowerChanged += OnTargetConsumingPowerChanged;
			UpdateText(electricityConsumer.TargetConsumingPower);
		}

		protected override void OnDisableAction()
		{
			SelectedHardpointEquipment.GetComponent<ElectricityConsumer>().TargetConsumingPowerChanged -=
				OnTargetConsumingPowerChanged;
		}

		private void OnTargetConsumingPowerChanged(ElectricityConsumer electricityConsumer, Int64 value)
		{
			UpdateText(value);
		}

		private void UpdateText(Int64 value)
		{
			GetComponent<Text>().text = Units.GetMegawattsString(value);
		}
	}
}