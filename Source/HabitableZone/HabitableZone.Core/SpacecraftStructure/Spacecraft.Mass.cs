using System;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure
{
	public partial class Spacecraft
	{
		/// <summary>
		///    Occurs when mass is changed.
		/// </summary>
		public event CEventHandler<Spacecraft, MassChangedEventArgs> MassChanged;

		/// <summary>
		///    Mass of this spacecraft.
		/// </summary>
		public Single Mass
		{
			get { return _mass; }
			private set
			{
				Single oldValue = _mass;

				if (oldValue == value) return;

				_mass = value;
				MassChanged?.Invoke(this, new MassChangedEventArgs(oldValue, value));
			}
		}

		private void RecalculateMass()
		{
			Single result = 0;
			Hardpoints.ForEach(hp => result += hp.Mass);

			Mass = result;
		}

		private void OnHardpointMounted(Hardpoints sender, Hardpoint hardpoint)
		{
			hardpoint.MassChanged += OnHardpointMassChanged;
			RecalculateMass();
		}

		private void OnHardpointUnmounted(Hardpoints sender, Hardpoint hardpoint)
		{
			hardpoint.MassChanged -= OnHardpointMassChanged;
			RecalculateMass();
		}

		private void OnHardpointMassChanged(Hardpoint sender, MassChangedEventArgs args)
		{
			RecalculateMass();
		}

		private Single _mass;
	}
}