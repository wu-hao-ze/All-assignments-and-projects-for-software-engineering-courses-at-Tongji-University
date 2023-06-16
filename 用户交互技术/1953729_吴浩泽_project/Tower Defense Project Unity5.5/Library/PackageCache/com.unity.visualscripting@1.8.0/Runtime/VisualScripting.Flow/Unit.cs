using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [SerializationVersion("A")]
    public abstract class Unit : GraphElement<FlowGraph>, IUnit
    {
        public class DebugData : IUnitDebugData
        {
            public int lastInvokeFrame { get; set; }

            public float lastInvokeTime { get; set; }

            public Exception runtimeException { get; set; }
        }

        protected Unit() : base()
        {
            controlInputs = new UnitPortCollection<ControlInput>(this);
            controlOutputs = new UnitPortCollection<ControlOutput>(this);
            valueInputs = new UnitPortCollection<ValueInput>(this);
            valueOutputs = new UnitPortCollection<ValueOutput>(this);
            invalidInputs = new UnitPortCollection<InvalidInput>(this);
            invalidOutputs = new UnitPortCollection<InvalidOutput>(this);

            relations = new ConnectionCollection<IUnitRelation, IUnitPort, IUnitPort>();

            defaultValues = new Dictionary<string, object>();
        }

        public virtual IGraphElementDebugData CreateDebugData()
        {
            return new DebugData();
        }

        public override void AfterAdd()
        {
            // Important to define before notifying instances
            Define();

            base.AfterAdd();
        }

        public override void BeforeRemove()
        {
            base.BeforeRemove();

            Disconnect();
        }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            if (this is IGraphEventListener listener && XGraphEventListener.IsHierarchyListening(instance))
            {
                listener.StartListening(instance);
            }
        }

        public override void Uninstantiate(GraphReference instance)
        {
            if (this is IGraphEventListener listener)
            {
                listener.StopListening(instance);
            }

            base.Uninstantiate(instance);
        }

        #region Poutine

        protected void CopyFrom(Unit source)
        {
            base.CopyFrom(source);

            defaultValues = source.defaultValues;
        }

        #endregion

        #region Definition

        [DoNotSerialize]
        public virtual bool canDefine => true;

        [DoNotSerialize]
        public bool failedToDefine => definitionException != null;

        [DoNotSerialize]
        public bool isDefined { get; private set; }

        protected abstract void Definition();

        protected virtual void AfterDefine() { }

        protected virtual void BeforeUndefine() { }

        private void Undefine()
        {
            // Because a node is always undefined on definition,
            // even if it wasn't defined before, we make sure the user
            // code for undefinition can safely presume it was defined.
            if (isDefined)
            {
                BeforeUndefine();
            }

            Disconnect();
            defaultValues.Clear();
            controlInputs.Clear();
            controlOutputs.Clear();
            valueInputs.Clear();
            valueOutputs.Clear();
            invalidInputs.Clear();
            invalidOutputs.Clear();
            relations.Clear();
            isDefined = false;
        }

        public void EnsureDefined()
        {
            if (!isDefined)
            {
                Define();
            }
        }

        public void Define()
        {
            var preservation = UnitPreservation.Preserve(this);

            // A node needs to undefine even if it wasn't defined,
            // because there might be invalid ports and connections
            // that we need to clear to avoid duplicates on definition.
            Undefine();

            if (canDefine)
            {
                try
                {
                    Definition();
                    isDefined = true;
                    definitionException = null;
                    AfterDefine();
                }
                catch (Exception ex)
                {
                    Undefine();
                    definitionException = ex;
                    Debug.LogWarning($"Failed to define {this}:\n{ex}");
                }
            }

            preservation.RestoreTo(this);
        }

        public void RemoveUnconnectedInvalidPorts()
        {
            foreach (var unconnectedInvalidInput in invalidInputs.Where(p => !p.hasAnyConnection).ToArray())
            {
                invalidInputs.Remove(unconnectedInvalidInput);
            }

            foreach (var unconnectedInvalidOutput in invalidOutputs.Where(p => !p.hasAnyConnection).ToArray())
            {
                invalidOutputs.Remove(unconnectedInvalidOutput);
            }
        }

        #endregion

        #region Ports

        [DoNotSerialize]
        public IUnitPortCollection<ControlInput> controlInputs { get; }

        [DoNotSerialize]
        public IUnitPortCollection<ControlOutput> controlOutputs { get; }

        [DoNotSerialize]
        public IUnitPortCollection<ValueInput> valueInputs { get; }

        [DoNotSerialize]
        public IUnitPortCollection<ValueOutput> valueOutputs { get; }

        [DoNotSerialize]
        public IUnitPortCollection<InvalidInput> invalidInputs { get; }

        [DoNotSerialize]
        public IUnitPortCollection<InvalidOutput> invalidOutputs { get; }

        [DoNotSerialize]
        public IEnumerable<IUnitInputPort> inputs => LinqUtility.Concat<IUnitInputPort>(controlInputs, valueInputs, invalidInputs);

        [DoNotSerialize]
        public IEnumerable<IUnitOutputPort> outputs => LinqUtility.Concat<IUnitOutputPort>(controlOutputs, valueOutputs, invalidOutputs);

        [DoNotSerialize]
        public IEnumerable<IUnitInputPort> validInputs => LinqUtility.Concat<IUnitInputPort>(controlInputs, valueInputs);

        [DoNotSerialize]
        public IEnumerable<IUnitOutputPort> validOutputs => LinqUtility.Concat<IUnitOutputPort>(controlOutputs, valueOutputs);

        [DoNotSerialize]
        public IEnumerable<IUnitPort> ports => LinqUtility.Concat<IUnitPort>(inputs, outputs);

        [DoNotSerialize]
        public IEnumerable<IUnitPort> invalidPorts => LinqUtility.Concat<IUnitPort>(invalidInputs, invalidOutputs);

        [DoNotSerialize]
        public IEnumerable<IUnitPort> validPorts => LinqUtility.Concat<IUnitPort>(validInputs, validOutputs);

        public event Action onPortsChanged;

        public void PortsChanged()
        {
            onPortsChanged?.Invoke();
        }

        #endregion

        #region Default Values

        [Serialize]
        public Dictionary<string, object> defaultValues { get; private set; }

        #endregion

        #region Connections

        [DoNotSerialize]
        public IConnectionCollection<IUnitRelation, IUnitPort, IUnitPort> relations { get; private set; }

        [DoNotSerialize]
        public IEnumerable<IUnitConnection> connections => ports.SelectMany(p => p.connections);

        public void Disconnect()
        {
            // Can't use a foreach because invalid ports may get removed as they disconnect
            while (ports.Any(p => p.hasAnyConnection))
            {
                ports.First(p => p.hasAnyConnection).Disconnect();
            }
        }

        #endregion

        #region Analysis

        [DoNotSerialize]
        public virtual bool isControlRoot { get; protected set; } = false;

        #endregion

        #region Helpers

        protected void EnsureUniqueInput(string key)
        {
            if (controlInputs.Contains(key) || valueInputs.Contains(key) || invalidInputs.Contains(key))
            {
                throw new ArgumentException($"Duplicate input for '{key}' in {GetType()}.");
            }
        }

        protected void EnsureUniqueOutput(string key)
        {
            if (controlOutputs.Contains(key) || valueOutputs.Contains(key) || invalidOutputs.Contains(key))
            {
                throw new ArgumentException($"Duplicate output for '{key}' in {GetType()}.");
            }
        }

        protected ControlInput ControlInput(string key, Func<Flow, ControlOutput> action)
        {
            EnsureUniqueInput(key);
            var port = new ControlInput(key, action);
            controlInputs.Add(port);
            return port;
        }

        protected ControlInput ControlInputCoroutine(string key, Func<Flow, IEnumerator> coroutineAction)
        {
            EnsureUniqueInput(key);
            var port = new ControlInput(key, coroutineAction);
            controlInputs.Add(port);
            return port;
        }

        protected ControlInput ControlInputCoroutine(string key, Func<Flow, ControlOutput> action, Func<Flow, IEnumerator> coroutineAction)
        {
            EnsureUniqueInput(key);
            var port = new ControlInput(key, action, coroutineAction);
            controlInputs.Add(port);
            return port;
        }

        protected ControlOutput ControlOutput(string key)
        {
            EnsureUniqueOutput(key);
            var port = new ControlOutput(key);
            controlOutputs.Add(port);
            return port;
        }

        protected ValueInput ValueInput(Type type, string key)
        {
            EnsureUniqueInput(key);
            var port = new ValueInput(key, type);
            valueInputs.Add(port);
            return port;
        }

        protected ValueInput ValueInput<T>(string key)
        {
            return ValueInput(typeof(T), key);
        }

        protected ValueInput ValueInput<T>(string key, T @default)
        {
            var port = ValueInput<T>(key);
            port.SetDefaultValue(@default);
            return port;
        }

        protected ValueOutput ValueOutput(Type type, string key)
        {
            EnsureUniqueOutput(key);
            var port = new ValueOutput(key, type);
            valueOutputs.Add(port);
            return port;
        }

        protected ValueOutput ValueOutput(Type type, string key, Func<Flow, object> getValue)
        {
            EnsureUniqueOutput(key);
            var port = new ValueOutput(key, type, getValue);
            valueOutputs.Add(port);
            return port;
        }

        protected ValueOutput ValueOutput<T>(string key)
        {
            return ValueOutput(typeof(T), key);
        }

        protected ValueOutput ValueOutput<T>(string key, Func<Flow, T> getValue)
        {
            return ValueOutput(typeof(T), key, (recursion) => getValue(recursion));
        }

        private void Relation(IUnitPort source, IUnitPort destination)
        {
            relations.Add(new UnitRelation(source, destination));
        }

        /// <summary>
        /// Triggering the destination may fetch the source value.
        /// </summary>
        protected void Requirement(ValueInput source, ControlInput destination)
        {
            Relation(source, destination);
        }

        /// <summary>
        /// Getting the value of the destination may fetch the value of the source.
        /// </summary>
        protected void Requirement(ValueInput source, ValueOutput destination)
        {
            Relation(source, destination);
        }

        /// <summary>
        /// Triggering the source may assign the destination value on the flow.
        /// </summary>
        protected void Assignment(ControlInput source, ValueOutput destination)
        {
            Relation(source, destination);
        }

        /// <summary>
        /// Triggering the source may trigger the destination.
        /// </summary>
        protected void Succession(ControlInput source, ControlOutput destination)
        {
            Relation(source, destination);
        }

        #endregion

        #region Widget

        [Serialize]
        public Vector2 position { get; set; }

        [DoNotSerialize]
        public Exception definitionException { get; protected set; }

        #endregion

        #region Analytics

        public override AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            var aid = new AnalyticsIdentifier
            {
                Identifier = GetType().FullName,
                Namespace = GetType().Namespace,
            };
            aid.Hashcode = aid.Identifier.GetHashCode();
            return aid;
        }

        #endregion
    }
}
