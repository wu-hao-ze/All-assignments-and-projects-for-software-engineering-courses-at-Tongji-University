using UnityEngine;

namespace Unity.VisualScripting
{
    [SpecialUnit]
    public abstract class UnifiedVariableUnit : Unit, IUnifiedVariableUnit
    {
        /// <summary>
        /// The kind of variable.
        /// </summary>
        [Serialize, Inspectable, UnitHeaderInspectable]
        public VariableKind kind { get; set; }

        /// <summary>
        /// The name of the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput name { get; private set; }

        /// <summary>
        /// The source of the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput @object { get; private set; }

        protected override void Definition()
        {
            name = ValueInput(nameof(name), string.Empty);

            if (kind == VariableKind.Object)
            {
                @object = ValueInput<GameObject>(nameof(@object), null).NullMeansSelf();
            }
        }
    }
}
