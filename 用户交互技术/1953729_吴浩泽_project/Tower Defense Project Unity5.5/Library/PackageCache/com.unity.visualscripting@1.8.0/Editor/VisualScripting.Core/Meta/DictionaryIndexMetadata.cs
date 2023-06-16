using System;
using System.Collections;
using System.Collections.Specialized;

namespace Unity.VisualScripting
{
    public abstract class DictionaryIndexMetadata : Metadata
    {
        protected DictionaryIndexMetadata(string subpathPrefix, int index, Metadata parent) : base(subpathPrefix + index, parent)
        {
            this.index = index;

            Reflect(true);
        }

        public int index { get; private set; }
        protected bool parentIsOrderedDictionary { get; private set; }

        protected override void OnParentValueChange(object previousValue)
        {
            base.OnParentValueChange(previousValue);

            Reflect(false);
        }

        protected abstract Type GetDefinedType(Type dictionaryType);

        private void Reflect(bool throwOnFail)
        {
            if (typeof(IDictionary).IsAssignableFrom(parent.valueType))
            {
                definedType = GetDefinedType(parent.valueType);

                parentIsOrderedDictionary = typeof(IOrderedDictionary).IsAssignableFrom(parent.valueType);
            }
            else
            {
                if (throwOnFail)
                {
                    throw new InvalidOperationException("Parent of dictionary index is not a dictionary:\n" + this);
                }
                else
                {
                    Unlink();
                    return;
                }
            }

            label = parent.label;
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return parent.GetCustomAttributes(inherit);
        }
    }
}
