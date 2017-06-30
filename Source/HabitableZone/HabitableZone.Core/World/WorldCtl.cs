using System;
using HabitableZone.Common;
using UnityEngine;

namespace HabitableZone.Core.World
{
	/// <summary>
	///    Controls date/time and turn switching.
	/// </summary>
	/// <remarks>
	///    Game logic actually don't care about turn timer - actually it is perfomed in TurnStarted and TurnStopped.
	///    As for now, the timer is used only to interpolate game process for player.
	/// </remarks>
	public class WorldCtl
	{
		/// <summary>
		///    In-game duration of a turn in seconds.
		/// </summary>
		public static Single TurnDuration = 1.5f;

		public WorldCtl(WorldCtlData data)
		{
			Date = data.Date;
		}

		/// <summary>
		///    Occurs when the turn is started.
		/// </summary>
		/// <remarks>
		///    At this point, IsTurnActive is false and NormalizedTurnElapsedTime returns 0.
		/// </remarks>
		public event CEventHandler<WorldCtl> TurnStarted;

		/// <summary>
		///    Occurs when the time is up and the turn is ended.
		/// </summary>
		/// <remarks>
		///    At this point, IsTurnActive is true and NormalizedTurnElapsedTime returns 1.
		/// </remarks>
		public event CEventHandler<WorldCtl> TurnStopped;

		/// <summary>
		///    Current date.
		/// </summary>
		public DateTime Date { get; private set; }

		/// <summary>
		///    Determines whether the turn currently active.
		/// </summary>
		/// <remarks>
		///    The turn is considered active in the interval from TurnStarted invocation (not including) and to TurnStopped
		///    invocation (including).
		/// </remarks>
		public Boolean IsTurnActive { get; private set; }

		/// <summary>
		///    The ratio of time elapsed since TurnStarted to TurnDuration.
		/// </summary>
		/// <remarks>
		///    Returns 0 if turn isn't active and 1 if time's up (at the time of TurnStopped invocation).
		/// </remarks>
		public Single NormalizedTurnElapsedTime { get; private set; }

		public WorldCtlData GetSerializationData()
		{
			return new WorldCtlData(this);
		}

		/// <summary>
		///    Starts turn.
		/// </summary>
		public void StartTurn()
		{
			Assert.IsFalse(IsTurnActive, "Attempted to start turn while it was already started.");

			TurnStarted?.Invoke(this);
			IsTurnActive = true;
			_turnStartTime = Time.time;
			_turnStartDate = Date;
		}

		/// <summary>
		///    Internal method that sets NormalizedTurnElapsedTime to 1 and ends turn.
		/// </summary>
		/// <remarks>
		///    Not for external use. Use TurnSwitchController instead.
		/// </remarks>
		public void StopTurn()
		{
			Assert.IsTrue(IsTurnActive, "Attempted to stop turn while it was already stopped.");

			NormalizedTurnElapsedTime = 1;

			Date = _turnStartDate.AddDays(1);
			TurnStopped?.Invoke(this);
			IsTurnActive = false;

			_turnStartTime = 0f;
			NormalizedTurnElapsedTime = 0;
		}

		/// <summary>
		///    Internal method called from TurnSwitcher.Update.
		/// </summary>
		/// <remarks>
		///    If TurnSwitchController.FastTurnSwitchModeEnabled then TurnSwitchController will instead call TurnStop by itself.
		/// </remarks>
		public void UpdateAndCheckTimer()
		{
			Single normalizedTurnElapsedTime = (Time.time - _turnStartTime) / TurnDuration;

			if (normalizedTurnElapsedTime >= 1)
			{
				StopTurn();
			}
			else
			{
				NormalizedTurnElapsedTime = normalizedTurnElapsedTime;
				Date = _turnStartDate.AddSeconds(normalizedTurnElapsedTime * 86400);
			}
		}

		private DateTime _turnStartDate;

		private Single _turnStartTime;
	}

	public struct WorldCtlData
	{
		public WorldCtlData(WorldCtl worldCtl)
		{
			Date = worldCtl.Date;
		}

		public WorldCtl GetInstanceFromData()
		{
			return new WorldCtl(this);
		}

		public DateTime Date;
	}
}