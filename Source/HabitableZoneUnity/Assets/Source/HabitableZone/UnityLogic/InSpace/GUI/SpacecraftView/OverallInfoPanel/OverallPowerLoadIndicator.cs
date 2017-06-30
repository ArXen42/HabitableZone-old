using System;
using HabitableZone.Core.SpacecraftStructure.Hardware.Electricity;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.GUI.SpacecraftView.OverallInfoPanel
{
	public class OverallPowerLoadIndicator : MonoBehaviour
	{
		private ElectricitySubsystem SpacecraftElectricitySubsystem => _spacecraftViewController.Spacecraft.ElectricitySubsystem;

		private void OnEnable()
		{
			_spacecraftViewController = GetComponentInParent<SpacecraftViewController>();

			if (_filledPoints == null || _emptyPoints == null)
				InitializePool();

			SpacecraftElectricitySubsystem.PowerStateChanged += OnPowerLoadChanged;

			OnPowerLoadChanged(null);
		}

		private void OnDisable()
		{
			SpacecraftElectricitySubsystem.PowerStateChanged -= OnPowerLoadChanged;
		}

		private void InitializePool()
		{
			_filledPoints = new GameObject[_pointsCount];
			_emptyPoints = new GameObject[_pointsCount];
			Int32 interval = _length / _pointsCount;

			for (Int32 i = 0; i < _pointsCount; i++)
			{
				{
					_filledPoints[i] = Instantiate(_filledPointPrefab);
					var rectTransform = _filledPoints[i].GetComponent<RectTransform>();
					rectTransform.SetParent(transform);
					rectTransform.localPosition = new Vector3(i * interval, 0, 0);
					_filledPoints[i].SetActive(false);
				}

				{
					_emptyPoints[i] = Instantiate(_emptyPointPrefab);
					var rectTransform = _emptyPoints[i].GetComponent<RectTransform>();
					rectTransform.SetParent(transform);
					rectTransform.localPosition = new Vector3(i * interval, 0, 0);
					_emptyPoints[i].SetActive(false);
				}
			}
		}

		private void OnPowerLoadChanged(ElectricitySubsystem electricitySubsystem)
		{
			Single producinging = SpacecraftElectricitySubsystem.OverallProducingPower;
			Single consuming = SpacecraftElectricitySubsystem.OverallConsumingPower;

			Int32 filledPointsCount = (Int32) Math.Round(consuming / producinging * _pointsCount);

			for (Int32 i = 0; i < _pointsCount; i++)
				if (i < filledPointsCount)
				{
					_emptyPoints[i].SetActive(false);
					_filledPoints[i].SetActive(true);
				}
				else
				{
					_filledPoints[i].SetActive(false);
					_emptyPoints[i].SetActive(true);
				}
		}

		[SerializeField] private GameObject _emptyPointPrefab;

		[SerializeField] private GameObject _filledPointPrefab;

		private GameObject[] _filledPoints, _emptyPoints; //Пул делений шкалы
		[SerializeField] private Int32 _length; //Длина в пикселях относительно canvas
		[SerializeField] private Int32 _pointsCount; //Длина в пикселях относительно canvas
		private SpacecraftViewController _spacecraftViewController;
	}
}