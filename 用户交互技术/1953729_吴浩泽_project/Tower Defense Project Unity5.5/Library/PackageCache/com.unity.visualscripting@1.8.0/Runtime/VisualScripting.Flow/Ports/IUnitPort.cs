using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IUnitPort : IGraphItem
    {
        IUnit unit { get; set; }
        string key { get; }

        IEnumerable<IUnitRelation> relations { get; }

        IEnumerable<IUnitConnection> validConnections { get; }
        IEnumerable<InvalidConnection> invalidConnections { get; }
        IEnumerable<IUnitConnection> connections { get; }
        IEnumerable<IUnitPort> connectedPorts { get; }
        bool hasAnyConnection { get; }
        bool hasValidConnection { get; }
        bool hasInvalidConnection { get; }
        bool CanInvalidlyConnectTo(IUnitPort port);
        bool CanValidlyConnectTo(IUnitPort port);
        void InvalidlyConnectTo(IUnitPort port);
        void ValidlyConnectTo(IUnitPort port);
        void Disconnect();
        IUnitPort CompatiblePort(IUnit unit);
    }
}
