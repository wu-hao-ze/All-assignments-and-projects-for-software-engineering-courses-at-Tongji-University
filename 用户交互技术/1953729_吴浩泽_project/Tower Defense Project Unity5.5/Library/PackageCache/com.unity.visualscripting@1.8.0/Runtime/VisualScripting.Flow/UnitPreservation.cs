using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class UnitPreservation : IPoolable
    {
        private struct UnitPortPreservation
        {
            public readonly IUnit unit;

            public readonly string key;

            public UnitPortPreservation(IUnitPort port)
            {
                unit = port.unit;
                key = port.key;
            }

            public UnitPortPreservation(IUnit unit, string key)
            {
                this.unit = unit;
                this.key = key;
            }

            public IUnitPort GetOrCreateInput(out InvalidInput newInvalidInput)
            {
                var key = this.key;

                if (!unit.inputs.Any(p => p.key == key))
                {
                    newInvalidInput = new InvalidInput(key);
                    unit.invalidInputs.Add(newInvalidInput);
                }
                else
                {
                    newInvalidInput = null;
                }

                return unit.inputs.Single(p => p.key == key);
            }

            public IUnitPort GetOrCreateOutput(out InvalidOutput newInvalidOutput)
            {
                var key = this.key;

                if (!unit.outputs.Any(p => p.key == key))
                {
                    newInvalidOutput = new InvalidOutput(key);
                    unit.invalidOutputs.Add(newInvalidOutput);
                }
                else
                {
                    newInvalidOutput = null;
                }

                return unit.outputs.Single(p => p.key == key);
            }
        }

        private readonly Dictionary<string, object> defaultValues = new Dictionary<string, object>();

        private readonly Dictionary<string, List<UnitPortPreservation>> inputConnections = new Dictionary<string, List<UnitPortPreservation>>();

        private readonly Dictionary<string, List<UnitPortPreservation>> outputConnections = new Dictionary<string, List<UnitPortPreservation>>();

        private bool disposed;

        void IPoolable.New()
        {
            disposed = false;
        }

        void IPoolable.Free()
        {
            disposed = true;

            foreach (var inputConnection in inputConnections)
            {
                ListPool<UnitPortPreservation>.Free(inputConnection.Value);
            }

            foreach (var outputConnection in outputConnections)
            {
                ListPool<UnitPortPreservation>.Free(outputConnection.Value);
            }

            defaultValues.Clear();
            inputConnections.Clear();
            outputConnections.Clear();
        }

        private UnitPreservation() { }

        public static UnitPreservation Preserve(IUnit unit)
        {
            var preservation = GenericPool<UnitPreservation>.New(() => new UnitPreservation());

            foreach (var defaultValue in unit.defaultValues)
            {
                preservation.defaultValues.Add(defaultValue.Key, defaultValue.Value);
            }

            foreach (var input in unit.inputs)
            {
                if (input.hasAnyConnection)
                {
                    preservation.inputConnections.Add(input.key, ListPool<UnitPortPreservation>.New());

                    foreach (var connectedPort in input.connectedPorts)
                    {
                        preservation.inputConnections[input.key].Add(new UnitPortPreservation(connectedPort));
                    }
                }
            }

            foreach (var output in unit.outputs)
            {
                if (output.hasAnyConnection)
                {
                    preservation.outputConnections.Add(output.key, ListPool<UnitPortPreservation>.New());

                    foreach (var connectedPort in output.connectedPorts)
                    {
                        preservation.outputConnections[output.key].Add(new UnitPortPreservation(connectedPort));
                    }
                }
            }

            return preservation;
        }

        public void RestoreTo(IUnit unit)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(ToString());
            }

            // Restore inline values if possible

            foreach (var previousDefaultValue in defaultValues)
            {
                if (unit.defaultValues.ContainsKey(previousDefaultValue.Key) &&
                    unit.valueInputs.Contains(previousDefaultValue.Key) &&
                    unit.valueInputs[previousDefaultValue.Key].type.IsAssignableFrom(previousDefaultValue.Value))
                {
                    unit.defaultValues[previousDefaultValue.Key] = previousDefaultValue.Value;
                }
            }

            // Restore connections if possible

            foreach (var previousInputConnections in inputConnections)
            {
                var previousInputPort = new UnitPortPreservation(unit, previousInputConnections.Key);
                var previousOutputPorts = previousInputConnections.Value;

                foreach (var previousOutputPort in previousOutputPorts)
                {
                    RestoreConnection(previousOutputPort, previousInputPort);
                }
            }

            foreach (var previousOutputConnections in outputConnections)
            {
                var previousOutputPort = new UnitPortPreservation(unit, previousOutputConnections.Key);
                var previousInputPorts = previousOutputConnections.Value;

                foreach (var previousInputPort in previousInputPorts)
                {
                    RestoreConnection(previousOutputPort, previousInputPort);
                }
            }

            GenericPool<UnitPreservation>.Free(this);
        }

        private void RestoreConnection(UnitPortPreservation sourcePreservation, UnitPortPreservation destinationPreservation)
        {
            InvalidOutput newInvalidSource;
            InvalidInput newInvalidDestination;

            var source = sourcePreservation.GetOrCreateOutput(out newInvalidSource);
            var destination = destinationPreservation.GetOrCreateInput(out newInvalidDestination);

            if (source.CanValidlyConnectTo(destination))
            {
                source.ValidlyConnectTo(destination);
            }
            else if (source.CanInvalidlyConnectTo(destination))
            {
                source.InvalidlyConnectTo(destination);
            }
            else
            {
                // In this case, we created invalid ports to attempt a connection,
                // but even that failed (due to, for example, a cross-graph restoration).
                // Therefore, we need to delete the invalid ports we created.

                if (newInvalidSource != null)
                {
                    sourcePreservation.unit.invalidOutputs.Remove(newInvalidSource);
                }

                if (newInvalidDestination != null)
                {
                    destinationPreservation.unit.invalidInputs.Remove(newInvalidDestination);
                }
            }
        }
    }
}
