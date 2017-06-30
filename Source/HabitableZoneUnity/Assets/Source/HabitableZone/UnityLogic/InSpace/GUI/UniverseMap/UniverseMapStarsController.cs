using System;
using System.Linq;
using HabitableZone.Core.World.Universe;
using HabitableZone.UnityLogic.Shared;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HabitableZone.UnityLogic.InSpace.GUI.UniverseMap
{
	public class UniverseMapStarsController : MonoBehaviour
	{
		public event Action<StarSystem> TargetedStarSystemChanged;
		public Single StarPositionsMultiplier { get; private set; }

		public StarSystem TargetedStarSystem
		{
			get { return _targetedStarSystem; }
			private set
			{
				if (value == _targetedStarSystem) return;

				_targetedStarSystem = value;
				if (TargetedStarSystemChanged != null) TargetedStarSystemChanged.Invoke(value);
			}
		}

		private void OnEnable()
		{
			InstantiateStarIcons();
		}

		private void InstantiateStarIcons()
		{
			var starSystems = _sharedGOSpawner.WorldContext.StarSystems.All.ToList();

			Single width = _starIconsRectTransform.rect.width;
			Single height = _starIconsRectTransform.rect.height;

			Single maxDeltaX = starSystems.Select(starSystem => starSystem.UniverseMapPosition.x).Max();
			Single maxDeltaY = starSystems.Select(starSystem => starSystem.UniverseMapPosition.y).Max();

			Single xRatio = width / maxDeltaX, yRatio = height / maxDeltaY;
			StarPositionsMultiplier = xRatio < yRatio ? xRatio : yRatio;

			foreach (var system in starSystems)
			{
				var instantiatedStarIcon = (GameObject) Instantiate(_starIconPrefab);
				var instantiatedRectTransform = instantiatedStarIcon.GetComponent<RectTransform>();

				var capturedStarSystem = system;
				instantiatedRectTransform.SetParent(_starIconsRectTransform, false);
				instantiatedRectTransform.anchoredPosition = system.UniverseMapPosition * StarPositionsMultiplier;
				instantiatedStarIcon.GetComponent<Button>()
					.onClick.AddListener(
						() => TargetedStarSystem = capturedStarSystem);
			}
		}

		[SerializeField] private SharedGOSpawner _sharedGOSpawner;
		[SerializeField] private Object _starIconPrefab;
		[SerializeField] private RectTransform _starIconsRectTransform;

		private StarSystem _targetedStarSystem;
	}
}