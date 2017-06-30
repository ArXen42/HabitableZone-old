using System;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.HUD.SpaceObjectInfo
{
	public class SpaceObjectInfoController : MonoBehaviour
	{
		public event Action<SpaceObjectWatcher> WatcherUnderCursorChanged;

		public SpaceObjectWatcher WatcherUnderCursor
		{
			get { return _watcherUnderCursor; }
			private set
			{
				if (_watcherUnderCursor == value) return;

				_watcherUnderCursor = value;
				if (WatcherUnderCursorChanged != null) WatcherUnderCursorChanged.Invoke(value);
			}
		}

		private void Update()
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Boolean raycastHitted = Physics.Raycast(ray, out hit);

			WatcherUnderCursor = raycastHitted ? hit.collider.gameObject.GetComponentInParent<SpaceObjectWatcher>() : null;
		}

		private SpaceObjectWatcher _watcherUnderCursor;
	}
}