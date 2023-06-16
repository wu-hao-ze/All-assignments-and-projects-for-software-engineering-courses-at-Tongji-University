using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class ValueOutput : UnitPort<ValueInput, IUnitInputPort, ValueConnection>, IUnitValuePort, IUnitOutputPort
    {
        public ValueOutput(string key, Type type, Func<Flow, object> getValue) : base(key)
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(getValue)).IsNotNull(getValue);

            this.type = type;
            this.getValue = getValue;
        }

        public ValueOutput(string key, Type type) : base(key)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            this.type = type;
        }

        internal readonly Func<Flow, object> getValue;

        internal Func<Flow, bool> canPredictValue;

        public bool supportsPrediction => canPredictValue != null;

        public bool supportsFetch => getValue != null;

        public Type type { get; }

        public override IEnumerable<ValueConnection> validConnections => unit?.graph?.valueConnections.WithSource(this) ?? Enumerable.Empty<ValueConnection>();

        public override IEnumerable<InvalidConnection> invalidConnections => unit?.graph?.invalidConnections.WithSource(this) ?? Enumerable.Empty<InvalidConnection>();

        public override IEnumerable<ValueInput> validConnectedPorts => validConnections.Select(c => c.destination);

        public override IEnumerable<IUnitInputPort> invalidConnectedPorts => invalidConnections.Select(c => c.destination);

        public override bool CanConnectToValid(ValueInput port)
        {
            var source = this;
            var destination = port;

            return source.type.IsConvertibleTo(destination.type, false);
        }

        public override void ConnectToValid(ValueInput port)
        {
            var source = this;
            var destination = port;

            destination.Disconnect();

            unit.graph.valueConnections.Add(new ValueConnection(source, destination));
        }

        public override void ConnectToInvalid(IUnitInputPort port)
        {
            ConnectInvalid(this, port);
        }

        public override void DisconnectFromValid(ValueInput port)
        {
            var connection = validConnections.SingleOrDefault(c => c.destination == port);

            if (connection != null)
            {
                unit.graph.valueConnections.Remove(connection);
            }
        }

        public override void DisconnectFromInvalid(IUnitInputPort port)
        {
            DisconnectInvalid(this, port);
        }

        public ValueOutput PredictableIf(Func<Flow, bool> condition)
        {
            Ensure.That(nameof(condition)).IsNotNull(condition);

            canPredictValue = condition;

            return this;
        }

        public ValueOutput Predictable()
        {
            canPredictValue = (flow) => true;

            return this;
        }

        public override IUnitPort CompatiblePort(IUnit unit)
        {
            if (unit == this.unit) return null;

            return unit.CompatibleValueInput(type);
        }
    }
}
