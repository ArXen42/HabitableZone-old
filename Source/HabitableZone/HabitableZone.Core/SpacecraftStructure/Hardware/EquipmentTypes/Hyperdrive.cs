using System;
using HabitableZone.Common;
using HabitableZone.Core.World;
using HabitableZone.Core.World.Universe;
using Pathfinding.Serialization.JsonFx;
using UnityEngine;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.EquipmentTypes
{
	/// <summary>
	///    Представляет функционал гипердвигателя.
	/// </summary>
	public class Hyperdrive : Equipment
	{
		public static readonly Single HyperjumpDurationFactor = 1 / 30f;

		public static readonly Single HyperspaceEntryPointDistance = 60e9f;

		public Hyperdrive(HyperdriveData data) : base(data)
		{
			MaxJumpDist = data.MaxJumpDist;
			CurrentHyperjumpInfo = data.HyperjumpInfo;
		}

		public override EquipmentData GetSerializationData()
		{
			return new HyperdriveData(this);
		}

		public Single MaxJumpDist { get; }

		/// <summary>
		///    Если корабль находится в гиперпространстве, то будет содержать информацию о текущем гиперпрыжке.
		/// </summary>
		public HyperjumpInfo CurrentHyperjumpInfo
		{
			get { return _currentHyperjumpInfo; }
			private set
			{
				if (value == null) //Значит, прыжка нет и можно не слушать WorldCtl.TurnStarted
				{
					if (_currentHyperjumpInfo != null) //Else we might be not installed and WorldContext will throw NullReferenceException
						WorldContext.WorldCtl.TurnStarted -= OnTurnStarted;
				}
				else
				{
					Assert.IsNull(_currentHyperjumpInfo);
					WorldContext.WorldCtl.TurnStarted += OnTurnStarted;
				}

				_currentHyperjumpInfo = value;
			}
		}

		/// <summary>
		///    Возможен ли прыжок в текущую систему.
		/// </summary>
		public Boolean IsJumpPossible(StarSystem targetStarSystem)
		{
			if (targetStarSystem == Spacecraft.Location)
				return false;

			var currentPos = Spacecraft.Location.UniverseMapPosition;
			var targetPos = targetStarSystem.UniverseMapPosition;
			return Vector2.Distance(currentPos, targetPos) <= MaxJumpDist; //TODO: учет топлива
		}

		/// <summary>
		///    Вернет угол на целевую систему из текущей системы игрока либо,
		///    если игрок в процессе гиперпрыжка, из той, из которой выполнялся прыжок.
		/// </summary>
		public Single GetJumpAngle(StarSystem targetStarSystem)
		{
			var startPos =
			(CurrentHyperjumpInfo == null
				? Spacecraft.Location
				: CurrentHyperjumpInfo.StartSystem).UniverseMapPosition;

			var targetPos = targetStarSystem.UniverseMapPosition;
			return Geometry.EulerAngleOfVector(targetPos - startPos);
		}

		/// <summary>
		///    Использует GetJumpAngle для рассчета точки входа в гиперпрыжок.
		/// </summary>
		public Vector2 GetHyperspaceEntryPoint(StarSystem targetStarSystem)
		{
			return Geometry.VertexOnLine(
				Vector2.zero,
				GetJumpAngle(targetStarSystem),
				HyperspaceEntryPointDistance);
		}

		/// <summary>
		///    Использует GetJumpAngle для рассчета точки выхода из гиперпрыжка.
		/// </summary>
		public Vector2 GetHyperspaceExitPoint(StarSystem targetStarSystem)
		{
			return Geometry.VertexOnLine(
				Vector2.zero,
				GetJumpAngle(targetStarSystem) + 180,
				HyperspaceEntryPointDistance);
		}

		/// <summary>
		///    Выполняет прыжок в указанную систему. Перемещает корабль игрока в StarSystem.Void и сохраняет информацию о прыжке.
		/// </summary>
		/// <exception cref="InvalidOperationException">Будет выброшено, если прыжок совершить невозможно. Расценивать как баг.</exception>
		public void Jump(StarSystem targetStarSystem)
		{
			Assert.IsFalse(Spacecraft.Location == WorldContext.StarSystems.Void);
			Assert.IsTrue(CurrentHyperjumpInfo == null);

			if (!IsJumpPossible(targetStarSystem))
				throw new InvalidOperationException("Jump failed: can't jump from current star system.");

			var currentPos = Spacecraft.Location.UniverseMapPosition;
			var targetPos = targetStarSystem.UniverseMapPosition;

			var jumpDuration =
				new TimeSpan(Mathf.RoundToInt(Vector2.Distance(currentPos, targetPos) * HyperjumpDurationFactor) + 1, 0, 0, 0, 0);

			CurrentHyperjumpInfo = new HyperjumpInfo(WorldContext.WorldCtl.Date + jumpDuration, Spacecraft.Location, targetStarSystem);

			Spacecraft.Location = WorldContext.StarSystems.Void;
		}

		/// <summary>
		///    Если корабль в состоянии гиперпрыжка, то подписан на WorldCtl.TurnStarted.
		///    Если текущая дата совпадает с датой прибытия, перемещает корабль в целевую систему.
		/// </summary>
		/// <remarks>
		///    Продолжение банкета - HyperjumpFlightTask.OnTargetStarSystemReached.
		/// </remarks>
		private void OnTurnStarted(WorldCtl sender)
		{
			if (WorldContext.WorldCtl.Date < CurrentHyperjumpInfo.ArrivalDate) return; //Если еще рано, ждем дальше

			Spacecraft.Location = CurrentHyperjumpInfo.TargetSystem;
			CurrentHyperjumpInfo = null;
		}

		private HyperjumpInfo _currentHyperjumpInfo;
	}

	public class HyperdriveData : EquipmentData
	{
		public HyperdriveData() { }

		public HyperdriveData(Hyperdrive hyperdrive) : base(hyperdrive)
		{
			MaxJumpDist = hyperdrive.MaxJumpDist;
			HyperjumpInfo = hyperdrive.CurrentHyperjumpInfo;
		}

		public override Equipment GetInstanceFromData()
		{
			return new Hyperdrive(this);
		}

		public HyperjumpInfo HyperjumpInfo;

		public Single MaxJumpDist;
	}

	/// <summary>
	///    Представляет информацию о текущем гиперпрыжке.
	/// </summary>
	[Serializable]
	public class HyperjumpInfo
	{
		public HyperjumpInfo(DateTime arrivalDate, StarSystem startSystem, StarSystem targetSystem)
		{
			ArrivalDate = arrivalDate;
			StartSystem = startSystem;
			TargetSystem = targetSystem;

			_worldContext = startSystem.WorldContext;
		}

		public DateTime ArrivalDate { get; }
		public Guid StarSystemID { get; private set; }
		public Guid TargetSystemID { get; private set; }

		[JsonIgnore]
		public StarSystem StartSystem
		{
			get { return _worldContext.StarSystems.ByID(StarSystemID); }
			set { StarSystemID = value.ID; }
		}

		[JsonIgnore]
		public StarSystem TargetSystem
		{
			get { return _worldContext.StarSystems.ByID(TargetSystemID); }
			set { TargetSystemID = value.ID; }
		}

		private readonly WorldContext _worldContext;
	}
}