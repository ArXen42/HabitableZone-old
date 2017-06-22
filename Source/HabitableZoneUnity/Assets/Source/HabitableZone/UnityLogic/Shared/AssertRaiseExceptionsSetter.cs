using UnityEngine;
using UnityEngine.Assertions;

namespace HabitableZone.UnityLogic.Shared
{
	public class AssertRaiseExceptionsSetter : MonoBehaviour
	{
		private void OnEnable()
		{
			Assert.raiseExceptions = true;
		}
	}
}