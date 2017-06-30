using System;
using HabitableZone.Common;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.Electricity
{
	/// <summary>
	///    Base class for electricity components.
	/// </summary>
	public abstract class ElectricityComponent : EquipmentComponent, IEquipmentEngagementDependency
	{
		protected ElectricityComponent(EquipmentComponentData data, Equipment ownerEquipment) : base(data, ownerEquipment)
		{
			OwnerEquipment.RegisterDependency(this);
		}

		Equipment IEquipmentEngagementDependency.Equipment => OwnerEquipment;

		public Boolean IsEngagementAllowed { get; private set; }
		//Initialization doesn't required: it is guaranteed to be false initially

		public event CEventHandler<IEquipmentEngagementDependency> EngagementAllowed;
		public event CEventHandler<IEquipmentEngagementDependency> EngagementProhibited;

		/// <summary>
		///    Internal property used to couple EquipmentNetwork.Node and ElectricityEquipment
		///    when equipment is involved in the power distribution.
		/// </summary>
		internal EquipmentNetwork.Node ContainingNode
		{
			get { return _containingNode; }
			set
			{
				if (_containingNode != null)
				{
					_containingNode.PowerConfigurationChanged -= OnNodePowerConfigurationChanged;
					_containingNode.EquipmentEngagementAllowed -= OnNodeAllowedEngagement;
					_containingNode.EquipmentEngagementProhibited -= OnNodeProhibitedEngagement;
				}

				_containingNode = value;

				if (_containingNode != null)
				{
					_containingNode.PowerConfigurationChanged += OnNodePowerConfigurationChanged;
					_containingNode.EquipmentEngagementAllowed += OnNodeAllowedEngagement;
					_containingNode.EquipmentEngagementProhibited += OnNodeProhibitedEngagement;
				}
			}
		}

		/// <summary>
		///    Action which performs when node reports that it's input/output power changed.
		/// </summary>
		protected abstract void OnNodePowerConfigurationChanged(EquipmentNetwork.Node sender);

		private void OnNodeAllowedEngagement(EquipmentNetwork.Node sender)
		{
			IsEngagementAllowed = true;
			EngagementAllowed?.Invoke(this);
		}

		private void OnNodeProhibitedEngagement(EquipmentNetwork.Node sender)
		{
			IsEngagementAllowed = false;
			EngagementProhibited?.Invoke(this);
		}

		private EquipmentNetwork.Node _containingNode;
	}
}