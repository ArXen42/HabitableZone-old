using HabitableZone.Core.Localization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.HardpointTab
{
	public class OverallHardpointInfoController : MonoBehaviour
	{
		private void OnEnable()
		{
			var selectedHardpoint = GetComponentInParent<SpacecraftViewController>().SelectedHardpoint;
			Assert.IsNotNull(selectedHardpoint);

			_hardpointNameElement.text = selectedHardpoint.Name;
			_installedEquipmentNameElement.text = selectedHardpoint.IsEquipmentInstalled
				? selectedHardpoint.InstalledEquipment.Name
				: LocalizationManager.GetLocalizationString("UI.Space.SpacecraftView.LeftPanel.HardpointTab.InstalledEquipmentNullValue").Value;
		}

		[SerializeField] private Text _hardpointNameElement;
		[SerializeField] private Text _installedEquipmentNameElement;
	}
}