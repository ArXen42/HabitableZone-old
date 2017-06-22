using System;
using System.Collections.Generic;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.Screens
{
	/// <summary>
	///    Реализует логику простого игрового экрана.
	/// </summary>
	public class GUIScreen : MonoBehaviour
	{
		[SerializeField] public List<GameObject> GameObjects;
		[SerializeField] public Boolean InitialState;
		[SerializeField] public String Name;

		public virtual Boolean Active
		{
			get { return _active; }
			set
			{
				if (_active == value) return;

				_active = value;
				GameObjects.ForEach(go => go.SetActive(value));
				OnStateChanged(value);
			}
		}

		public event EventHandler Enabled;
		public event EventHandler Disabled;

		protected void OnStateChanged(Boolean state)
		{
			if (state)
			{
				if (Enabled != null) Enabled.Invoke(this, EventArgs.Empty);
			}
			else
			{
				if (Disabled != null) Disabled.Invoke(this, EventArgs.Empty);
			}
		}

		private void Start()
		{
			if (!InitialState)
				_active = true; //Принудительное изменение состояния при запуске

			Active = InitialState;
		}

		private Boolean _active;
	}
}