using System;

namespace Unity.VisualScripting
{
    public sealed class ValueConnection : UnitConnection<ValueOutput, ValueInput>, IUnitConnection
    {
        public class DebugData : UnitConnectionDebugData
        {
            public object lastValue { get; set; }

            public bool assignedLastValue { get; set; }
        }

        public override IGraphElementDebugData CreateDebugData()
        {
            return new DebugData();
        }

        [Obsolete(Serialization.ConstructorWarning)]
        public ValueConnection() : base() { }

        public ValueConnection(ValueOutput source, ValueInput destination) : base(source, destination)
        {
            if (destination.hasValidConnection)
            {
                throw new InvalidConnectionException("Value input ports do not support multiple connections.");
            }

            if (!source.type.IsConvertibleTo(destination.type, false))
            {
                throw new InvalidConnectionException($"Cannot convert from '{source.type}' to '{destination.type}'.");
            }
        }

        #region Ports

        public override ValueOutput source => sourceUnit.valueOutputs[sourceKey];

        public override ValueInput destination => destinationUnit.valueInputs[destinationKey];

        IUnitOutputPort IConnection<IUnitOutputPort, IUnitInputPort>.source => source;

        IUnitInputPort IConnection<IUnitOutputPort, IUnitInputPort>.destination => destination;

        #endregion

        #region Dependencies

        public override bool sourceExists => sourceUnit.valueOutputs.Contains(sourceKey);

        public override bool destinationExists => destinationUnit.valueInputs.Contains(destinationKey);

        #endregion
    }
}
