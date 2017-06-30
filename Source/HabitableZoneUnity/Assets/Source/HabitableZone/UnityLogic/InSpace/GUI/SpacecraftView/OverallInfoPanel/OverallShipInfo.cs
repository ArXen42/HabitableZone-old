using System;
using HabitableZone.Core.SpacecraftStructure;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.OverallInfoPanel
{
	public class OverallShipInfo : MonoBehaviour
	{
		private ElectricitySubsystem SpacecraftElectricitySubsystem => _spacecraftViewController.Spacecraft.ElectricitySubsystem;

		private void OnEnable()
		{
			_spacecraftViewController = GetComponentInParent<SpacecraftViewController>();

			OnMassChanged(null, null);
			OnPowerLevelChanged(null);
			OnAccelerationChanged(null);

			_spacecraftViewController.Spacecraft.MassChanged += OnMassChanged;
			SpacecraftElectricitySubsystem.PowerStateChanged += OnAccelerationChanged;
			SpacecraftElectricitySubsystem.PowerStateChanged += OnPowerLevelChanged;
		}

		private void OnDisable()
		{
			_spacecraftViewController.Spacecraft.MassChanged -= OnMassChanged;
			SpacecraftElectricitySubsystem.PowerStateChanged -= OnAccelerationChanged;
			SpacecraftElectricitySubsystem.PowerStateChanged -= OnPowerLevelChanged;
		}

		private void OnMassChanged(Spacecraft sender, MassChangedEventArgs args)
		{
			_massInfo.text = Math.Round(_spacecraftViewController.Spacecraft.Mass / 1e3f, 1).ToString();
		}

		private void OnAccelerationChanged(ElectricitySubsystem sender) //TODO: implement appropriate event
		{
			_accelerationInfo.text = Math.Round(_spacecraftViewController.Spacecraft.Acceleration, 1).ToString();
		}

		private void OnPowerLevelChanged(ElectricitySubsystem sender)
		{
			Int64 producing = SpacecraftElectricitySubsystem.OverallProducingPower;
			Int64 consuming = SpacecraftElectricitySubsystem.OverallConsumingPower;
			_powerInfo.text = producing > 0
				? (100 * consuming / producing).ToString()
				: 0.ToString();
		}

		[SerializeField] private Text _accelerationInfo;

		[SerializeField] private Text _massInfo;
		[SerializeField] private Text _powerInfo;

		private SpacecraftViewController _spacecraftViewController;
	}
}