using System;

namespace Unity.VisualScripting
{
    public sealed class ControlConnection : UnitConnection<ControlOutput, ControlInput>, IUnitConnection
    {
        [Obsolete(Serialization.ConstructorWarning)]
        public ControlConnection() : base() { }

        public ControlConnection(ControlOutput source, ControlInput destination) : base(source, destination)
        {
            if (source.hasValidConnection)
            {
                throw new InvalidConnectionException("Control output ports do not support multiple connections.");
            }
        }

        #region Ports

        public override ControlOutput source => sourceUnit.controlOutputs[sourceKey];

        public override ControlInput destination => destinationUnit.controlInputs[destinationKey];

        IUnitOutputPort IConnection<IUnitOutputPort, IUnitInputPort>.source => source;

        IUnitInputPort IConnection<IUnitOutputPort, IUnitInputPort>.destination => destination;

        #endregion

        #region Dependencies

        public override bool sourceExists => sourceUnit.controlOutputs.Contains(sourceKey);

        public override bool destinationExists => destinationUnit.controlInputs.Contains(destinationKey);

        #endregion
    }
}
