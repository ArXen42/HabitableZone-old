using HabitableZone.Core.SpacecraftStructure;
using HabitableZone.Core.SpacecraftStructure.Hardware;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel
{
	/// <summary>
	///    Предоставляет кэширование выбранного отсека, оборудования и т.д. для элементов вкладок левой панели.
	/// </summary>
	/// <remarks>
	///    При смене выбранного отсека/оборудования в нем позволяет унаследованному компоненту корректно отписаться
	///    от предыдущего выбранного отсека/оборудования, выключиться и при необходимости быть включенным заново (и
	///    инициализироваться новыми значениями).
	/// </remarks>
	public abstract class LeftPanelTabMonoBehaviour : MonoBehaviour
	{
		protected SpacecraftViewController SpacecraftViewController { get; private set; }
		protected Hardpoint SelectedHardpoint { get; private set; }
		protected Equipment SelectedHardpointEquipment { get; private set; }

		protected ElectricitySubsystem SpacecraftElectricitySubsystem => SpacecraftViewController.Spacecraft.ElectricitySubsystem;

		protected abstract void OnEnableAction();
		protected abstract void OnDisableAction();

		private void OnEnable()
		{
			SpacecraftViewController = GetComponentInParent<SpacecraftViewController>();
			SelectedHardpoint = SpacecraftViewController.SelectedHardpoint;
			SelectedHardpointEquipment = SpacecraftViewController.SelectedHardpoint.InstalledEquipment;
			//Как и с SelectedHardpoint, вкладка EquipmentTab будет в будущем перезагружаться при внутренней смене InstalledEquipment.

			OnEnableAction();
		}

		private void OnDisable()
		{
			OnDisableAction();

			SelectedHardpoint = null;
			SelectedHardpointEquipment = null;
		}
	}
}