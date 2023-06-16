using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of an object variable.
    /// </summary>
    [UnitSurtitle("Object")]
    public sealed class SetObjectVariable : SetVariableUnit, IObjectVariableUnit
    {
        public SetObjectVariable() : base() { }

        public SetObjectVariable(string name) : base(name) { }

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

            Requirement(source, assign);
        }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Object(flow.GetValue<GameObject>(source));
        }
    }
}
