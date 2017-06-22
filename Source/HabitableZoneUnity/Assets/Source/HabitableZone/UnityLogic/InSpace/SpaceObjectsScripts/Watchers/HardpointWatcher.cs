using HabitableZone.Core.SpacecraftStructure;
using UnityEngine;

namespace HabitableZone.UnityLogic.InSpace.SpaceObjectsScripts.Watchers
{
	/// <summary>
	///    Maintains GameObject --> Hardpoint linkage.
	/// </summary>
	public class HardpointWatcher : MonoBehaviour
	{
		public Hardpoint Hardpoint { get; private set; }

		public void Init(Hardpoint hardpoint)
		{
			Hardpoint = hardpoint;
		}
	}
}