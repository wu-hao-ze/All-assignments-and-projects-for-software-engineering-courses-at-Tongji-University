using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Assigns the value of a variable.
    /// </summary>
    [UnitShortTitle("Set Variable")]
    public sealed class SetVariable : UnifiedVariableUnit
    {
        /// <summary>
        /// The entry point to assign the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput assign { get; set; }

        /// <summary>
        /// The value to assign to the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("New Value")]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The action to execute once the variable has been assigned.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput assigned { get; set; }

        /// <summary>
        /// The value assigned to the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Value")]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            assign = ControlInput(nameof(assign), Assign);
            input = ValueInput<object>(nameof(input)).AllowsNull();
            output = ValueOutput<object>(nameof(output));
            assigned = ControlOutput(nameof(assigned));

            Requirement(name, assign);
            Requirement(input, assign);
            Assignment(assign, output);
            Succession(assign, assigned);

            if (kind == VariableKind.Object)
            {
                Requirement(@object, assign);
            }
        }

        private ControlOutput Assign(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);
            var input = flow.GetValue(this.input);

            switch (kind)
            {
                case VariableKind.Flow:
                    flow.variables.Set(name, input);
                    break;
                case VariableKind.Graph:
                    Variables.Graph(flow.stack).Set(name, input);
                    break;
                case VariableKind.Object:
                    Variables.Object(flow.GetValue<GameObject>(@object)).Set(name, input);
                    break;
                case VariableKind.Scene:
                    Variables.Scene(flow.stack.scene).Set(name, input);
                    break;
                case VariableKind.Application:
                    Variables.Application.Set(name, input);
                    break;
                case VariableKind.Saved:
                    Variables.Saved.Set(name, input);
                    break;
                default:
                    throw new UnexpectedEnumValueException<VariableKind>(kind);
            }

            flow.SetValue(output, input);

            return assigned;
        }
    }
}
