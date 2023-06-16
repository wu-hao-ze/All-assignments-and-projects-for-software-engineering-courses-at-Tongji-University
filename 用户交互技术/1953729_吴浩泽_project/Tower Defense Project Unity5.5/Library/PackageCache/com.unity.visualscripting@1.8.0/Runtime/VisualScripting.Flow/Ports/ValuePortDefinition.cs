using System;

namespace Unity.VisualScripting
{
    public abstract class ValuePortDefinition : UnitPortDefinition, IUnitValuePortDefinition
    {
        // For the virtual inheritors
        [SerializeAs(nameof(_type))]
        private Type _type { get; set; }

        [Inspectable]
        [DoNotSerialize]
        public virtual Type type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public override bool isValid => base.isValid && type != null;
    }
}
