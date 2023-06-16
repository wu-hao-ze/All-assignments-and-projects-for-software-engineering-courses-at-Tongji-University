using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks if an object variable is defined.
    /// </summary>
    [UnitSurtitle("Object")]
    public sealed class IsObjectVariableDefined : IsVariableDefinedUnit, IObjectVariableUnit
    {
        public IsObjectVariableDefined() : base() { }

        public IsObjectVariableDefined(string name) : base(name) { }

        /// <summary>
        /// The source of the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput source { get; private set; }

        protected override void Definition()
        {
            source = ValueInput<GameObject>(nameof(source), null).NullMeansSelf();

            base.Definition();

            Requirement(source, isDefined);
        }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Object(flow.GetValue<GameObject>(source));
        }
    }
}
