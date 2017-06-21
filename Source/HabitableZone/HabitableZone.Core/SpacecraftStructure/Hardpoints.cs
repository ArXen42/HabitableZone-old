using System;
using System.Collections;
using System.Collections.Generic;
using HabitableZone.Common;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure
{
	/// <summary>
	///    Observable collection of hardpoints with all necessary logick.
	/// </summary>
	public class Hardpoints : IEnumerable<Hardpoint>
	{
		public Hardpoints(Spacecraft spacecraft)
		{
			Spacecraft = spacecraft;
		}

		public IEnumerator<Hardpoint> GetEnumerator()
		{
			return _hardpoints.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Int32 Count => _hardpoints.Count;

		public void ForEach(Action<Hardpoint> action)
		{
			_hardpoints.ForEach(action);
		}

		/// <summary>
		///    Owner spacecraft.
		/// </summary>
		public readonly Spacecraft Spacecraft;

		/// <summary>
		///    Occurs when some hardpoint is mounted.
		/// </summary>
		public event SEventHandler<Hardpoints, Hardpoint> HardpointMounted;

		/// <summary>
		///    Occurs when some hardpoint is unmounted (no longer attached to spacecraft).
		/// </summary>
		public event SEventHandler<Hardpoints, Hardpoint> HardpointUnmounted;

		/// <summary>
		///    Attaches given hardpoint to the spacecraft.
		/// </summary>
		public void Mount(Hardpoint hardpoint)
		{
			_hardpoints.Add(hardpoint);
			hardpoint.HandleMountToSpacecraft(Spacecraft);

			HardpointMounted?.Invoke(this, hardpoint);
		}

		/// <summary>
		///    Detaches given hardpoin from the spacecraft.
		/// </summary>
		public void Unmount(Hardpoint hardpoint)
		{
			_hardpoints.Remove(hardpoint);
			hardpoint.HandleUnmountFromSpacecraft();

			Assert.IsFalse(hardpoint.IsEquipmentInstalled,
				"Hardpoint was unmounted but some equipment is still installed in it.");

			HardpointUnmounted?.Invoke(this, hardpoint);
		}

		private readonly List<Hardpoint> _hardpoints = new List<Hardpoint>();
	}
}