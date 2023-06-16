using System;

namespace Unity.VisualScripting
{
    [SpecialUnit]
    [Obsolete("Use the new unified variable nodes instead.")]
    public abstract class VariableUnit : Unit, IVariableUnit
    {
        protected VariableUnit() : base() { }

        protected VariableUnit(string defaultName)
        {
            Ensure.That(nameof(defaultName)).IsNotNull(defaultName);

            this.defaultName = defaultName;
        }

        [DoNotSerialize]
        public string defaultName { get; } = string.Empty;

        /// <summary>
        /// The name of the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput name { get; private set; }

        protected abstract VariableDeclarations GetDeclarations(Flow flow);

        protected override void Definition()
        {
            name = ValueInput(nameof(name), defaultName);
        }
    }
}
