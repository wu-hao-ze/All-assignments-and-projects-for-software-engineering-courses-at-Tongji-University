using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class DictionaryValueAtIndexMetadata : DictionaryIndexMetadata
    {
        public DictionaryValueAtIndexMetadata(int index, Metadata parent) : base(SubpathPrefix, index, parent) { }

        protected override object rawValue
        {
            get
            {
                if (parentIsOrderedDictionary)
                {
                    return ((IOrderedDictionary)parent.value)[index];
                }
                else
                {
                    return ((IDictionary)parent.value).Values.Cast<object>().ElementAt(index);
                }
            }
            set
            {
                if (parentIsOrderedDictionary)
                {
                    ((IOrderedDictionary)parent.value)[index] = value;
                }
                else
                {
                    var key = ((IDictionary)parent.value).Keys.Cast<object>().ElementAt(index);

                    ((IDictionary)parent.value)[key] = value;
                }
            }
        }

        protected override Type GetDefinedType(Type dictionaryType)
        {
            return TypeUtility.GetDictionaryValueType(dictionaryType, true);
        }

        public const string SubpathPrefix = "__valueAt.";
    }
}
