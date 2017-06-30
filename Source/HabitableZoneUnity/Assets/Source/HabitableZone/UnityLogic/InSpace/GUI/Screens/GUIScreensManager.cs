using System;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.Screens
{
	/// <summary>
	///    Контроллирует переключение игровых экранов.
	/// </summary>
	public class GUIScreensManager : MonoBehaviour
	{
		public static GUIScreen HUDScreen { get; private set; }
		public static GUIScreen MinimapScreen { get; private set; }
		public static GUIScreen SpacecraftViewScreen { get; private set; }
		public static GUIScreen UniverseMapScreen { get; private set; }

		public void SetActiveSpacecraftViewScreen(Boolean state)
		{
			SpacecraftViewScreen.Active = state;
		}

		public void ToggleUniverseMapScreen()
		{
			UniverseMapScreen.Active = !UniverseMapScreen.Active;
		}

		private void Awake()
		{
			var components = GetComponents<GUIScreen>();

			HUDScreen = Array.Find(components, screen => screen.Name == "HUDScreen");
			MinimapScreen = Array.Find(components, screen => screen.Name == "MinimapScreen");
			SpacecraftViewScreen = Array.Find(components, screen => screen.Name == "SpacecraftViewScreen");
			UniverseMapScreen = Array.Find(components, screen => screen.Name == "UniverseMapScreen");

			HUDScreen.Enabled += (sender, e) => MinimapScreen.Active = true;
			HUDScreen.Disabled += (sender, e) => MinimapScreen.Active = false;

			SpacecraftViewScreen.Enabled += (sender, e) => HUDScreen.Active = false;
			SpacecraftViewScreen.Disabled += (sender, e) => HUDScreen.Active = true;
		}
	}
}