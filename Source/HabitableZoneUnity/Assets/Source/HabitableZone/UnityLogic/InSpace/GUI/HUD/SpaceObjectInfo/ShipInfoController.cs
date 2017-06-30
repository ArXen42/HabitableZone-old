using HabitableZone.Core.ShipLogic;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.HUD.SpaceObjectInfo
{
	public class ShipInfoController : MonoBehaviour
	{
		private void Awake()
		{
			GetComponentInParent<SpaceObjectInfoController>().WatcherUnderCursorChanged += OnWatcherUnderCursorChanged;
			gameObject.SetActive(false);
		}

		private void OnWatcherUnderCursorChanged(SpaceObjectWatcher watcher)
		{
			var shipWatcher = watcher as ShipWatcher;
			if (shipWatcher == null)
			{
				gameObject.SetActive(false);
			}
			else
			{
				UpdateValues(shipWatcher.Ship);
				gameObject.SetActive(true);
			}
		}

		private void UpdateValues(Ship ship)
		{
			_shipNameText.text = ship.Name;
			_massValueText.text = (ship.Mass / 1e3).ToString();
			_velocityValueText.text = Mathf.RoundToInt(ship.Velocity.magnitude / 1e3f).ToString();
		}

		[SerializeField] private Text _massValueText;

		[SerializeField] private Text _shipNameText;
		[SerializeField] private Text _velocityValueText;
	}
}