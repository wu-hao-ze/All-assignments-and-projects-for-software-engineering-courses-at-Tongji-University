using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class ControlOutput : UnitPort<ControlInput, IUnitInputPort, ControlConnection>, IUnitControlPort, IUnitOutputPort
    {
        public ControlOutput(string key) : base(key) { }

        public override IEnumerable<ControlConnection> validConnections => unit?.graph?.controlConnections.WithSource(this) ?? Enumerable.Empty<ControlConnection>();

        public override IEnumerable<InvalidConnection> invalidConnections => unit?.graph?.invalidConnections.WithSource(this) ?? Enumerable.Empty<InvalidConnection>();

        public override IEnumerable<ControlInput> validConnectedPorts => validConnections.Select(c => c.destination);

        public override IEnumerable<IUnitInputPort> invalidConnectedPorts => invalidConnections.Select(c => c.destination);

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
            if (unit.isControlRoot)
            {
                return true;
            }

            if (!recursion?.TryEnter(this) ?? false)
            {
                return false;
            }

            var isPredictable = unit.relations.WithDestination(this).Where(r => r.source is ControlInput).All(r => ((ControlInput)r.source).IsPredictable(recursion));

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

                if (unit.isControlRoot)
                {
                    return true;
                }

                return unit.relations.WithDestination(this).Where(r => r.source is ControlInput).Any(r => ((ControlInput)r.source).couldBeEntered);
            }
        }

        public ControlConnection connection => unit.graph?.controlConnections.SingleOrDefaultWithSource(this);

        public override bool hasValidConnection => connection != null;

        public override bool CanConnectToValid(ControlInput port)
        {
            return true;
        }

        public override void ConnectToValid(ControlInput port)
        {
            var source = this;
            var destination = port;

            source.Disconnect();

            unit.graph.controlConnections.Add(new ControlConnection(source, destination));
        }

        public override void ConnectToInvalid(IUnitInputPort port)
        {
            ConnectInvalid(this, port);
        }

        public override void DisconnectFromValid(ControlInput port)
        {
            var connection = validConnections.SingleOrDefault(c => c.destination == port);

            if (connection != null)
            {
                unit.graph.controlConnections.Remove(connection);
            }
        }

        public override void DisconnectFromInvalid(IUnitInputPort port)
        {
            DisconnectInvalid(this, port);
        }

        public override IUnitPort CompatiblePort(IUnit unit)
        {
            if (unit == this.unit) return null;

            return unit.controlInputs.FirstOrDefault();
        }
    }
}
