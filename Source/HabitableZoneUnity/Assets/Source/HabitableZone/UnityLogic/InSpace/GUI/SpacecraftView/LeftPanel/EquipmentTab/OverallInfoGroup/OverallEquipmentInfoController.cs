using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.EquipmentTab.OverallInfoGroup
{
	public class OverallEquipmentInfoController : LeftPanelTabMonoBehaviour
	{
		protected override void OnEnableAction()
		{
			var equipment = GetComponentInParent<SpacecraftViewController>().SelectedHardpoint.InstalledEquipment;

			_equipmentNameText.text = equipment.Name;
			_equipmentMassText.text = (equipment.Mass / 1e3).ToString(CultureInfo.CurrentCulture);
		}


		protected override void OnDisableAction() { }
		[SerializeField] private Text _equipmentMassText;

		[SerializeField] private Text _equipmentNameText;
	}
}