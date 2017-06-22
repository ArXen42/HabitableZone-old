using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.ElectricityConsumerGroup
{
	public class RealConsumingPowerSliderController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			_slider = GetComponent<Slider>();

			var electricityConsumer = SelectedHardpoint.InstalledEquipment.GetComponent<ElectricityConsumer>();

			electricityConsumer.ConsumingPowerChanged += OnConsumingPowerChanged;

			_slider.minValue = electricityConsumer.MinPower;
			_slider.maxValue = electricityConsumer.MaxPower;

			UpdateSlider(electricityConsumer.ConsumingPower);
		}

		protected override void OnDisableAction()
		{
			var electricityConsumer = SelectedHardpoint.InstalledEquipment.GetComponent<ElectricityConsumer>();

			electricityConsumer.ConsumingPowerChanged -= OnConsumingPowerChanged;
		}

		private void OnConsumingPowerChanged(ElectricityConsumer sender, PowerValueChangedEventArgs args)
		{
			UpdateSlider(sender.ConsumingPower);
		}


		private void UpdateSlider(Int64 value)
		{
			if ((Int64) _slider.value != value)
				_slider.value = value;
		}

		private Slider _slider;
	}
}