using HabitableZone.Core.World;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	/// <summary>
	///    Base class for SpaceObject's watchers.
	/// </summary>
	public abstract class SpaceObjectWatcher : MonoBehaviour
	{
		public abstract SpaceObject SpaceObject { get; }
	}
}