using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.LeftPanel.HardpointTab
{
	public class HardpointTabController : MonoBehaviour
	{
		private void OnEnable()
		{
			var spacecraftViewController = GetComponentInParent<SpacecraftViewController>();
			_hardpointNotInstalledStub.SetActive(!spacecraftViewController.AnyHardpointSelected);
			_overallInfoGroup.SetActive(spacecraftViewController.AnyHardpointSelected);
		}

		private void OnDisable()
		{
			_hardpointNotInstalledStub.SetActive(false);
			_overallInfoGroup.SetActive(false);
		}

		[SerializeField] private GameObject _hardpointNotInstalledStub;
		[SerializeField] private GameObject _overallInfoGroup;
	}
}