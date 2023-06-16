using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using NCalc = Unity.VisualScripting.Dependencies.NCalc.Expression;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Evaluates a mathematical or logical formula with multiple arguments.
    /// </summary>
    public sealed class Formula : MultiInputUnit<object>
    {
        [SerializeAs(nameof(Formula))]
        private string _formula;

        private NCalc ncalc;

        /// <summary>
        /// A mathematical or logical expression tree.
        /// </summary>
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable]
        [InspectorTextArea]
        public string formula
        {
            get => _formula;
            set
            {
                _formula = value;

                InitializeNCalc();
            }
        }

        /// <summary>
        /// Whether input arguments should only be fetched once and then reused.
        /// </summary>
        [Serialize]
        [Inspectable(order = int.MaxValue)]
        [InspectorExpandTooltip]
        public bool cacheArguments { get; set; }

        /// <summary>
        /// The result of the calculation or evaluation.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput result { get; private set; }

        protected override int minInputCount => 0;

        protected override void Definition()
        {
            base.Definition();

            result = ValueOutput(nameof(result), Evaluate);

            InputsAllowNull();

            foreach (var input in multiInputs)
            {
                Requirement(input, result);
            }

            InitializeNCalc();
        }

        private void InitializeNCalc()
        {
            if (string.IsNullOrEmpty(formula))
            {
                ncalc = null;
                return;
            }

            ncalc = new NCalc(formula);
            ncalc.Options = EvaluateOptions.IgnoreCase;
            ncalc.EvaluateParameter += EvaluateTreeParameter;
            ncalc.EvaluateFunction += EvaluateTreeFunction;
        }

        private object Evaluate(Flow flow)
        {
            if (ncalc == null)
            {
                throw new InvalidOperationException("No formula provided.");
            }

            ncalc.UpdateUnityTimeParameters();

            return ncalc.Evaluate(flow);
        }

        private void EvaluateTreeFunction(Flow flow, string name, FunctionArgs args)
        {
            if (name == "v2" || name == "V2")
            {
                if (args.Parameters.Length != 2)
                {
                    throw new ArgumentException($"v2() takes at exactly 2 arguments. {args.Parameters.Length} provided.");
                }

                args.Result = new Vector2
                    (
                    ConversionUtility.Convert<float>(args.Parameters[0].Evaluate(flow)),
                    ConversionUtility.Convert<float>(args.Parameters[1].Evaluate(flow))
                    );
            }
            else if (name == "v3" || name == "V3")
            {
                if (args.Parameters.Length != 3)
                {
                    throw new ArgumentException($"v3() takes at exactly 3 arguments. {args.Parameters.Length} provided.");
                }

                args.Result = new Vector3
                    (
                    ConversionUtility.Convert<float>(args.Parameters[0].Evaluate(flow)),
                    ConversionUtility.Convert<float>(args.Parameters[1].Evaluate(flow)),
                    ConversionUtility.Convert<float>(args.Parameters[2].Evaluate(flow))
                    );
            }
            else if (name == "v4" || name == "V4")
            {
                if (args.Parameters.Length != 4)
                {
                    throw new ArgumentException($"v4() takes at exactly 4 arguments. {args.Parameters.Length} provided.");
                }

                args.Result = new Vector4
                    (
                    ConversionUtility.Convert<float>(args.Parameters[0].Evaluate(flow)),
                    ConversionUtility.Convert<float>(args.Parameters[1].Evaluate(flow)),
                    ConversionUtility.Convert<float>(args.Parameters[2].Evaluate(flow)),
                    ConversionUtility.Convert<float>(args.Parameters[3].Evaluate(flow))
                    );
            }
        }

        public object GetParameterValue(Flow flow, string name)
        {
            if (name.Length == 1)
            {
                var character = name[0];

                if (char.IsLetter(character))
                {
                    character = char.ToLower(character);

                    var index = GetArgumentIndex(character);

                    if (index < multiInputs.Count)
                    {
                        var input = multiInputs[index];

                        if (cacheArguments && !flow.IsLocal(input))
                        {
                            flow.SetValue(input, flow.GetValue<object>(input));
                        }

                        return flow.GetValue<object>(input);
                    }
                }
            }
            else
            {
                if (Variables.Graph(flow.stack).IsDefined(name))
                {
                    return Variables.Graph(flow.stack).Get(name);
                }

                var self = flow.stack.self;

                if (self != null)
                {
                    if (Variables.Object(self).IsDefined(name))
                    {
                        return Variables.Object(self).Get(name);
                    }
                }

                var scene = flow.stack.scene;

                if (scene != null)
                {
                    if (Variables.Scene(scene).IsDefined(name))
                    {
                        return Variables.Scene(scene).Get(name);
                    }
                }

                if (Variables.Application.IsDefined(name))
                {
                    return Variables.Application.Get(name);
                }

                if (Variables.Saved.IsDefined(name))
                {
                    return Variables.Saved.Get(name);
                }
            }

            throw new InvalidOperationException($"Unknown expression tree parameter: '{name}'.\nSupported parameter names are alphabetical indices and variable names.");
        }

        private void EvaluateTreeParameter(Flow flow, string name, ParameterArgs args)
        {
            // [param.fieldOrProperty]
            // [param.parmeterLessMethod()]
            if (name.Contains("."))
            {
                var parts = name.Split('.');

                if (parts.Length == 2)
                {
                    var parameterName = parts[0];

                    var memberName = parts[1].TrimEnd("()");

                    var variableValue = GetParameterValue(flow, parameterName);

                    var manipulator = new Member(variableValue.GetType(), memberName, Type.EmptyTypes);

                    var target = variableValue;

                    if (manipulator.isInvocable)
                    {
                        args.Result = manipulator.Invoke(target);
                    }
                    else if (manipulator.isGettable)
                    {
                        args.Result = manipulator.Get(target);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot get or invoke expression tree parameter: [{parameterName}.{memberName}]");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Cannot parse expression tree parameter: [{name}]");
                }
            }
            else
            {
                args.Result = GetParameterValue(flow, name);
            }
        }

        public static string GetArgumentName(int index)
        {
            if (index > ('z' - 'a'))
            {
                throw new NotImplementedException("Argument indices above 26 are not yet supported.");
            }

            return ((char)('a' + index)).ToString();
        }

        public static int GetArgumentIndex(char name)
        {
            if (name < 'a' || name > 'z')
            {
                throw new NotImplementedException("Unalphabetical argument names are not yet supported.");
            }

            return name - 'a';
        }
    }
}
