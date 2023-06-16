using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class CustomPropertyDrawerInspector : Inspector
    {
        public CustomPropertyDrawerInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            property = SerializedPropertyUtility.CreateTemporaryProperty(metadata.definedType);
            propertyType = property.GetUnderlyingType();

            var adaptiveWidthAttribute = propertyType.GetAttribute<InspectorAdaptiveWidthAttribute>();
            _adaptiveWidth = adaptiveWidthAttribute?.width ?? 200;
        }

        private float _adaptiveWidth;

        private SerializedProperty property;

        private Type propertyType;

        public override void Dispose()
        {
            SerializedPropertyUtility.DestroyTemporaryProperty(property);
            base.Dispose();
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        protected override bool cacheHeight => false;

        protected override void OnGUI(Rect position, GUIContent label)
        {
            if (!propertyType.IsAssignableFrom(metadata.valueType))
            {
                if (propertyType.IsValueType)
                {
                    metadata.value = Activator.CreateInstance(propertyType);
                }
                else
                {
                    metadata.value = null;
                }
            }

            property.SetUnderlyingValue(metadata.value);

            property.serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(position, property, label);

            property.serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                metadata.RecordUndo();
                metadata.value = property.GetUnderlyingValue();
            }
        }

        public override float GetAdaptiveWidth()
        {
            return _adaptiveWidth;
        }
    }
}
