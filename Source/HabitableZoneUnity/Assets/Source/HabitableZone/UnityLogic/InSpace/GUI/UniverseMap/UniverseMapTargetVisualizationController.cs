using HabitableZone.Core.World.Universe;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.UniverseMap
{
	public class UniverseMapTargetVisualizationController : MonoBehaviour
	{
		private void OnEnable()
		{
			_universeMapStarsController = GetComponentInParent<UniverseMapStarsController>();
			_universeMapStarsController.TargetedStarSystemChanged += OnTargetedStarSystemChanged;
		}

		private void OnDisable()
		{
			_universeMapStarsController.TargetedStarSystemChanged -= OnTargetedStarSystemChanged;
		}

		private void OnTargetedStarSystemChanged(StarSystem starSystem)
		{
			if (starSystem == null)
			{
				_targetBoundsRectTransform.gameObject.SetActive(false);
			}
			else
			{
				_targetBoundsRectTransform.gameObject.SetActive(true);
				_targetBoundsRectTransform.anchoredPosition = starSystem.UniverseMapPosition *
																			 _universeMapStarsController.StarPositionsMultiplier;
			}
		}

		[SerializeField] private RectTransform _targetBoundsRectTransform;
		private UniverseMapStarsController _universeMapStarsController;
	}
}