using System;
using System.Collections.Generic;
using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe;
using HabitableZone.Core.World.Universe.CelestialBodies;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace HabitableZone.UnityLogic.InSpace.LevelInitialization
{
	/// <summary>
	///    Maintains representation of observing StarSystem (Unity scene).
	///    Generally, tracks SpaceObject's in observing system.
	/// </summary>
	public sealed class StarSystemViewController : MonoBehaviour, IWorldContextProvider
	{
		public WorldContext WorldContext => ObservingStarSystem.WorldContext;

		/// <summary>
		///    Observing StarSystem. For now - in which player is.
		/// </summary>
		public StarSystem ObservingStarSystem { get; private set; }

		/// <summary>
		///    Called from PlayerShipController when player has jumped from ObservingStarSystem.
		/// </summary>
		public void ReloadScene()
		{
			SceneManager.LoadScene("Space");
		}

		/// <summary>
		///    Returns GameObject representation of given logical SpaceObject.
		/// </summary>
		public GameObject GetSpaceObjectRepresentation(SpaceObject spaceObject)
		{
			Assert.IsTrue(_gameObjectsDictionary.ContainsKey(spaceObject),
				"Attempted to get GameObject representation of SpaceObject that is not located in observing system.");

			return _gameObjectsDictionary[spaceObject];
		}

		private void OnEnable()
		{
			var worldContext = GetComponent<SharedGOSpawner>().WorldContext;

			ObservingStarSystem = worldContext.Captains.Player.CurrentShip.Location;

			_gameObjectsDictionary = new Dictionary<SpaceObject, GameObject>();

			CreateInitializersDictionary();
			InitializeScene();

			ObservingStarSystem.SpaceObjectAdded += OnSpaceObjectAdded;
			ObservingStarSystem.SpaceObjectRemoved += OnSpaceObjectRemoved;
		}

		private void OnDisable()
		{
			ObservingStarSystem.SpaceObjectAdded -= OnSpaceObjectAdded;
			ObservingStarSystem.SpaceObjectRemoved -= OnSpaceObjectRemoved;
		}

		private void InitializeSpaceObject(SpaceObject spaceObject)
		{
			var spaceObjectRepresentation = _initializers[spaceObject.GetType()].Invoke(spaceObject);
			_gameObjectsDictionary.Add(spaceObject, spaceObjectRepresentation);
		}

		private void InitializeScene()
		{
			foreach (var spaceObject in ObservingStarSystem.AllSpaceObjects)
				InitializeSpaceObject(spaceObject);
		}

		private void OnSpaceObjectAdded(StarSystem sender, SpaceObject spaceObject)
		{
			Assert.IsFalse(_gameObjectsDictionary.ContainsKey(spaceObject),
				$"Tried to initialize representation of {spaceObject.Name} twice.");

			InitializeSpaceObject(spaceObject);
		}

		private void OnSpaceObjectRemoved(StarSystem sender, SpaceObject spaceObject)
		{
			var removingGameObject = _gameObjectsDictionary[spaceObject];

			_gameObjectsDictionary.Remove(spaceObject);
			Destroy(removingGameObject);
		}

		private void CreateInitializersDictionary()
		{
			var starInitializer = GetComponent<StarInitializer>();
			var planetInitializer = GetComponent<PlanetInitializer>();
			var asteroidFieldInitializer = GetComponent<AsteroidFieldInitializer>();
			var shipInitializer = GetComponent<ShipInitializer>();

			_initializers = new Dictionary<Type, Func<SpaceObject, GameObject>>
			{
				{typeof(Star), so => starInitializer.InitializeStar((Star) so)},
				{typeof(Planet), so => planetInitializer.InitializePlanet((Planet) so)},
				{typeof(AsteroidField), so => asteroidFieldInitializer.InitializeAsteroidField((AsteroidField) so)},
				{typeof(Ship), so => shipInitializer.InitializeShip((Ship) so)}
			};
		}

		/// <summary>
		///    Contains all created representations of SpaceObjects in this scene.
		/// </summary>
		private Dictionary<SpaceObject, GameObject> _gameObjectsDictionary;

		/// <summary>
		///    Contains components-initializers for each kind of SpaceObject.
		/// </summary>
		private Dictionary<Type, Func<SpaceObject, GameObject>> _initializers;
	}
}