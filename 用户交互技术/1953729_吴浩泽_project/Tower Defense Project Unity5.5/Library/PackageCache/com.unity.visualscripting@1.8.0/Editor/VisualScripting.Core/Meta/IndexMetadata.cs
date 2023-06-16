using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class IndexMetadata : Metadata
    {
        public enum Mode
        {
            List,
            Enumerable
        }

        public IndexMetadata(int index, Metadata parent) : base(index, parent)
        {
            this.index = index;

            Reflect(true);
        }

        public int index { get; private set; }

        public Mode mode { get; private set; }

        protected override object rawValue
        {
            get
            {
                switch (mode)
                {
                    case Mode.List:
                        return ((IList)parent.value)[index];
                    case Mode.Enumerable:
                        return ((IEnumerable)parent.value).Cast<object>().ElementAt(index); // ouch?
                    default:
                        throw new UnexpectedEnumValueException<Mode>(mode);
                }
            }
            set
            {
                switch (mode)
                {
                    case Mode.List:
                        ((IList)parent.value)[index] = value;
                        break;
                    case Mode.Enumerable:
                        throw new NotSupportedException("Cannot assign the value of an enumerated item.");
                    default:
                        throw new UnexpectedEnumValueException<Mode>(mode);
                }
            }
        }

        protected override void OnParentValueTypeChange(Type previousType)
        {
            Reflect(false);
        }

        private void Reflect(bool throwOnFail)
        {
            if (typeof(IList).IsAssignableFrom(parent.valueType))
            {
                definedType = TypeUtility.GetListElementType(parent.valueType, true);

                if (index < 0 || index >= ((IList)parent.value).Count)
                {
                    if (throwOnFail)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }
                    else
                    {
                        Unlink();
                        return;
                    }
                }

                mode = Mode.List;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(parent.valueType))
            {
                definedType = TypeUtility.GetEnumerableElementType(parent.valueType, true);
                mode = Mode.Enumerable;
            }
            else
            {
                if (throwOnFail)
                {
                    throw new InvalidOperationException("Parent of reflected index is not a list nor an enumerable:\n" + this);
                }
                else
                {
                    Unlink();
                    return;
                }
            }

            label = new GUIContent(parent.label);
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return parent.GetCustomAttributes(inherit);
        }
    }
}
