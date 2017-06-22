using System;
using HabitableZone.Core.SpacecraftStructure.Hardware;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.ElectricityConsumerGroup
{
	[RequireComponent(typeof(Slider))]
	public sealed class TargetConsumingPowerSliderController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			_slider = GetComponent<Slider>();

			var electricityConsumer = SelectedHardpoint.InstalledEquipment.GetComponent<ElectricityConsumer>();

			electricityConsumer.TargetConsumingPowerChanged += OnTargetConsumingPowerChanged;
			SelectedHardpointEquipment.TargetEnabledChanged += OnEquipmentTargetEnabledChanged;

			_slider.minValue = electricityConsumer.MinPower;
			_slider.maxValue = electricityConsumer.MaxPower;

			_slider.onValueChanged.AddListener(OnSliderValueChanged);

			UpdateSlider(electricityConsumer.TargetConsumingPower);
			_slider.interactable = SelectedHardpointEquipment.TargetEnabled;
		}

		protected override void OnDisableAction()
		{
			var electricityConsumer = SelectedHardpoint.InstalledEquipment.GetComponent<ElectricityConsumer>();

			electricityConsumer.TargetConsumingPowerChanged -= OnTargetConsumingPowerChanged;
			SelectedHardpoint.InstalledEquipment.TargetEnabledChanged -= OnEquipmentTargetEnabledChanged;

			_slider.onValueChanged.RemoveAllListeners();
		}

		private void OnTargetConsumingPowerChanged(ElectricityConsumer electricityConsumer, Int64 value)
		{
			UpdateSlider(value);
		}

		private void OnSliderValueChanged(Single value)
		{
			if (!SelectedHardpoint.InstalledEquipment.Enabled) return;

			var electricityConsumer = SelectedHardpoint.InstalledEquipment.GetComponent<ElectricityConsumer>();
			electricityConsumer.TargetConsumingPower = (Int64) value;
		}

		private void OnEquipmentTargetEnabledChanged(Equipment sender, Boolean value)
		{
			Assert.IsTrue(SelectedHardpointEquipment == sender && sender != null);
			_slider.interactable = value;
		}

		private void UpdateSlider(Int64 value)
		{
			if ((Int64) _slider.value != value)
				_slider.value = value;
		}

		private Slider _slider;
	}
}