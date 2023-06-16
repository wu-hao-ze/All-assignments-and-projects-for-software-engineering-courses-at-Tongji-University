using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class ControlInput : UnitPort<ControlOutput, IUnitOutputPort, ControlConnection>, IUnitControlPort, IUnitInputPort
    {
        public ControlInput(string key, Func<Flow, ControlOutput> action) : base(key)
        {
            Ensure.That(nameof(action)).IsNotNull(action);

            this.action = action;
        }

        public ControlInput(string key, Func<Flow, IEnumerator> coroutineAction) : base(key)
        {
            Ensure.That(nameof(coroutineAction)).IsNotNull(coroutineAction);

            this.coroutineAction = coroutineAction;
        }

        public ControlInput(string key, Func<Flow, ControlOutput> action, Func<Flow, IEnumerator> coroutineAction) : base(key)
        {
            Ensure.That(nameof(action)).IsNotNull(action);
            Ensure.That(nameof(coroutineAction)).IsNotNull(coroutineAction);

            this.action = action;
            this.coroutineAction = coroutineAction;
        }

        public bool supportsCoroutine => coroutineAction != null;

        public bool requiresCoroutine => action == null;

        internal readonly Func<Flow, ControlOutput> action;

        internal readonly Func<Flow, IEnumerator> coroutineAction;

        public override IEnumerable<ControlConnection> validConnections => unit?.graph?.controlConnections.WithDestination(this) ?? Enumerable.Empty<ControlConnection>();

        public override IEnumerable<InvalidConnection> invalidConnections => unit?.graph?.invalidConnections.WithDestination(this) ?? Enumerable.Empty<InvalidConnection>();

        public override IEnumerable<ControlOutput> validConnectedPorts => validConnections.Select(c => c.source);

        public override IEnumerable<IUnitOutputPort> invalidConnectedPorts => invalidConnections.Select(c => c.source);

        public bool isPredictable
        {
            get
            {
                using (var recursion = Recursion.New(1))
                {
                    return IsPredictable(recursion);
                }
            }
        }

        public bool IsPredictable(Recursion recursion)
        {
            if (!hasValidConnection)
            {
                return true;
            }

            if (!recursion?.TryEnter(this) ?? false)
            {
                return false;
            }

            var isPredictable = validConnectedPorts.All(cop => cop.IsPredictable(recursion));

            recursion?.Exit(this);

            return isPredictable;
        }

        public bool couldBeEntered
        {
            get
            {
                if (!isPredictable)
                {
                    throw new NotSupportedException();
                }

                if (!hasValidConnection)
                {
                    return false;
                }

                return validConnectedPorts.Any(cop => cop.couldBeEntered);
            }
        }

        public override bool CanConnectToValid(ControlOutput port)
        {
            return true;
        }

        public override void ConnectToValid(ControlOutput port)
        {
            var source = port;
            var destination = this;

            source.Disconnect();

            unit.graph.controlConnections.Add(new ControlConnection(source, destination));
        }

        public override void ConnectToInvalid(IUnitOutputPort port)
        {
            ConnectInvalid(port, this);
        }

        public override void DisconnectFromValid(ControlOutput port)
        {
            var connection = validConnections.SingleOrDefault(c => c.source == port);

            if (connection != null)
            {
                unit.graph.controlConnections.Remove(connection);
            }
        }

        public override void DisconnectFromInvalid(IUnitOutputPort port)
        {
            DisconnectInvalid(port, this);
        }

        public override IUnitPort CompatiblePort(IUnit unit)
        {
            if (unit == this.unit) return null;

            return unit.controlOutputs.FirstOrDefault();
        }
    }
}
