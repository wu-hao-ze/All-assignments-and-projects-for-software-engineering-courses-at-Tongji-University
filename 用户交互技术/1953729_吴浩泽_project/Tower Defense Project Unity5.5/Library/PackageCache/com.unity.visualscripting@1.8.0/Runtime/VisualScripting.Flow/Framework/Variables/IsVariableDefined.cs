using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks whether a variable is defined.
    /// </summary>
    [UnitTitle("Has Variable")]
    public sealed class IsVariableDefined : UnifiedVariableUnit
    {
        /// <summary>
        /// Whether the variable is defined.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Defined")]
        [PortLabelHidden]
        [PortKey("isDefined")]
        public ValueOutput isVariableDefined { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            isVariableDefined = ValueOutput("isDefined", IsDefined);

            Requirement(name, isVariableDefined);

            if (kind == VariableKind.Object)
            {
                Requirement(@object, isVariableDefined);
            }
        }

        private bool IsDefined(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);

            switch (kind)
            {
                case VariableKind.Flow:
                    return flow.variables.IsDefined(name);
                case VariableKind.Graph:
                    return Variables.Graph(flow.stack).IsDefined(name);
                case VariableKind.Object:
                    return Variables.Object(flow.GetValue<GameObject>(@object)).IsDefined(name);
                case VariableKind.Scene:
                    return Variables.Scene(flow.stack.scene).IsDefined(name);
                case VariableKind.Application:
                    return Variables.Application.IsDefined(name);
                case VariableKind.Saved:
                    return Variables.Saved.IsDefined(name);
                default:
                    throw new UnexpectedEnumValueException<VariableKind>(kind);
            }
        }
    }
}
