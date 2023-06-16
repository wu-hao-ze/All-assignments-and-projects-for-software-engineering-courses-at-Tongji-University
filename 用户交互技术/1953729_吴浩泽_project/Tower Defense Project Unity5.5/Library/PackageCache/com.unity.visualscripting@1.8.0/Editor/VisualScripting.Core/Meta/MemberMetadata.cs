using System;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class MemberMetadata : Metadata
    {
        public enum Mode
        {
            Field,
            Property
        }

        public MemberMetadata(string name, BindingFlags bindingFlags, Metadata parent) : base(name, parent)
        {
            this.name = name;
            this.bindingFlags = bindingFlags;
            Reflect(true);
        }

        public string name { get; private set; }
        public BindingFlags bindingFlags { get; private set; }
        public MemberInfo member { get; private set; }
        public FieldInfo field { get; private set; }
        public PropertyInfo property { get; private set; }

        public Mode mode { get; private set; }

        protected override object rawValue
        {
            get
            {
                switch (mode)
                {
                    case Mode.Field:
                        return field.GetValueOptimized(parent.value);
                    case Mode.Property:
                        return property.GetValueOptimized(parent.value);
                    default:
                        throw new UnexpectedEnumValueException<Mode>(mode);
                }
            }
            set
            {
                switch (mode)
                {
                    case Mode.Field:
                        field.SetValueOptimized(parent.value, value);
                        break;
                    case Mode.Property:
                        property.SetValueOptimized(parent.value, value);
                        break;
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
            field = parent.valueType.GetFieldUnambiguous(name, bindingFlags);

            if (field != null)
            {
                mode = Mode.Field;
                member = field;
                definedType = field.FieldType;
                field.Prewarm();
            }
            else
            {
                property = parent.valueType.GetPropertyUnambiguous(name, bindingFlags);

                if (property != null)
                {
                    mode = Mode.Property;
                    member = property;
                    definedType = property.PropertyType;
                    property.Prewarm();
                }
            }

            if (member == null)
            {
                if (throwOnFail)
                {
                    throw new MissingMemberException($"Failed to find reflected member '{name}' on '{parent.valueType.CSharpName()}':\n{this}");
                }
                else
                {
                    Unlink();
                    return;
                }
            }

            // The text is created from class properties found by reflection.
            // So to mitigate the risk of breaking something that is looking for a 'macro' property,
            // instead of renaming the 'macro' property into 'graph asset' we are just changing the text.
            var text = member.HumanName();
            if (text == "Macro")
                text = "Graph";

            label = new GUIContent(text, member.Summary());
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return Attribute.GetCustomAttributes(member, inherit);
        }

        public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    }
}
