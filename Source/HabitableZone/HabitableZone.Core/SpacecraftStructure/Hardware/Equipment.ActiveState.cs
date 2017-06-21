using System;
using System.Collections.Generic;
using System.Linq;
using HabitableZone.Common;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	public partial class Equipment
	{
		/// <summary>
		///    Occurs when equipment is enabled or disabled.
		/// </summary>
		public event SEventHandler<Equipment, Boolean> EnabledChanged;

		/// <summary>
		///    Occurs when TargetEnabled has changed.
		/// </summary>
		public event SEventHandler<Equipment, Boolean> TargetEnabledChanged;

		/// <summary>
		///    Is the equipment currently enabled.
		/// </summary>
		public Boolean Enabled
		{
			get { return _enabled; }
			private set
			{
				Assert.IsFalse(_enabled == value);
				Assert.IsTrue(IsInstalled);

				_enabled = value;
				EnabledChanged?.Invoke(this, _enabled);
			}
		}

		/// <summary>
		///    Target state of this equipment, determines if it is pending to be enabled.
		/// </summary>
		public Boolean TargetEnabled
		{
			get { return _targetEnabled; }
			private set
			{
				if (value == _targetEnabled) return;

				_targetEnabled = value;
				TargetEnabledChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		///    Sets TargetEnabled to true. Equipment will be enabled as soon as possible.
		/// </summary>
		public void RequestEngagement()
		{
			Assert.IsFalse(TargetEnabled, "Requested equipment engagement, but it is already pending it.");

			TargetEnabled = true;

			if (_dependencies.Count == 0)
			{
				Assert.IsFalse(Enabled, "How could this equipment had been enabled if there were no dependencies?");
				TryToEngage();
			}
		}

		/// <summary>
		///    Sets TargetEnabled to false, which is currently leads to equipment immediate shutdown.
		/// </summary>
		public void RequestDisengagement()
		{
			TargetEnabled = false;
			if (Enabled) Disengage();

			Assert.IsFalse(Enabled, "TargetEnabled was set to false but this action didn't lead to disengagement.");
		}

		/// <summary>
		///    Registers object that determines whether that equipment is allowed to be powered on
		///    by some subsystem or another external or internal dependency.
		/// </summary>
		public void RegisterDependency(IEquipmentEngagementDependency dependency)
		{
			Assert.IsTrue(dependency.Equipment == this, "Attempted to register foreign dependency.");

			_dependencies.Add(dependency);

			dependency.EngagementAllowed += OnDependencyAllowedEngagement;
			dependency.EngagementProhibited += OnDependencyProhibitedEngagement;
		}

		/// <summary>
		///    Unregisters engagement dependency.
		/// </summary>
		public void UnregisterDependency(IEquipmentEngagementDependency dependency)
		{
			dependency.EngagementAllowed -= OnDependencyAllowedEngagement;
			dependency.EngagementProhibited -= OnDependencyProhibitedEngagement;

			_dependencies.Remove(dependency);
		}

		private void OnDependencyAllowedEngagement(IEquipmentEngagementDependency sender)
		{
			if (!IsInstalled) return;
			//We don't need to (un)subscribe from/to dependencies since there should be
			//no external references to them preventing equipment to be garbage-collected (if it no longer needed).
			//Although Equipment.Destroy is coming anyway, so it would be good to unregister all dependencies in it.

			Assert.IsTrue(_dependencies.Contains(sender), "Reacted to dependency that wasn't in dependencies list.");

			TryToEngage();
		}

		private void OnDependencyProhibitedEngagement(IEquipmentEngagementDependency sender)
		{
			Assert.IsTrue(_dependencies.Contains(sender), "Reacted to dependency that wasn't in dependencies list.");

			if (Enabled)
				Disengage();
		}

		private void TryToEngage()
		{
			Assert.IsTrue(TargetEnabled, "Attempted to enable equipment, but it's TargetEnabled is false.");
			Assert.IsFalse(Enabled, "Attempted to enable equipment, but it was already enabled.");

			Boolean engagementAllowed = _dependencies.All(dependency => dependency.IsEngagementAllowed);

			if (engagementAllowed)
				Engage();
		}

		private void Engage()
		{
			Enabled = true;
		}

		private void Disengage()
		{
			Enabled = false;
		}

		private Boolean _enabled;
		private Boolean _targetEnabled;
		private readonly List<IEquipmentEngagementDependency> _dependencies;
	}
}