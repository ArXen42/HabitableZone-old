using HabitableZone.Core.World.Universe;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace HabitableZone.UnityLogic.MainMenu.UI
{
	/// <summary>
	///    Right buttons (New/Load/Settings/etc) call methods from this script.
	/// </summary>
	public class RightButtonsController : MonoBehaviour
	{
		public void StartNewGame()
		{
			var worldContext = UniverseGeneration.GenerateWorld();
			var worldHolder = WorldHolder.GetWorldHolderInCurrentScene();

			Assert.IsFalse(worldHolder.IsWorldLoaded, "Some world was already loaded in WorldHolder while started a new game from main menu.");

			worldHolder.WorldContext = worldContext;
			SceneManager.LoadScene("Space");
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}