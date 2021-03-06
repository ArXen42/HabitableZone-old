﻿using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HabitableZone.UnityLogic.Shared
{
	/// <summary>
	///    Maintains SharedGO GameObject. Creates it if it doesn't persist, does nothing if it's already loaded.
	/// </summary>
	/// <remarks>
	///    Currently used for carrying WorldContextHolder.
	/// </remarks>
	public class SharedGOSpawner : MonoBehaviour, IWorldContextProvider
	{
		/// <summary>
		///    Provides more laconical way to get WorldContext from WorldHolder attached to SharedGO.
		/// </summary>
		public WorldContext WorldContext
		{
			get
			{
				if (SharedGO == null)
					InitializeSharedGO(); //In case it was called before OnEnable

				return SharedGO.GetComponent<WorldHolder>().WorldContext;
			}
		}

		/// <summary>
		///    Returns GameObject that is shared between all scenes.
		/// </summary>
		public GameObject SharedGO { get; private set; }

		private void InitializeSharedGO()
		{
			var found = GameObject.Find(_sharedGOPrefab.name);
			if (found == null)
			{
				SharedGO = Instantiate(_sharedGOPrefab);
				SharedGO.name = "SharedGO";
				DontDestroyOnLoad(SharedGO);

				if (SceneManager.GetActiveScene().name == "Space") //Crutch for generating world not only from main menu
					SharedGO.GetComponent<WorldHolder>().WorldContext = UniverseGeneration.GenerateWorld();
			}
			else
			{
				SharedGO = found;
			}
		}

		private void OnEnable() //Initializes SharedGO on startup (works in most cases)
		{
			InitializeSharedGO();
		}

		[SerializeField] private GameObject _sharedGOPrefab;
	}
}