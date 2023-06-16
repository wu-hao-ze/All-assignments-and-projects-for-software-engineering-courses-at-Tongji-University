using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IUnit : IGraphElementWithDebugData
    {
        new FlowGraph graph { get; }

        #region Definition

        bool canDefine { get; }

        bool isDefined { get; }

        bool failedToDefine { get; }

        Exception definitionException { get; }

        void Define();

        void EnsureDefined();

        void RemoveUnconnectedInvalidPorts();

        #endregion

        #region Default Values

        Dictionary<string, object> defaultValues { get; }

        #endregion

        #region Ports

        IUnitPortCollection<ControlInput> controlInputs { get; }

        IUnitPortCollection<ControlOutput> controlOutputs { get; }

        IUnitPortCollection<ValueInput> valueInputs { get; }

        IUnitPortCollection<ValueOutput> valueOutputs { get; }

        IUnitPortCollection<InvalidInput> invalidInputs { get; }

        IUnitPortCollection<InvalidOutput> invalidOutputs { get; }

        IEnumerable<IUnitInputPort> inputs { get; }

        IEnumerable<IUnitOutputPort> outputs { get; }

        IEnumerable<IUnitInputPort> validInputs { get; }

        IEnumerable<IUnitOutputPort> validOutputs { get; }

        IEnumerable<IUnitPort> ports { get; }

        IEnumerable<IUnitPort> invalidPorts { get; }

        IEnumerable<IUnitPort> validPorts { get; }

        void PortsChanged();

        event Action onPortsChanged;

        #endregion

        #region Connections

        IConnectionCollection<IUnitRelation, IUnitPort, IUnitPort> relations { get; }

        IEnumerable<IUnitConnection> connections { get; }

        #endregion

        #region Analysis

        bool isControlRoot { get; }

        #endregion

        #region Widget

        Vector2 position { get; set; }

        #endregion
    }

    public static class XUnit
    {
        public static ValueInput CompatibleValueInput(this IUnit unit, Type outputType)
        {
            Ensure.That(nameof(outputType)).IsNotNull(outputType);

            return unit.valueInputs
                .Where(valueInput => ConversionUtility.CanConvert(outputType, valueInput.type, false))
                .OrderBy((valueInput) =>
                {
                    var exactType = outputType == valueInput.type;
                    var free = !valueInput.hasValidConnection;

                    if (free && exactType)
                    {
                        return 1;
                    }
                    else if (free)
                    {
                        return 2;
                    }
                    else if (exactType)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }).FirstOrDefault();
        }

        public static ValueOutput CompatibleValueOutput(this IUnit unit, Type inputType)
        {
            Ensure.That(nameof(inputType)).IsNotNull(inputType);

            return unit.valueOutputs
                .Where(valueOutput => ConversionUtility.CanConvert(valueOutput.type, inputType, false))
                .OrderBy((valueOutput) =>
                {
                    var exactType = inputType == valueOutput.type;
                    var free = !valueOutput.hasValidConnection;

                    if (free && exactType)
                    {
                        return 1;
                    }
                    else if (free)
                    {
                        return 2;
                    }
                    else if (exactType)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }).FirstOrDefault();
        }
    }
}
