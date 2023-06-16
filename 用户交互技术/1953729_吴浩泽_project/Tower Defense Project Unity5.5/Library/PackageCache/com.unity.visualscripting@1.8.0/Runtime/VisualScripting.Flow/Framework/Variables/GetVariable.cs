using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of a variable.
    /// </summary>
    public sealed class GetVariable : UnifiedVariableUnit
    {
        /// <summary>
        /// The value of the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        /// <summary>
        /// The value to return if the variable is not defined.
        /// </summary>
        [DoNotSerialize]
        public ValueInput fallback { get; private set; }

        /// <summary>
        /// Whether a fallback value should be provided if the
        /// variable is not defined.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorLabel("Fallback")]
        public bool specifyFallback { get; set; } = false;

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput(nameof(value), Get).PredictableIf(IsDefined);

            Requirement(name, value);

            if (kind == VariableKind.Object)
            {
                Requirement(@object, value);
            }

            if (specifyFallback)
            {
                fallback = ValueInput<object>(nameof(fallback));
                Requirement(fallback, value);
            }
        }

        private bool IsDefined(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            GameObject @object = null;

            if (kind == VariableKind.Object)
            {
                @object = flow.GetValue<GameObject>(this.@object);

                if (@object == null)
                {
                    return false;
                }
            }

            var scene = flow.stack.scene;

            if (kind == VariableKind.Scene)
            {
                if (scene == null || !scene.Value.IsValid() || !scene.Value.isLoaded || !Variables.ExistInScene(scene))
                {
                    return false;
                }
            }

            switch (kind)
            {
                case VariableKind.Flow:
                    return flow.variables.IsDefined(name);
                case VariableKind.Graph:
                    return Variables.Graph(flow.stack).IsDefined(name);
                case VariableKind.Object:
                    return Variables.Object(@object).IsDefined(name);
                case VariableKind.Scene:
                    return Variables.Scene(scene.Value).IsDefined(name);
                case VariableKind.Application:
                    return Variables.Application.IsDefined(name);
                case VariableKind.Saved:
                    return Variables.Saved.IsDefined(name);
                default:
                    throw new UnexpectedEnumValueException<VariableKind>(kind);
            }
        }

        private object Get(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);

            VariableDeclarations variables;

            switch (kind)
            {
                case VariableKind.Flow:
                    variables = flow.variables;
                    break;
                case VariableKind.Graph:
                    variables = Variables.Graph(flow.stack);
                    break;
                case VariableKind.Object:
                    variables = Variables.Object(flow.GetValue<GameObject>(@object));
                    break;
                case VariableKind.Scene:
                    variables = Variables.Scene(flow.stack.scene);
                    break;
                case VariableKind.Application:
                    variables = Variables.Application;
                    break;
                case VariableKind.Saved:
                    variables = Variables.Saved;
                    break;
                default:
                    throw new UnexpectedEnumValueException<VariableKind>(kind);
            }

            if (specifyFallback && !variables.IsDefined(name))
            {
                return flow.GetValue(fallback);
            }

            return variables.Get(name);
        }
    }
}
