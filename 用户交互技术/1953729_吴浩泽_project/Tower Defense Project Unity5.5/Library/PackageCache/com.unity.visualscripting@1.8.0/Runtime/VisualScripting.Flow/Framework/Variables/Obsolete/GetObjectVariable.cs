using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of an object variable.
    /// </summary>
    [UnitSurtitle("Object")]
    public sealed class GetObjectVariable : GetVariableUnit, IObjectVariableUnit
    {
        public GetObjectVariable() : base() { }

        public GetObjectVariable(string name) : base(name) { }

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

            Requirement(source, value);
        }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Object(flow.GetValue<GameObject>(source));
        }
    }
}
