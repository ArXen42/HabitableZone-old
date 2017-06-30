using System;
using HabitableZone.Core.SpacecraftStructure;
using HabitableZone.UnityLogic.InSpace.Control;
using HabitableZone.UnityLogic.InSpace.LevelInitialization;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView
{
	/// <summary>
	///    Представляет функционал для работы экрана управления кораблем.
	/// </summary>
	public class SpacecraftViewController : MonoBehaviour
	{
		public event EventHandler SelectionChanged;
//		public static SpacecraftViewController Instance { get; private set; }

		public Spacecraft Spacecraft => _starSystemViewController.WorldContext.Captains.Player.CurrentShip;

		public SpacecraftStructureController StructureController => _starSystemViewController
			.GetSpaceObjectRepresentation(Spacecraft)
			.GetComponent<SpacecraftStructureController>();

		public Hardpoint SelectedHardpoint
		{
			get { return _selectedHardpoint; }
			private set
			{
				if (_selectedHardpoint == value) return;

				_selectedHardpoint = value;
				if (SelectionChanged != null)
					SelectionChanged.Invoke(null, EventArgs.Empty);
			}
		}

		public Boolean AnyHardpointSelected => _selectedHardpoint != null;

//		private void OnEnable()
//		{
//			Instance = this;
//		}

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0)) return;

			var ray = _camerasController.HullCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (!Physics.Raycast(ray, out hit, LayerMask.NameToLayer("Hardpoints"))) return;

			var hardpointWatcher = hit.collider.gameObject.GetComponent<HardpointWatcher>();
			if (hardpointWatcher != null)
				SelectedHardpoint = hardpointWatcher.Hardpoint;
		}

		[SerializeField] private SpacecraftViewCamerasController _camerasController;

		private Hardpoint _selectedHardpoint;
		[SerializeField] private StarSystemViewController _starSystemViewController;
	}
}