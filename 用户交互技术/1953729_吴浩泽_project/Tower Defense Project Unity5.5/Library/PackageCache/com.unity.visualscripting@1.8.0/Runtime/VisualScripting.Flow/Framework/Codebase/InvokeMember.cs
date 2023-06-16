using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Invokes a method or a constructor via reflection.
    /// </summary>
    public sealed class InvokeMember : MemberUnit
    {
        public InvokeMember() : base() { }

        public InvokeMember(Member member) : base(member) { }

        private bool useExpandedParameters;

        /// <summary>
        /// Whether the target should be output to allow for chaining.
        /// </summary>
        [Serialize]
        [InspectableIf(nameof(supportsChaining))]
        public bool chainable { get; set; }

        [DoNotSerialize]
        public bool supportsChaining => member.requiresTarget;

        [DoNotSerialize]
        [MemberFilter(Methods = true, Constructors = true)]
        public Member invocation
        {
            get { return member; }
            set { member = value; }
        }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        [DoNotSerialize]
        public Dictionary<int, ValueInput> inputParameters { get; private set; }

        /// <summary>
        /// The target object used when setting the value.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Target")]
        [PortLabelHidden]
        public ValueOutput targetOutput { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput result { get; private set; }

        [DoNotSerialize]
        public Dictionary<int, ValueOutput> outputParameters { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        [DoNotSerialize]
        private int parameterCount;

        [Serialize]
        List<string> parameterNames;

        public override bool HandleDependencies()
        {
            if (!base.HandleDependencies())
                return false;

            // Here we have a chance to do a bit of post processing after deserialization of this node has occured.

            // In the past we did not serialize parameter names explicitly (only parameter types), however, if we have
            // exactly the same number of defaults as parameters, we happen to know what the original parameter names were.
            if (parameterNames == null && member.parameterTypes.Length == defaultValues.Count)
            {
                // Note that we strip the "%" prefix from the parameter name in the default values (the "%" denotes that
                // it is a parameter input)
                parameterNames = defaultValues.Select(defaultValue => defaultValue.Key.Substring(1)).ToList();
            }

            return true;
        }

        protected override void Definition()
        {
            base.Definition();

            inputParameters = new Dictionary<int, ValueInput>();
            outputParameters = new Dictionary<int, ValueOutput>();
            useExpandedParameters = true;

            enter = ControlInput(nameof(enter), Enter);
            exit = ControlOutput(nameof(exit));
            Succession(enter, exit);

            if (member.requiresTarget)
            {
                Requirement(target, enter);
            }

            if (supportsChaining && chainable)
            {
                targetOutput = ValueOutput(member.targetType, nameof(targetOutput));
                Assignment(enter, targetOutput);
            }

            if (member.isGettable)
            {
                result = ValueOutput(member.type, nameof(result), Result);

                if (member.requiresTarget)
                {
                    Requirement(target, result);
                }
            }

            var parameterInfos = member.GetParameterInfos().ToArray();

            parameterCount = parameterInfos.Length;

            bool needsParameterRemapping = false;
            for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                var parameterInfo = parameterInfos[parameterIndex];

                var parameterType = parameterInfo.UnderlyingParameterType();

                if (!parameterInfo.HasOutModifier())
                {
                    var inputParameterKey = "%" + parameterInfo.Name;

                    // Changes in parameter names are tolerated, use the old parameter naming for now and fix it later.
                    if (parameterNames != null && parameterNames[parameterIndex] != parameterInfo.Name)
                    {
                        inputParameterKey = "%" + parameterNames[parameterIndex];
                        needsParameterRemapping = true;
                    }

                    var inputParameter = ValueInput(parameterType, inputParameterKey);

                    inputParameters.Add(parameterIndex, inputParameter);

                    inputParameter.SetDefaultValue(parameterInfo.PseudoDefaultValue());

                    if (parameterInfo.AllowsNull())
                    {
                        inputParameter.AllowsNull();
                    }

                    Requirement(inputParameter, enter);

                    if (member.isGettable)
                    {
                        Requirement(inputParameter, result);
                    }
                }

                if (parameterInfo.ParameterType.IsByRef || parameterInfo.IsOut)
                {
                    var outputParameterKey = "&" + parameterInfo.Name;

                    // Changes in parameter names are tolerated, use the old parameter naming for now and fix it later.
                    if (parameterNames != null && parameterNames[parameterIndex] != parameterInfo.Name)
                    {
                        outputParameterKey = "&" + parameterNames[parameterIndex];
                        needsParameterRemapping = true;
                    }

                    var outputParameter = ValueOutput(parameterType, outputParameterKey);

                    outputParameters.Add(parameterIndex, outputParameter);

                    Assignment(enter, outputParameter);

                    useExpandedParameters = false;
                }
            }

            if (inputParameters.Count > 5)
            {
                useExpandedParameters = false;
            }

            if (parameterNames == null)
            {
                parameterNames = parameterInfos.Select(pInfo => pInfo.Name).ToList();
            }

            if (needsParameterRemapping)
            {
                // Note, this will have no effect unless we are in an Editor context. This is okay since for runtime
                // purposes as it is actually fine to continue to use the old parameter names for the sake of setting up
                // connections and default values. The only reason it is interesting to update to the new parameter
                // names is for UI purposes.
                UnityThread.EditorAsync(PostDeserializeRemapParameterNames);
            }
        }

        private void PostDeserializeRemapParameterNames()
        {
            var parameterInfos = member.GetParameterInfos().ToArray();

            // Sanity check
            if (parameterNames?.Count != parameterInfos.Length)
                return;

            // Check if any of the method parameter names have changed (Note: handling of parameter type changes is not
            // supported here, it is detected and handled elsewhere)
            List<(ValueInput port, ValueOutput[] connectedSources)> renamedInputs = null;
            List<(ValueOutput port, ValueInput[] connectedDestinations)> renamedOutputs = null;
            List<(string name, object value)> renamedDefaults = null;
            for (var i = 0; i < parameterInfos.Length; ++i)
            {
                var paramInfo = parameterInfos[i];
                var oldParamName = parameterNames[i];

                if (paramInfo.Name != oldParamName)
                {
                    // Phase 1 of parameter renaming: disconnect any nodes connected to affected ports, remove affected
                    // ports from port definition, and remove any default values associated with affected ports.
                    if (valueInputs.TryGetValue("%" + oldParamName, out var oldInput))
                    {
                        var connectionSources = oldInput.validConnections.Select(con => con.source).ToArray();
                        foreach (var source in connectionSources)
                            source.DisconnectFromValid(oldInput);

                        valueInputs.Remove(oldInput);

                        if (renamedInputs == null)
                            renamedInputs = new List<(ValueInput, ValueOutput[])>(1);
                        renamedInputs.Add((new ValueInput("%" + paramInfo.Name, paramInfo.ParameterType), connectionSources));

                        if (defaultValues.TryGetValue(oldInput.key, out var defaultValue))
                        {
                            defaultValues.Remove(oldInput.key);
                            if (renamedDefaults == null)
                                renamedDefaults = new List<(string, object)>(1);
                            renamedDefaults.Add(("%" + paramInfo.Name, defaultValue));
                        }
                    }
                    else if (valueOutputs.TryGetValue("&" + oldParamName, out var oldOutput))
                    {
                        var connectionDestinations = oldOutput.validConnections.Select(con => con.destination).ToArray();
                        foreach (var destination in connectionDestinations)
                            destination.DisconnectFromValid(oldOutput);

                        valueOutputs.Remove(oldOutput);

                        if (renamedOutputs == null)
                            renamedOutputs = new List<(ValueOutput, ValueInput[])>(1);
                        renamedOutputs.Add((new ValueOutput("&" + paramInfo.Name, paramInfo.ParameterType), connectionDestinations));
                    }

                    parameterNames[i] = paramInfo.Name;
                }
            }

            // Phase 2 of parameter renaming: add renamed version of affected ports back to the port definition, reconnect
            // nodes back to those renamed ports, and redefine default values for those ports.
            if (renamedInputs != null)
            {
                foreach (var renamedInput in renamedInputs)
                {
                    valueInputs.Add(renamedInput.port);
                    foreach (var source in renamedInput.connectedSources)
                        source.ConnectToValid(renamedInput.port);
                }
                if (renamedDefaults != null)
                {
                    foreach (var renamedDefault in renamedDefaults)
                        defaultValues[renamedDefault.name] = renamedDefault.value;
                }
            }

            if (renamedOutputs != null)
            {
                foreach (var renamedOutput in renamedOutputs)
                {
                    valueOutputs.Add(renamedOutput.port);
                    foreach (var destination in renamedOutput.connectedDestinations)
                        destination.ConnectToValid(renamedOutput.port);
                }
            }


            if (renamedInputs != null || renamedOutputs != null)
            {
                Define();
            }
        }

        protected override bool IsMemberValid(Member member)
        {
            return member.isInvocable;
        }

        private object Invoke(object target, Flow flow)
        {
            if (useExpandedParameters)
            {
                switch (inputParameters.Count)
                {
                    case 0:

                        return member.Invoke(target);

                    case 1:

                        return member.Invoke(target,
                            flow.GetConvertedValue(inputParameters[0]));

                    case 2:

                        return member.Invoke(target,
                            flow.GetConvertedValue(inputParameters[0]),
                            flow.GetConvertedValue(inputParameters[1]));

                    case 3:

                        return member.Invoke(target,
                            flow.GetConvertedValue(inputParameters[0]),
                            flow.GetConvertedValue(inputParameters[1]),
                            flow.GetConvertedValue(inputParameters[2]));

                    case 4:

                        return member.Invoke(target,
                            flow.GetConvertedValue(inputParameters[0]),
                            flow.GetConvertedValue(inputParameters[1]),
                            flow.GetConvertedValue(inputParameters[2]),
                            flow.GetConvertedValue(inputParameters[3]));

                    case 5:

                        return member.Invoke(target,
                            flow.GetConvertedValue(inputParameters[0]),
                            flow.GetConvertedValue(inputParameters[1]),
                            flow.GetConvertedValue(inputParameters[2]),
                            flow.GetConvertedValue(inputParameters[3]),
                            flow.GetConvertedValue(inputParameters[4]));

                    default:

                        throw new NotSupportedException();
                }
            }
            else
            {
                var arguments = new object[parameterCount];

                for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
                {
                    if (inputParameters.TryGetValue(parameterIndex, out var inputParameter))
                    {
                        arguments[parameterIndex] = flow.GetConvertedValue(inputParameter);
                    }
                }

                var result = member.Invoke(target, arguments);

                for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
                {
                    if (outputParameters.TryGetValue(parameterIndex, out var outputParameter))
                    {
                        flow.SetValue(outputParameter, arguments[parameterIndex]);
                    }
                }

                return result;
            }
        }

        private object GetAndChainTarget(Flow flow)
        {
            if (member.requiresTarget)
            {
                var target = flow.GetValue(this.target, member.targetType);

                if (supportsChaining && chainable)
                {
                    flow.SetValue(targetOutput, target);
                }

                return target;
            }

            return null;
        }

        private object Result(Flow flow)
        {
            var target = GetAndChainTarget(flow);

            return Invoke(target, flow);
        }

        private ControlOutput Enter(Flow flow)
        {
            var target = GetAndChainTarget(flow);

            var result = Invoke(target, flow);

            if (this.result != null)
            {
                flow.SetValue(this.result, result);
            }

            return exit;
        }

        #region Analytics

        public override AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            const int maxNumParameters = 5;
            var s = $"{member.targetType.FullName}.{member.name}";

            if (member.parameterTypes != null)
            {
                s += "(";

                for (var i = 0; i < member.parameterTypes.Length; ++i)
                {
                    if (i >= maxNumParameters)
                    {
                        s += $"->{i}";
                        break;
                    }

                    s += member.parameterTypes[i].FullName;
                    if (i < member.parameterTypes.Length - 1)
                        s += ", ";
                }

                s += ")";
            }

            var aid = new AnalyticsIdentifier
            {
                Identifier = s,
                Namespace = member.targetType.Namespace
            };
            aid.Hashcode = aid.Identifier.GetHashCode();
            return aid;
        }

        #endregion
    }
}
