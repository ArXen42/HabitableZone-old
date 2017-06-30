using System;
using System.Collections.Generic;
using HabitableZone.Common;
using HabitableZone.Core.World;

namespace HabitableZone.Core.SpacecraftStructure.Hardware
{
	/// <summary>
	///    Base class for all spacecraft equipment.
	/// </summary>
	public abstract partial class Equipment : IWorldContextProvider
	{
		protected Equipment(EquipmentData data)
		{
			Name = data.Name;
			DryMass = data.DryMass;

			TargetEnabled = data.TargetEnabled;

			_components = new Dictionary<Type, EquipmentComponent>();
			_dependencies = new List<IEquipmentEngagementDependency>();
		}

		public WorldContext WorldContext => Spacecraft.WorldContext;

		/// <summary>
		///    Name of this equipment.
		/// </summary>
		public String Name { get; protected set; } //TODO: localize

		public abstract EquipmentData GetSerializationData();

		/// <summary>
		///    Returns component of desired type or null if that component doesn't persists on this equipment.
		/// </summary>
		public T GetComponent<T>() where T : EquipmentComponent
		{
			EquipmentComponent result;
			_components.TryGetValue(typeof(T), out result);
			return result as T;
		}

		/// <summary>
		///    Determines whether a component of desired type persists on this equipment.
		/// </summary>
		public Boolean ContainsComponent<T>() where T : EquipmentComponent
		{
			return GetComponent<T>() != null;
		}

		/// <summary>
		///    Service method called by newly instantiated components.
		/// </summary>
		/// <remarks>
		///    Yes, again "friend" method.
		/// </remarks>
		public void RegisterComponent(EquipmentComponent component)
		{
			Assert.IsTrue(component.OwnerEquipment == this,
				"Tried to register component owned by another equipment.");
			Assert.IsFalse(_components.ContainsValue(component),
				"Tried to register component multiple times.");

			_components.Add(component.GetType(), component);
		}

		private readonly Dictionary<Type, EquipmentComponent> _components;
	}

	[Serializable]
	public abstract class EquipmentData
	{
		protected EquipmentData() { }

		protected EquipmentData(Equipment equipment)
		{
			Name = equipment.Name;
			DryMass = equipment.DryMass;
			TargetEnabled = equipment.TargetEnabled;
		}

		public abstract Equipment GetInstanceFromData();
		public Single DryMass;

		public String Name;
		public Boolean TargetEnabled;
	}
}