using System;
using HabitableZone.Core.Localization;
using HabitableZone.Core.SpacecraftStructure.Hardware;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.OverallInfoGroup
{
	public class EquipmentEnabledElementController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			OnEquipmentTargetEnabledChanged(SelectedHardpointEquipment, SelectedHardpointEquipment.TargetEnabled);
			OnEquipmentEnabledChanged(SelectedHardpointEquipment, SelectedHardpointEquipment.Enabled);

			SelectedHardpointEquipment.TargetEnabledChanged += OnEquipmentTargetEnabledChanged;
			SelectedHardpointEquipment.EnabledChanged += OnEquipmentEnabledChanged;
			_targetEnabledToggle.onValueChanged.AddListener(OnToggleChanged);
		}

		protected override void OnDisableAction()
		{
			SelectedHardpointEquipment.TargetEnabledChanged -= OnEquipmentTargetEnabledChanged;
			SelectedHardpointEquipment.EnabledChanged -= OnEquipmentEnabledChanged;
			_targetEnabledToggle.onValueChanged.RemoveAllListeners();
		}

		private void OnEquipmentTargetEnabledChanged(Equipment sender, Boolean value)
		{
			_targetEnabledToggle.isOn = value;
		}

		private void OnEquipmentEnabledChanged(Equipment sender, Boolean value)
		{
			_realEnabledText.text = value
				? LocalizationManager.GetLocalizationString("UI.Space.SpacecraftView.LeftPanel.EquipmentTab.EquipmentEnabledValue").Value
				: LocalizationManager.GetLocalizationString("UI.Space.SpacecraftView.LeftPanel.EquipmentTab.EquipmentDisabledValue").Value;
		}

		private void OnToggleChanged(Boolean value)
		{
			if (value)
				SelectedHardpointEquipment.RequestEngagement();
			else
				SelectedHardpointEquipment.RequestDisengagement();
		}

		[SerializeField] private Toggle _targetEnabledToggle;
		[SerializeField] private Text _realEnabledText;
	}
}