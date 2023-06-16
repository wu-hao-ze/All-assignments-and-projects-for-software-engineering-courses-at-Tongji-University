using System;

namespace Unity.VisualScripting
{
    public sealed class ValueInputDefinition : ValuePortDefinition, IUnitInputPortDefinition
    {
        [SerializeAs(nameof(defaultValue))]
        private object _defaultvalue;

        [Inspectable]
        [DoNotSerialize]
        public override Type type
        {
            get
            {
                return base.type;
            }
            set
            {
                base.type = value;

                if (!type.IsAssignableFrom(defaultValue))
                {
                    if (ValueInput.SupportsDefaultValue(type))
                    {
                        _defaultvalue = type.PseudoDefault();
                    }
                    else
                    {
                        hasDefaultValue = false;
                        _defaultvalue = null;
                    }
                }
            }
        }

        [Serialize]
        [Inspectable]
        public bool hasDefaultValue { get; set; }

        [DoNotSerialize]
        [Inspectable]
        public object defaultValue
        {
            get
            {
                return _defaultvalue;
            }
            set
            {
                if (type == null)
                {
                    throw new InvalidOperationException("A type must be defined before setting the default value.");
                }

                if (!ValueInput.SupportsDefaultValue(type))
                {
                    throw new InvalidOperationException("The selected type does not support default values.");
                }

                Ensure.That(nameof(value)).IsOfType(value, type);

                _defaultvalue = value;
            }
        }
    }
}
