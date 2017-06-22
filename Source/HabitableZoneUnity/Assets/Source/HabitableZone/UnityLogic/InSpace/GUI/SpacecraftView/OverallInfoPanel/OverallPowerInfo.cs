using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.OverallInfoPanel
{
	/// <summary>
	///    Displays/updates overall power information on corresponding panel.
	/// </summary>
	public class OverallPowerInfo : MonoBehaviour
	{
		private ElectricitySubsystem SpacecraftElectricitySubsystem => _spacecraftViewController.Spacecraft.ElectricitySubsystem;

		private void OnEnable()
		{
			_spacecraftViewController = GetComponentInParent<SpacecraftViewController>();
			RefreshAllValues();

			SpacecraftElectricitySubsystem.PowerStateChanged += OnPowerStateChanged;
		}

		private void OnDisable()
		{
			SpacecraftElectricitySubsystem.PowerStateChanged -= OnPowerStateChanged;
		}

		private void OnPowerStateChanged(ElectricitySubsystem sender)
		{
			_generatingPowerElement.text =
				Units.GetMegawattsString(SpacecraftElectricitySubsystem.OverallProducingPower);
			_consumingPowerElement.text =
				Units.GetMegawattsString(SpacecraftElectricitySubsystem.OverallConsumingPower);
			_availablePowerElement.text =
				Units.GetMegawattsString(SpacecraftElectricitySubsystem.AvailablePower);
		}

		private void RefreshAllValues()
		{
			_generatingPowerElement.text =
				Units.GetMegawattsString(SpacecraftElectricitySubsystem.OverallProducingPower);
			_consumingPowerElement.text =
				Units.GetMegawattsString(SpacecraftElectricitySubsystem.OverallConsumingPower);
			_availablePowerElement.text =
				Units.GetMegawattsString(SpacecraftElectricitySubsystem.AvailablePower);
		}

		[SerializeField] private Text _availablePowerElement;
		[SerializeField] private Text _consumingPowerElement;
		[SerializeField] private Text _generatingPowerElement;

		private SpacecraftViewController _spacecraftViewController;
	}
}