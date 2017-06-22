using System;
using System.IO;
using HabitableZone.Common;
using HabitableZone.Core;
using HabitableZone.Core.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HabitableZone.UnityLogic.Shared
{
	/// <summary>
	///    Provides access to currently loaded world. It's not destroyed when (re)loading scene.
	/// </summary>
	public class WorldHolder : MonoBehaviour, IWorldContextProvider
	{
		/// <summary>
		///    Searches for SharedGO in loaded scenes and returns WorldHolder on it.
		/// </summary>
		public static WorldHolder GetWorldHolderInCurrentScene() => GameObject.Find("SharedGO").GetComponent<WorldHolder>();

		/// <summary>
		///    Searches for SharedGO in loaded scenes and returns WorldHolder.WorldContext on it.
		/// </summary>
		/// <returns></returns>
		public static WorldContext GetWorldContextInCurrentScene() => GetWorldHolderInCurrentScene().WorldContext;

		/// <summary>
		///    Currently loaded world.
		/// </summary>
		public WorldContext WorldContext
		{
			get { return _worldContext; }
			set
			{
				if (SceneManager.GetActiveScene().name == "Space" && IsWorldLoaded)
					throw new NotSupportedException("World was reloaded from Space scene." +
					                                "Before deleting this make sure it's intended and appropriate logic persists.");

				_worldContext = value;
				WorldContextChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		///    Determines whether some world was loaded.
		/// </summary>
		public Boolean IsWorldLoaded => _worldContext != null;

		/// <summary>
		///    Occurs when world is changed.
		/// </summary>
		public event SEventHandler<WorldHolder, WorldContext> WorldContextChanged;

		private void OnDisable()
		{
			if (IsWorldLoaded)
				using (var stream = new FileStream("save.json", FileMode.Create))
					WorldContext.SerializeTo(stream);
		}

		private WorldContext _worldContext;
	}
}