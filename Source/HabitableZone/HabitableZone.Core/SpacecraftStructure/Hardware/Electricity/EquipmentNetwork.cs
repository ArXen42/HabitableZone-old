using System;
using HabitableZone.Common;
using UnityEngine;

namespace HabitableZone.Core.SpacecraftStructure.Hardware.Electricity
{
	/// <summary>
	///    The core of ElectricitySubsystem, controls electrical state of equipment in it.
	/// </summary>
	/// <remarks>
	///    Internally uses implementation of double linked list similar to LinkedList,
	///    which actually is doubly-linked circular list.
	///    Additionally introduces weighted edges between nodes corresponding to input and output power.
	/// </remarks>
	public sealed class EquipmentNetwork
	{
		public override String ToString()
		{
			String str = String.Empty;

			var iteratingNode = First;
			str += iteratingNode.InputPower;
			do
			{
				str += " - " + iteratingNode.Equipment.Name + " - " + iteratingNode.OutputPower;
				iteratingNode = iteratingNode.Next;
			} while (iteratingNode != null);

			return str;
		}

		/// <summary>
		///    Occurs when some node's input power has changed.
		/// </summary>
		/// <remarks>
		///    Usually changing some node's input power causing chain reaction.
		///    This event is called once only after new power state will be fully established and carries initial node.
		/// </remarks>
		public event CEventHandler<EquipmentNetwork> PowerStateChanged;

		/// <summary>
		///    Gets the number of nodes actually contained in the network (or however it can be called).
		/// </summary>
		public Int32 Count { get; private set; }

		/// <summary>
		///    Gets the first node.
		/// </summary>
		public Node First { get; private set; }

		/// <summary>
		///    Gets the last node.
		/// </summary>
		public Node Last => First?._prev;

		/// <summary>
		///    Used to easily determine overall producing power.
		/// </summary>
		/// <remarks>
		///    It's output power actually is the overall producing power.
		///    But if the chain formation (producers before consumers) will be changed this would broke.
		/// </remarks>
		public Node LastProducerNode { get; private set; }

		/// <summary>
		///    Finds node associated with given equipment.
		/// </summary>
		public Node Find(Equipment equipment)
		{
			var node = First;
			if (node == null) return null;

			do
			{
				if (node.Equipment == equipment) return node;
				node = node._next;
			} while (node != First);

			return null;
		}

		/// <summary>
		///    Inserts equipment in chain. Position calculates automatically based on it's priority.
		/// </summary>
		/// <remarks>
		///    EquipmentNetwork doesn't track PriorityChanged by itself. Electricity subsystem is responsible for this.
		///    Normally this is called only by ElectricitySubsystem in response to TargetEnabledChanged or PriorityChanged.
		/// </remarks>
		public Node InsertEquipment(Equipment equipment)
		{
			var node = Node.CreateNode(this, equipment);
			if (First == null)
			{
				InsertNodeToEmptyList(node);
				return node;
			}

			Int16 priority = node.Priority;
			var iteratingNode = First;
			while (iteratingNode.Next != null && iteratingNode.Next.Priority <= priority)
				iteratingNode = iteratingNode._next;

			if (iteratingNode.Priority > priority)
				InsertBefore(iteratingNode, node);
			else
				InsertAfter(iteratingNode, node);

			return node;
		}

		/// <summary>
		///    Removes equipment from chain.
		/// </summary>
		/// <remarks>
		///    Normally this is called only by ElectricitySubsystem in response to TargetEnabledChanged or PriorityChanged.
		/// </remarks>
		public void RemoveEquipment(Equipment equipment)
		{
			RemoveNode(Find(equipment));
		}

		/// <summary>
		///    Determines whether some node started a chain of InputPower/OutputPower properties changes.
		///    This property is used to guarantee calling PowerStateChanged only at the end of the process.
		/// </summary>
		private Boolean PowerRedistributionInProcess
		{
			get { return _powerRedistributionInProcess; }
			set
			{
				Assert.IsFalse(!value && !_powerRedistributionInProcess,
					"This shouldn't be setted to false twice because it indicates the end of power changes cascade.");

				if (!value && PowerStateChanged != null)
					PowerStateChanged.Invoke(this); //Invoking power state changed at the end of changes cascade.

				_powerRedistributionInProcess = value;
			}
		}

		/// <summary>
		///    Removes node from internal linked list.
		/// </summary>
		private void RemoveNode(Node node)
		{
			Assert.IsFalse(First == null, "This method shouldn't be called on empty list.");
			Assert.IsFalse(node == null, "Attempted to remove null node.");

			node.HandleRemoval();

			Assert.IsFalse(node.Equipment.Enabled,
				"Equipment had to be turned off before removing it from the EquipmentNetwork");

			if (node._next == node)
			{
				Assert.IsTrue(Count == 1 && First == node);
				First = null;
			}
			else
			{
				node._next._prev = node._prev;
				node._prev._next = node._next;

				if (First == node)
					First = node._next;
			}

			node.Invalidate();
			Count--;
		}

		/// <summary>
		///    Inserts new node before given.
		/// </summary>
		/// <remarks>
		///    As in LinkedList it just invokes InternalInsertBefore and changes _head if needed.
		/// </remarks>
		private void InsertBefore(Node node, Node newNode)
		{
			InternalInsertBefore(node, newNode);

			if (node == First)
				First = newNode;

			newNode.HandleInsertion();
		}

		/// <summary>
		///    Inserts new node after given.
		/// </summary>
		/// <remarks>
		///    As in LinkedList, it just invokes InternalInsertBefore to the _next (not Next!).
		/// </remarks>
		private void InsertAfter(Node node, Node newNode)
		{
			InternalInsertBefore(node._next, newNode);
			newNode.HandleInsertion();
		}

		/// <summary>
		///    Doing actual insertion of new node.
		/// </summary>
		private void InternalInsertBefore(Node node, Node newNode)
		{
			newNode._next = node;
			newNode._prev = node._prev;
			node._prev._next = newNode;
			node._prev = newNode;
			Count++;
		}

		/// <summary>
		///    This method is called when the first node arrives.
		/// </summary>
		private void InsertNodeToEmptyList(Node node)
		{
			Assert.IsTrue(First == null && Count == 0, "EquipmentNetwork must be empty when this method is called.");
			node._next = node;
			node._prev = node;
			First = node;
			Count++;

			node.HandleInsertion();
		}

		//We don't need PowerStateChanged to occur before the new power state will be completely established,
		//so we have this field and property.
		private Boolean _powerRedistributionInProcess;

		/// <summary>
		///    Node of EquipmentNetwork. Shouldn't be created manually or somewhere outside EquipmentNetwork.
		/// </summary>
		public abstract class Node
		{
			/// <summary>
			///    Factory method that creates a node corresponding to equipment's electricity components.
			/// </summary>
			public static Node CreateNode(EquipmentNetwork equipmentNetwork, Equipment equipment)
			{
				var producer = equipment.GetComponent<ElectricityProducer>();
				var consumer = equipment.GetComponent<ElectricityConsumer>();

				if (producer == null && consumer == null)
					throw new ArgumentException("Attempted to create node for non-electrical equipment");
				if (producer != null && consumer == null) return new ProducerNode(equipmentNetwork, equipment);
				if (producer == null && consumer != null) return new ConsumerNode(equipmentNetwork, equipment);

				throw new ArgumentException("Electricity subsystem currently doesn't support electricity " +
													 "consumer and producer being on equipment simultaneously. " +
													 "If it's really needed, implement desired behaviour.");
			}

			/// <summary>
			///    Constructs new node and subscribes for Equipment.EnabledChanged.
			/// </summary>
			protected Node(EquipmentNetwork equipmentNetwork, Equipment equipment) //It's private
			{
				Assert.IsTrue(equipment.TargetEnabled, "Tried to create node that \"don't want\" to be enabled.");

				EquipmentNetwork = equipmentNetwork;
				Equipment = equipment;

				equipment.EnabledChanged += OnEquipmentEnabledChanged;
			}

			/// <summary>
			///    Occurs when the input (and probably output) power has been changed.
			/// </summary>
			/// <remarks>
			///    Occurs only when the correct power state of node is established
			///    (not between changes of InputPower and OutputPower, for example).
			/// </remarks>
			public event CEventHandler<Node> PowerConfigurationChanged;

			/// <summary>
			///    Occures when equipment engagement becomes allowed by this subsystem.
			/// </summary>
			public event CEventHandler<Node> EquipmentEngagementAllowed;

			/// <summary>
			///    Occures when equipment no longer can be enabled (or should be immediately disabled).
			/// </summary>
			public event CEventHandler<Node> EquipmentEngagementProhibited;

			/// <summary>
			///    Gets the next node in chain.
			/// </summary>
			public Node Next => _next == null || _next == EquipmentNetwork.First ? null : _next;

			/// <summary>
			///    Gets the previous node in chain.
			/// </summary>
			public Node Previous => _prev == null || this == EquipmentNetwork.First ? null : _prev;

			/// <summary>
			///    EquipmentNetwork this node belongs to.
			/// </summary>
			public EquipmentNetwork EquipmentNetwork { get; private set; }

			/// <summary>
			///    Equipment contained in this node.
			/// </summary>
			public Equipment Equipment { get; private set; }

			/// <summary>
			///    Actual priority of this node. -2 for non-electrical,
			///    -1 for producers, ElectricityConsumer.Priority for consumers.
			/// </summary>
			public abstract Int16 Priority { get; }

			/// <summary>
			///    Left edge of this node. Determines how much power this node can use.
			///    It is equal to output power of previous node (or 0 if this is the first node).
			/// </summary>
			/// <remarks>
			///    It would be really bad idea to change this from outside.
			/// </remarks>
			public Int64 InputPower
			{
				get { return _inputPower; }
				internal set
				{
					Boolean hasStartedPowerRedistribution = !EquipmentNetwork.PowerRedistributionInProcess;
					EquipmentNetwork.PowerRedistributionInProcess = true;

					_inputPower = value;
					OnInputPowerChanged();

					if (hasStartedPowerRedistribution)
						EquipmentNetwork.PowerRedistributionInProcess = false;
				}
			}

			/// <summary>
			///    Right edge of this node. Determines how much power this node left for the following.
			///    It is equal to input power of the next node (if exists).
			/// </summary>
			public Int64 OutputPower
			{
				get { return _outputPower; }
				protected set
				{
					if (_outputPower == value) return;

					Boolean hasStartedPowerRedistribution = !EquipmentNetwork.PowerRedistributionInProcess;
					EquipmentNetwork.PowerRedistributionInProcess = true;

					_outputPower = value;
					if (Next != null) Next.InputPower = value;

					if (hasStartedPowerRedistribution)
						EquipmentNetwork.PowerRedistributionInProcess = false;

					Assert.IsTrue(Equipment.Enabled == (_inputPower != _outputPower));
				}
			}

			/// <summary>
			///    Internal method called when node is inserted to the chain.
			/// </summary>
			internal void HandleInsertion() //It's private
			{
				Assert.IsFalse(Equipment.Enabled, "Equipment should be disabled at this point.");

				OnInsertion(); //This should be placed before initial power settings because of components subscription

				InputPower = Previous?.OutputPower ?? 0;
			}

			/// <summary>
			///    Internal method called before actual node removal from the chain.
			/// </summary>
			internal void HandleRemoval() //It's private
			{
				ProhibitEngagement();
				OnRemoval();
				Equipment.EnabledChanged -= OnEquipmentEnabledChanged;
			}

			/// <summary>
			///    Internal method called immediately after node is removed from the chain (HandleRemoval - before).
			///    Clears the node's fields so it's become invalid.
			/// </summary>
			internal void Invalidate() //It's private
			{
				EquipmentNetwork = null;
				Equipment = null;
				_next = null;
				_prev = null;
			}

			internal Node _next; //It's private
			internal Node _prev; //It's private

			/// <summary>
			///    This is called after the node has been inserted to the network.
			///    Override if you need additional behaviour after HandleInsertion.
			/// </summary>
			protected virtual void OnInsertion() { }

			/// <summary>
			///    This is called before node invalidation (caused by removal).
			///    Override if you need to do some changes before the final deconstruction of the node.
			/// </summary>
			protected virtual void OnRemoval() { }

			/// <summary>
			///    This is called when InputPower has changed.
			/// </summary>
			protected abstract void OnInputPowerChanged();

			/// <summary>
			///    This is called when equipment has been enabled (immediately after AllowEngagement() or somewhen in the future.
			/// </summary>
			protected abstract void OnEquipmentEnabledChanged(Equipment sender, Boolean value);

			/// <summary>
			///    This is called by derived classes when they determines that equipment can be enabled.
			/// </summary>
			protected void AllowEngagement()
			{
				EquipmentEngagementAllowed?.Invoke(this);
			}

			/// <summary>
			///    This is called by derived classes when they determines that equipment can no longer be enabled
			///    (and should be powered off if it is).
			/// </summary>
			protected void ProhibitEngagement()
			{
				EquipmentEngagementProhibited?.Invoke(this);
			}

			protected void InvokePowerConfigurationChanged()
			{
				PowerConfigurationChanged?.Invoke(this);
			}

			private Int64 _inputPower;
			private Int64 _outputPower;
		}

		/// <summary>
		///    Represents a node containing electricity producer.
		/// </summary>
		public class ProducerNode : Node
		{
			internal ProducerNode(EquipmentNetwork equipmentNetwork, Equipment equipment)
				: base(equipmentNetwork, equipment)
			{
				ProducerComponent = equipment.GetComponent<ElectricityProducer>();
				Assert.IsNotNull(ProducerComponent);
				Assert.IsNull(equipment.GetComponent<ElectricityConsumer>());
			}

			public override Int16 Priority => -1;

			public readonly ElectricityProducer ProducerComponent;

			protected override void OnInsertion()
			{
				ProducerComponent.ContainingNode = this;
				EquipmentNetwork.LastProducerNode = this;

				ProducerComponent.TargetProducingPowerChanged += OnTargetProducingPowerChanged;
			}

			protected override void OnRemoval()
			{
				EquipmentNetwork.LastProducerNode = Previous as ProducerNode; //null is correct value in this case
				ProducerComponent.ContainingNode = null;

				ProducerComponent.TargetProducingPowerChanged -= OnTargetProducingPowerChanged;

				Assert.IsTrue(ProducerComponent.ProducingPower == 0,
					"Equipment was removed from network but ProducerComponent.ProducingPower isn't equal to zero.");
			}

			protected override void OnInputPowerChanged()
			{
				UpdateOutputPower();

				if (!Equipment.Enabled) AllowEngagement();
			}

			protected override void OnEquipmentEnabledChanged(Equipment sender, Boolean value)
			{
				if (!value && Equipment.TargetEnabled)
					Debug.LogWarning("Some producer has been disabled while his TargetEnabled was true. " +
										  "This is supported behaviour, but check if it's not a bug and shutdown " +
										  "was caused by non-electrical reasons (or were it intended changes in electrical logic?).");

				UpdateOutputPower();
			}

			private void OnTargetProducingPowerChanged(ElectricityProducer sender, Int64 newValue)
			{
				Assert.IsTrue(sender == ProducerComponent);
				UpdateOutputPower();
			}

			private void UpdateOutputPower()
			{
				Int64 delta = Equipment.Enabled ? ProducerComponent.TargetProducingPower : 0;
				OutputPower = InputPower + delta;

				InvokePowerConfigurationChanged();
			}
		}

		/// <summary>
		///    Represents a node containing electricity consumer.
		/// </summary>
		public class ConsumerNode : Node
		{
			internal ConsumerNode(EquipmentNetwork equipmentNetwork, Equipment equipment)
				: base(equipmentNetwork, equipment)
			{
				ConsumerComponent = equipment.GetComponent<ElectricityConsumer>();
				Assert.IsNotNull(ConsumerComponent);
				Assert.IsNull(equipment.GetComponent<ElectricityProducer>());
			}

			public override Int16 Priority => ConsumerComponent.Priority;

			public readonly ElectricityConsumer ConsumerComponent;

			protected override void OnInsertion()
			{
				ConsumerComponent.ContainingNode = this;

				ConsumerComponent.TargetConsumingPowerChanged += OnTargetConsumingPowerChanged;
			}

			protected override void OnRemoval()
			{
				ConsumerComponent.ContainingNode = null;
				ConsumerComponent.TargetConsumingPowerChanged -= OnTargetConsumingPowerChanged;

				Assert.IsTrue(ConsumerComponent.ConsumingPower == 0,
					"Equipment was removed from network but ConsumerComponent.ConsumingPower isn't equal to zero.");
			}

			protected override void OnInputPowerChanged()
			{
				if (ConsumerComponent.MinPower <= InputPower)
				{
					if (Equipment.Enabled)
					{
						UpdateOutputPower(true);
					}
					else
					{
						OutputPower = InputPower;
						InvokePowerConfigurationChanged();

						AllowEngagement();
					}
				}
				else
				{
					if (Equipment.Enabled)
					{
						ProhibitEngagement();

						Assert.IsFalse(Equipment.Enabled);
						Assert.IsTrue(InputPower == OutputPower, "Equipment disabled, but output power isn't equal to input.");
					}
					else
					{
						UpdateOutputPower(false);
					}
				}
			}

			protected override void OnEquipmentEnabledChanged(Equipment sender, Boolean value)
			{
				UpdateOutputPower(value);
			}

			private void OnTargetConsumingPowerChanged(ElectricityConsumer sender, Int64 newValue)
			{
				Assert.IsTrue(sender == ConsumerComponent);
				UpdateOutputPower(Equipment.Enabled);
			}

			private void UpdateOutputPower(Boolean enabled)
			{
				if (enabled)
				{
					Assert.IsTrue(InputPower >= ConsumerComponent.MinPower, "Engagement was allowed but there is not enough power.");

					Int64 targetConsumingPower = ConsumerComponent.TargetConsumingPower;
					OutputPower = InputPower > targetConsumingPower
						? InputPower - targetConsumingPower
						: 0;
				}
				else
				{
					OutputPower = InputPower;
				}

				InvokePowerConfigurationChanged();
			}
		}
	}
}