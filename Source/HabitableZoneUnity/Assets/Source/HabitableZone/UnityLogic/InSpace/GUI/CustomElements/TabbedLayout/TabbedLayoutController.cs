using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HabitableZone.UnityLogic.InSpace.GUI.CustomElements.TabbedLayout
{
	/// <summary>
	///    Представляет простой контроллер UI вкладок.
	/// </summary>
	/// <remarks>Issue #7</remarks>
	public class TabbedLayoutController : MonoBehaviour
	{
		/// <summary>
		///    Список вкладок.
		/// </summary>
		/// <remarks>
		///    Изменение списка вкладок во время исполнения не поддерживается.
		/// </remarks>
		public Tab[] Tabs => _tabs;

		/// <summary>
		///    Текущая активная вкладка.
		/// </summary>
		public Tab ActiveTab
		{
			get { return _activeTab; }
			set
			{
				Assert.IsNotNull(value);
				Assert.IsTrue(Tabs.Contains(value));

				_activeTab.Active = false;
				value.Active = true;

				_activeTab = value;
			}
		}

		/// <summary>
		///    Переключает на первую доступную вкладку.
		/// </summary>
		public Tab SwitchToFirstAcessible()
		{
			var firstAcessibleTab = Array.Find(Tabs, tab => tab.Acessible);
			if (firstAcessibleTab == null)
				throw new InvalidOperationException("No acessible tabs to switch.");

			ActiveTab = firstAcessibleTab;

			return firstAcessibleTab;
		}

		/// <summary>
		///    Перезагружает активную вкладку (выключает и включает ее).
		/// </summary>
		public void ReEnableActiveTab()
		{
			Assert.IsTrue(ActiveTab.Acessible);

			ActiveTab.Active = false;
			ActiveTab.Active = true;
		}


		private void OnEnable()
		{
			_activeTab = Tabs[0];
			for (Int32 i = 0; i < Tabs.Length; i++) //Не foreach, т.к. используются замыкания
			{
				var tab = Tabs[i];
				tab.Active = false;
				tab.Acessible = tab.Acessible; //Позволяет избежать любых ошибок с неправильными начальными состояниями GO/кнопок.
				tab.Controller = this;
				tab.Button.onClick.AddListener(() => ActiveTab = tab);
			}

			SwitchToFirstAcessible();
		}

		private void OnDisable()
		{
			foreach (var tab in Tabs)
				tab.Button.onClick.RemoveAllListeners();
		}

		private Tab _activeTab;

		[SerializeField] private Tab[] _tabs;
	}
}