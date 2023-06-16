using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class InvalidOutput : UnitPort<IUnitInputPort, IUnitInputPort, InvalidConnection>, IUnitInvalidPort, IUnitOutputPort
    {
        public InvalidOutput(string key) : base(key) { }

        public override IEnumerable<InvalidConnection> validConnections => unit?.graph?.invalidConnections.WithSource(this) ?? Enumerable.Empty<InvalidConnection>();

        public override IEnumerable<InvalidConnection> invalidConnections => Enumerable.Empty<InvalidConnection>();

        public override IEnumerable<IUnitInputPort> validConnectedPorts => validConnections.Select(c => c.destination);

        public override IEnumerable<IUnitInputPort> invalidConnectedPorts => invalidConnections.Select(c => c.destination);

        public override bool CanConnectToValid(IUnitInputPort port)
        {
            return false;
        }

        public override void ConnectToValid(IUnitInputPort port)
        {
            ConnectInvalid(this, port);
        }

        public override void ConnectToInvalid(IUnitInputPort port)
        {
            ConnectInvalid(this, port);
        }

        public override void DisconnectFromValid(IUnitInputPort port)
        {
            DisconnectInvalid(this, port);
        }

        public override void DisconnectFromInvalid(IUnitInputPort port)
        {
            DisconnectInvalid(this, port);
        }

        public override IUnitPort CompatiblePort(IUnit unit)
        {
            return null;
        }
    }
}
