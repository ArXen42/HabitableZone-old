using System;
using HabitableZone.UnityLogic.InSpace.GUI.CustomElements.TabbedLayout;
using UnityEngine;
using UnityEngine.Assertions;
using Object = System.Object;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel
{
	[RequireComponent(typeof(TabbedLayoutController))]
	public class LeftPanelTabsController : MonoBehaviour
	{
		private void OnEnable()
		{
			_tabbedLayoutController = GetComponent<TabbedLayoutController>();
			Assert.IsNotNull(_tabbedLayoutController);

			_spacecraftViewController = GetComponentInParent<SpacecraftViewController>();

			_spacecraftViewController.SelectionChanged += OnSpacecraftViewSelectionChanged;
			OnSpacecraftViewSelectionChanged(this, EventArgs.Empty);
		}

		private void OnDisable()
		{
			_spacecraftViewController.SelectionChanged -= OnSpacecraftViewSelectionChanged;
		}

		private void OnSpacecraftViewSelectionChanged(Object sender, EventArgs eventArgs)
		{
			var equipmentTab = _tabbedLayoutController.Tabs[1];

			Boolean isEquipmentInstalled = _spacecraftViewController.AnyHardpointSelected &&
			                               _spacecraftViewController.SelectedHardpoint.IsEquipmentInstalled;

			equipmentTab.Acessible = isEquipmentInstalled;

			_tabbedLayoutController.ReEnableActiveTab();
			//Позволяет не заботиться о подписке на смену фокуса в контроллерах элементов.
			//Если здесь происходит ошибка, значит сбился порядок инициализации скриптов
			//и этот OnEnable вызвался раньше OnEnable контроллера вкладок.
		}

		private TabbedLayoutController _tabbedLayoutController;
		private SpacecraftViewController _spacecraftViewController;
	}
}