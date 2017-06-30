using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HabitableZone.UnityLogic.InSpace.GUI.CustomElements.TabbedLayout
{
	/// <summary>
	///    Представляет UI вкладку.
	/// </summary>
	[Serializable]
	public class Tab
	{
		/// <summary>
		///    Состояние (вкл/выкл) вкладки в данный момент.
		/// </summary>
		public Boolean Active
		{
			get { return _active; }
			set
			{
				if (value && !Acessible)
					throw new InvalidOperationException("Tried to enable inacessible tab.");

				ControlledGameObject.SetActive(value);
				_active = value;
			}
		}

		/// <summary>
		///    Доступность вкладки в данный момент.
		/// </summary>
		public Boolean Acessible
		{
			get { return _acessible; }
			set
			{
				Assert.IsFalse(Active && !_acessible); //Вкладка не могла быть включена, если была недоступна.

				_acessible = value;
				Button.interactable = value;

				if (!_acessible && Active)
					Controller.SwitchToFirstAcessible();
			}
		}

		/// <summary>
		///    Кнопка, переклюшая вкладку.
		/// </summary>
		[SerializeField] public Button Button;

		/// <summary>
		///    Контроллируемый GO.
		/// </summary>
		[SerializeField] public GameObject ControlledGameObject;

		/// <summary>
		///    Будет установлен при инициализации в TabbedLayoutController.
		/// </summary>
		/// <remarks>
		///    Не изменять, воспринимать как readonly.
		/// </remarks>
		[NonSerialized] public TabbedLayoutController Controller;

		private Boolean _acessible = true;
		private Boolean _active;
	}
}