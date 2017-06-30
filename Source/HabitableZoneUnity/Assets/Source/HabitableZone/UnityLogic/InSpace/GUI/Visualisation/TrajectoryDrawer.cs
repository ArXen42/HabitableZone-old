using System;
using System.Linq;
using HabitableZone.Core.ShipLogic;
using HabitableZone.Core.ShipLogic.FlightTasks;
using HabitableZone.Core.World;
using HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.Visualisation
{
	public class TrajectoryDrawer : MonoBehaviour
	{
		public static Int32 TrajectoryPointsCacheSize = 4096; //Размер кэша

		public void DrawTrajectory()
		{
			var ship = GetComponent<ShipWatcher>().Ship;
			var trajectoryPoints = ship.CurrentFlightTask.VisibleTrajectoryPoints;

			if (trajectoryPoints.Length <= 1) return;

			Int32 tdptc = FlightTask.VisibleTrajectoryPointsPerTurnCount;
			Int32 sgCounter = 0, soCounter = 0, boCounter = 0;
			for (Int32 i = 1; i < trajectoryPoints.Length - 1; i++)
			{
				if (i < tdptc)
				{
					_sgVerticesCache[sgCounter].transform.position = Units.MetersPositionToUnityUnits(trajectoryPoints[i].Position);
					_sgVerticesCache[sgCounter].SetActive(true);
					sgCounter++;
				}
				else if (i % tdptc == 0)
				{
					_boVerticesCache[boCounter].transform.position = Units.MetersPositionToUnityUnits(trajectoryPoints[i].Position);
					_boVerticesCache[boCounter].SetActive(true);
					boCounter++;
				}
				else
				{
					_soVerticesCache[soCounter].transform.position = Units.MetersPositionToUnityUnits(trajectoryPoints[i].Position);
					_soVerticesCache[soCounter].SetActive(true);
					soCounter++;
				}

				_destinationIcon.transform.position = Units.MetersPositionToUnityUnits(trajectoryPoints.Last().Position);
				_destinationIcon.SetActive(true);
			}
		}

		public void CleanTrajectory()
		{
			foreach (var smallGreenDot in _sgVerticesCache)
				smallGreenDot.SetActive(false);
			foreach (var smallOrangeDot in _soVerticesCache)
				smallOrangeDot.SetActive(false);
			foreach (var bigOrangeDot in _boVerticesCache)
				bigOrangeDot.SetActive(false);

			_destinationIcon.SetActive(false);
		}

		private void Awake()
		{
			var parent = new GameObject("Dots");

			Int32 cacheSize = TrajectoryPointsCacheSize / 2;
			Int32 greenCacheSize = TrajectoryPointsCacheSize / 8; //Зеленых много не надо

			_sgVerticesCache = new GameObject[greenCacheSize];
			_soVerticesCache = new GameObject[cacheSize];
			_boVerticesCache = new GameObject[cacheSize];


			for (Int32 i = 0; i < _sgVerticesCache.Length; i++)
			{
				_sgVerticesCache[i] = Instantiate(_greenDotPrefab, Vector2.zero, Quaternion.identity);
				_sgVerticesCache[i].name = "Small green dot " + i;
				_sgVerticesCache[i].transform.parent = parent.transform;
			}

			for (Int32 i = 0; i < _soVerticesCache.Length; i++)
			{
				_soVerticesCache[i] = Instantiate(_smallOrangeDotPrefab, Vector2.zero, Quaternion.identity);
				_soVerticesCache[i].name = "Small orange dot " + i;
				_soVerticesCache[i].transform.parent = parent.transform;
			}

			for (Int32 i = 0; i < _boVerticesCache.Length; i++)
			{
				_boVerticesCache[i] = Instantiate(_bigOrangeDotPrefab, Vector2.zero, Quaternion.identity);
				_boVerticesCache[i].name = "Big orange dot " + i;
				_boVerticesCache[i].transform.parent = parent.transform;
			}

			_destinationIcon = Instantiate(_destinationIconPrefab, Vector2.zero, Quaternion.identity);

			CleanTrajectory();
		}

		private void Start()
		{
			_worldContext = WorldHolder.GetWorldContextInCurrentScene();
			_worldContext.WorldCtl.TurnStarted += OnTurnStarted;
			GetComponent<ShipWatcher>().Ship.CurrentTaskUpdatedOrChanged += OnFlightTaskUpdatedOrChanged;
		}

		private void OnDisable()
		{
			_worldContext.WorldCtl.TurnStarted -= OnTurnStarted;
			GetComponent<ShipWatcher>().Ship.CurrentTaskUpdatedOrChanged -= OnFlightTaskUpdatedOrChanged;

			_sgVerticesCache = _soVerticesCache = _boVerticesCache = null;
		}

		private void OnTurnStarted(WorldCtl sender)
		{
			CleanTrajectory();
		}

		private void OnFlightTaskUpdatedOrChanged(Ship ship, FlightTask flightTask)
		{
			CleanTrajectory();
			DrawTrajectory();
		}

		[SerializeField] private GameObject _bigOrangeDotPrefab;
		private GameObject _destinationIcon;

		//sg - small green, so - small orange, bo - big orange

		[SerializeField] private GameObject _destinationIconPrefab;
		[SerializeField] private GameObject _greenDotPrefab;
		private GameObject[] _sgVerticesCache, _soVerticesCache, _boVerticesCache;
		[SerializeField] private GameObject _smallOrangeDotPrefab;

		private WorldContext _worldContext;
	}
}