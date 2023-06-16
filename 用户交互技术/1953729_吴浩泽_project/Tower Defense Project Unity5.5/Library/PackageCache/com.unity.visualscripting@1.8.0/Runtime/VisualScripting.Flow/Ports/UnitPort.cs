using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public abstract class UnitPort<TValidOther, TInvalidOther, TExternalConnection> : IUnitPort
        where TValidOther : IUnitPort
        where TInvalidOther : IUnitPort
        where TExternalConnection : IUnitConnection
    {
        protected UnitPort(string key)
        {
            Ensure.That(nameof(key)).IsNotNull(key);

            this.key = key;
        }

        public IUnit unit { get; set; }

        public string key { get; }

        public IGraph graph => unit?.graph;

        public IEnumerable<IUnitRelation> relations =>
            LinqUtility.Concat<IUnitRelation>(unit.relations.WithSource(this),
                unit.relations.WithDestination(this)).Distinct();

        public abstract IEnumerable<TExternalConnection> validConnections { get; }

        public abstract IEnumerable<InvalidConnection> invalidConnections { get; }

        public abstract IEnumerable<TValidOther> validConnectedPorts { get; }

        public abstract IEnumerable<TInvalidOther> invalidConnectedPorts { get; }

        IEnumerable<IUnitConnection> IUnitPort.validConnections => validConnections.Cast<IUnitConnection>();

        public IEnumerable<IUnitConnection> connections => LinqUtility.Concat<IUnitConnection>(validConnections, invalidConnections);

        public IEnumerable<IUnitPort> connectedPorts => LinqUtility.Concat<IUnitPort>(validConnectedPorts, invalidConnectedPorts);

        public bool hasAnyConnection => hasValidConnection || hasInvalidConnection;

        // Allow for more efficient overrides

        public virtual bool hasValidConnection => validConnections.Any();

        public virtual bool hasInvalidConnection => invalidConnections.Any();

        private bool CanConnectTo(IUnitPort port)
        {
            Ensure.That(nameof(port)).IsNotNull(port);

            return unit != null && // We belong to a unit
                port.unit != null &&    // Port belongs to a unit
                port.unit != unit &&    // that is different than the current one
                port.unit.graph == unit.graph;    // but is on the same graph.
        }

        public bool CanValidlyConnectTo(IUnitPort port)
        {
            return CanConnectTo(port) && port is TValidOther && CanConnectToValid((TValidOther)port);
        }

        public bool CanInvalidlyConnectTo(IUnitPort port)
        {
            return CanConnectTo(port) && port is TInvalidOther && CanConnectToInvalid((TInvalidOther)port);
        }

        public void ValidlyConnectTo(IUnitPort port)
        {
            Ensure.That(nameof(port)).IsNotNull(port);

            if (!(port is TValidOther))
            {
                throw new InvalidConnectionException();
            }

            ConnectToValid((TValidOther)port);
        }

        public void InvalidlyConnectTo(IUnitPort port)
        {
            Ensure.That(nameof(port)).IsNotNull(port);

            if (!(port is TInvalidOther))
            {
                throw new InvalidConnectionException();
            }

            ConnectToInvalid((TInvalidOther)port);
        }

        public void Disconnect()
        {
            while (validConnectedPorts.Any())
            {
                DisconnectFromValid(validConnectedPorts.First());
            }

            while (invalidConnectedPorts.Any())
            {
                DisconnectFromInvalid(invalidConnectedPorts.First());
            }
        }

        public abstract bool CanConnectToValid(TValidOther port);

        public bool CanConnectToInvalid(TInvalidOther port)
        {
            return true;
        }

        public abstract void ConnectToValid(TValidOther port);

        public abstract void ConnectToInvalid(TInvalidOther port);

        public abstract void DisconnectFromValid(TValidOther port);

        public abstract void DisconnectFromInvalid(TInvalidOther port);

        public abstract IUnitPort CompatiblePort(IUnit unit);

        protected void ConnectInvalid(IUnitOutputPort source, IUnitInputPort destination)
        {
            var connection = unit.graph.invalidConnections.SingleOrDefault(c => c.source == source && c.destination == destination);

            if (connection != null)
            {
                return;
            }

            unit.graph.invalidConnections.Add(new InvalidConnection(source, destination));
        }

        protected void DisconnectInvalid(IUnitOutputPort source, IUnitInputPort destination)
        {
            var connection = unit.graph.invalidConnections.SingleOrDefault(c => c.source == source && c.destination == destination);

            if (connection == null)
            {
                return;
            }

            unit.graph.invalidConnections.Remove(connection);
        }
    }
}
