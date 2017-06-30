using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.Assertions;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab
{
	public class EquipmentTabController : MonoBehaviour
	{
		private void OnEnable()
		{
			var spacecraftViewController = GetComponentInParent<SpacecraftViewController>();
			Assert.IsNotNull(spacecraftViewController.SelectedHardpoint);

			var installedEquipment = spacecraftViewController.SelectedHardpoint.InstalledEquipment;

			Boolean isEquipmentInstalled = installedEquipment != null;
			Boolean isProducer = isEquipmentInstalled && installedEquipment.GetComponent<ElectricityProducer>() != null;
			Boolean isConsumer = isEquipmentInstalled && installedEquipment.GetComponent<ElectricityConsumer>() != null;

			_overallInfoGroup.SetActive(true);
			_producerComponentGroup.SetActive(isProducer);
			_consumerComponentGroup.SetActive(isConsumer);
		}

		private void OnDisable()
		{
			_overallInfoGroup.SetActive(false);
			_producerComponentGroup.SetActive(false);
			_consumerComponentGroup.SetActive(false);
		}

		[SerializeField] private GameObject _consumerComponentGroup;

		[SerializeField] private GameObject _overallInfoGroup;
		[SerializeField] private GameObject _producerComponentGroup;
	}
}