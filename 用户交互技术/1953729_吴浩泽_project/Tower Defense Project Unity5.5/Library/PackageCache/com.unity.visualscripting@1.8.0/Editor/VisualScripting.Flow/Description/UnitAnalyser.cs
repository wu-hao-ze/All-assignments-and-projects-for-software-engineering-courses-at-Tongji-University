using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Analyser(typeof(IUnit))]
    public class UnitAnalyser<TUnit> : Analyser<TUnit, UnitAnalysis>
        where TUnit : class, IUnit
    {
        public UnitAnalyser(GraphReference reference, TUnit target) : base(reference, target) { }

        public TUnit unit => target;

        [Assigns]
        protected bool IsEntered()
        {
            using (var recursion = Recursion.New(1))
            {
                return IsEntered(unit, recursion);
            }
        }

        private static bool IsEntered(IUnit unit, Recursion recursion)
        {
            if (unit.isControlRoot)
            {
                return true;
            }

            foreach (var controlInput in unit.controlInputs)
            {
                if (!controlInput.isPredictable || controlInput.couldBeEntered)
                {
                    return true;
                }
            }

            foreach (var valueOutput in unit.valueOutputs)
            {
                if (!recursion?.TryEnter(valueOutput) ?? false)
                {
                    continue;
                }

                var valueOutputEntered = valueOutput.validConnections.Any(c => IsEntered(c.destination.unit, recursion));

                recursion?.Exit(valueOutput);

                if (valueOutputEntered)
                {
                    return true;
                }
            }

            return false;
        }

        private string PortLabel(IUnitPort port)
        {
            return port.Description<UnitPortDescription>().label;
        }

        [Assigns]
        protected virtual IEnumerable<Warning> Warnings()
        {
            var isEntered = IsEntered();

            if (!unit.isDefined)
            {
                if (unit.definitionException != null)
                {
                    yield return Warning.Exception(unit.definitionException);
                }
                else if (!unit.canDefine)
                {
                    yield return Warning.Caution("Node is not properly configured.");
                }
            }
            else if (unit is MissingType)
            {
                var formerType = $"{(unit as MissingType)?.formerType}";
                formerType = string.IsNullOrEmpty(formerType) ? string.Empty : $"'{formerType}'";
                yield return new ActionButtonWarning(
                    WarningLevel.Error,
                    $"The source script for this node type can't be found. Did you remove its script?\n" +
                    $"Replace the node or add the {formerType} script file back to your project files.",
                    "Replace Node",
                    () =>
                    { UnitWidgetHelper.ReplaceUnit(unit, reference, context, context.selection, new EventWrapper(unit)); }
                );
                yield break;
            }

            if (!isEntered)
            {
                yield return Warning.Info("Node is never entered.");
            }

            // Obsolete attribute is not inherited, so traverse the chain manually
            var obsoleteAttribute = unit.GetType().AndHierarchy().FirstOrDefault(t => t.HasAttribute<ObsoleteAttribute>())?.GetAttribute<ObsoleteAttribute>();

            if (obsoleteAttribute != null)
            {
                var unitName = BoltFlowNameUtility.UnitTitle(unit.GetType(), true, false);

                if (obsoleteAttribute.Message != null)
                {
                    Debug.LogWarning($"\"{unitName}\" node is deprecated: {obsoleteAttribute.Message}");
                    yield return Warning.Caution($"Deprecated: {obsoleteAttribute.Message}");
                }
                else
                {
                    Debug.LogWarning($"\"{unitName}\" node is deprecated.");
                    yield return Warning.Caution("This node is deprecated.");
                }
            }

            if (unit.isDefined)
            {
                foreach (var invalidInput in unit.invalidInputs)
                {
                    yield return Warning.Caution($"{PortLabel(invalidInput)} is not used by this unit.");
                }

                foreach (var invalidOutput in unit.invalidOutputs)
                {
                    yield return Warning.Caution($"{PortLabel(invalidOutput)} is not provided by this unit.");
                }

                foreach (var validPort in unit.validPorts)
                {
                    if (validPort.hasInvalidConnection)
                    {
                        yield return Warning.Caution($"{PortLabel(validPort)} has an invalid connection.");
                    }
                }

#if UNITY_IOS || UNITY_ANDROID || UNITY_TVOS
                if (unit is IMouseEventUnit)
                {
                    var graphName = string.IsNullOrEmpty(unit.graph.title) ? "A ScriptGraph" : $"The ScriptGraph {unit.graph.title}";
                    var unitName = BoltFlowNameUtility.UnitTitle(unit.GetType(), true, false);
                    Debug.LogWarning($"{graphName} contains a {unitName} node. Presence of MouseEvent nodes might impact performance on handheld devices.");
                    yield return Warning.Caution("Presence of MouseEvent nodes might impact performance on handheld devices.");
                }
#endif
            }

            foreach (var controlInput in unit.controlInputs)
            {
                if (!controlInput.hasValidConnection)
                {
                    continue;
                }

                foreach (var relation in controlInput.relations)
                {
                    if (relation.source is ValueInput)
                    {
                        var valueInput = (ValueInput)relation.source;

                        foreach (var warning in ValueInputWarnings(valueInput))
                        {
                            yield return warning;
                        }
                    }
                }
            }

            foreach (var controlOutput in unit.controlOutputs)
            {
                if (!controlOutput.hasValidConnection)
                {
                    continue;
                }

                var controlInputs = controlOutput.relations.Select(r => r.source).OfType<ControlInput>();

                var isTriggered = !controlInputs.Any() || controlInputs.Any(ci => !ci.isPredictable || ci.couldBeEntered);

                foreach (var relation in controlOutput.relations)
                {
                    if (relation.source is ValueInput)
                    {
                        var valueInput = (ValueInput)relation.source;

                        foreach (var warning in ValueInputWarnings(valueInput))
                        {
                            yield return warning;
                        }
                    }
                }

                if (isEntered && !isTriggered)
                {
                    yield return Warning.Caution($"{PortLabel(controlOutput)} is connected, but it is never triggered.");
                }
            }

            foreach (var valueOutput in unit.valueOutputs)
            {
                if (!valueOutput.hasValidConnection)
                {
                    continue;
                }

                foreach (var relation in valueOutput.relations)
                {
                    if (relation.source is ControlInput)
                    {
                        var controlInput = (ControlInput)relation.source;

                        if (isEntered && controlInput.isPredictable && !controlInput.couldBeEntered)
                        {
                            yield return Warning.Severe($"{PortLabel(controlInput)} is required, but it is never entered.");
                        }
                    }
                    else if (relation.source is ValueInput)
                    {
                        var valueInput = (ValueInput)relation.source;

                        foreach (var warning in ValueInputWarnings(valueInput))
                        {
                            yield return warning;
                        }
                    }
                }
            }
        }

        private IEnumerable<Warning> ValueInputWarnings(ValueInput valueInput)
        {
            // We can disable null reference check if no self is available
            // and the port requires an owner, for example in macros.
            var trustFutureOwner = valueInput.nullMeansSelf && reference.self == null;

            var checkForNullReference = BoltFlow.Configuration.predictPotentialNullReferences && !valueInput.allowsNull && !trustFutureOwner;

            var checkForMissingComponent = BoltFlow.Configuration.predictPotentialMissingComponents && typeof(Component).IsAssignableFrom(valueInput.type);

            // Note that we cannot directly check the input's predicted value, because it
            // will return false for safeguard specifically because it might be missing requirements.
            // Therefore, we first check the connected value, then the default value.

            // If the port is connected to a predictable output, use the connected value to perform checks.
            if (valueInput.hasValidConnection)
            {
                var valueOutput = valueInput.validConnectedPorts.Single();

                if (Flow.CanPredict(valueOutput, reference))
                {
                    if (checkForNullReference)
                    {
                        if (Flow.Predict(valueOutput, reference) == null)
                        {
                            yield return Warning.Severe($"{PortLabel(valueInput)} cannot be null.");
                        }
                    }

                    if (checkForMissingComponent)
                    {
                        var connectedPredictedValue = Flow.Predict(valueOutput, reference);

                        // This check is necessary, because the predicted value could be
                        // incompatible as connections with non-guaranteed conversions are allowed.
                        if (ConversionUtility.CanConvert(connectedPredictedValue, typeof(GameObject), true))
                        {
                            var gameObject = ConversionUtility.Convert<GameObject>(connectedPredictedValue);

                            if (gameObject != null)
                            {
                                var component = (Component)ConversionUtility.Convert(gameObject, valueInput.type);

                                if (component == null)
                                {
                                    yield return Warning.Caution($"{PortLabel(valueInput)} is missing a {valueInput.type.DisplayName()} component.");
                                }
                            }
                        }
                    }
                }
            }
            // If the port isn't connected but has a default value, use the default value to perform checks.
            else if (valueInput.hasDefaultValue)
            {
                if (checkForNullReference)
                {
                    if (Flow.Predict(valueInput, reference) == null)
                    {
                        yield return Warning.Severe($"{PortLabel(valueInput)} cannot be null.");
                    }
                }

                if (checkForMissingComponent)
                {
                    var unconnectedPredictedValue = Flow.Predict(valueInput, reference);

                    if (ConversionUtility.CanConvert(unconnectedPredictedValue, typeof(GameObject), true))
                    {
                        var gameObject = ConversionUtility.Convert<GameObject>(unconnectedPredictedValue);

                        if (gameObject != null)
                        {
                            var component = (Component)ConversionUtility.Convert(gameObject, valueInput.type);

                            if (component == null)
                            {
                                yield return Warning.Caution($"{PortLabel(valueInput)} is missing a {valueInput.type.DisplayName()} component.");
                            }
                        }
                    }
                }
            }
            // The value isn't connected and has no default value,
            // therefore it is certain to be missing at runtime.
            else
            {
                yield return Warning.Severe($"{PortLabel(valueInput)} is missing.");
            }
        }
    }
}
