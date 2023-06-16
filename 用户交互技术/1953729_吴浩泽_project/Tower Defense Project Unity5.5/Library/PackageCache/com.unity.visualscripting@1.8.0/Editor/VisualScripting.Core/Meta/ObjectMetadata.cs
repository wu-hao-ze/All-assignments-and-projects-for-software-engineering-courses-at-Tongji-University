using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class ObjectMetadata : Metadata
    {
        public ObjectMetadata(object @object, Type definedType, Metadata parent) : base(@object, parent)
        {
            this.@object = @object;
            this.definedType = definedType;

            if (@object is UnityObject && !@object.IsUnityNull())
            {
                MatchWithPrefab();
            }
        }

        public ObjectMetadata(string name, object @object, Type definedType, Metadata parent) : base(name, parent)
        {
            this.name = name;
            this.@object = @object;
            this.definedType = definedType;
        }

        public object @object { get; private set; }
        public string name { get; private set; }

        protected override object rawValue
        {
            get
            {
                return @object;
            }
            set
            {
                if (name == null)
                {
                    throw new NotSupportedException("Cannot change the value of a static object.");
                }

                @object = value;
            }
        }

        protected override void OnValueTypeChange(Type previousType)
        {
            if (@object is UnityObject)
            {
                label = new GUIContent(ObjectNames.NicifyVariableName(((UnityObject)@object).name));
            }
            else
            {
                label = new GUIContent(valueType.HumanName());
            }

            label.tooltip = valueType.Summary();

            base.OnValueTypeChange(previousType);
        }

        protected override string SubpathToString()
        {
            return name ?? valueType.Name;
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return Empty<Attribute>.array;
        }
    }
}
