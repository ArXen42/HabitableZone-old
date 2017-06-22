using System;
using HabitableZone.Core.SpacecraftStructure.Hardware;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.ElectricityProducerGroup
{
	[RequireComponent(typeof(Slider))]
	public sealed class ElectricityProducerPowerSliderController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			_slider = GetComponent<Slider>();

			var electricityProducer = SelectedHardpointEquipment.GetComponent<ElectricityProducer>();

			electricityProducer.ProducingPowerChanged += OnProducingPowerChanged;
			SelectedHardpointEquipment.EnabledChanged += OnEquipmentEnabledChanged;

			_slider.minValue = electricityProducer.MinPower;
			_slider.maxValue = electricityProducer.MaxPower;

			_slider.onValueChanged.AddListener(OnSliderValueChanged);

			OnProducingPowerChanged(null, null);
			OnEquipmentEnabledChanged(SelectedHardpointEquipment, SelectedHardpointEquipment.Enabled);
		}

		protected override void OnDisableAction()
		{
			var electricityProducer = SelectedHardpointEquipment.GetComponent<ElectricityProducer>();

			electricityProducer.ProducingPowerChanged -= OnProducingPowerChanged;
			SelectedHardpointEquipment.EnabledChanged -= OnEquipmentEnabledChanged;

			_slider.onValueChanged.RemoveAllListeners();
		}

		private void OnProducingPowerChanged(ElectricityProducer sender, PowerValueChangedEventArgs args)
		{
			Int64 value = SelectedHardpointEquipment.GetComponent<ElectricityProducer>().ProducingPower;

			if ((Int64) _slider.value != value)
				_slider.value = value;
		}

		private void OnSliderValueChanged(Single value)
		{
			if (!SelectedHardpointEquipment.Enabled) return;
			//Changing TargetEnabled to false leads slider to move at minimal position.
			//Without this check it will override TargetProducingPower to MinPower. We don't need that.

			var electricityProducer = SelectedHardpointEquipment.GetComponent<ElectricityProducer>();
			electricityProducer.TargetProducingPower = (Int64) value;
		}

		private void OnEquipmentEnabledChanged(Equipment sender, Boolean newValue)
		{
			Assert.IsTrue(SelectedHardpointEquipment == sender && sender != null);
			_slider.interactable = newValue;
		}

		private Slider _slider;
	}
}