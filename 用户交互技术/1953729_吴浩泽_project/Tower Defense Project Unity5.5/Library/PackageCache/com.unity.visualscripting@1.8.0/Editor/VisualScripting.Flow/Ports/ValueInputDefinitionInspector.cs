using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(ValueInputDefinition))]
    public sealed class ValueInputDefinitionInspector : ValuePortDefinitionInspector
    {
        public ValueInputDefinitionInspector(Metadata metadata) : base(metadata) { }

        private Metadata hasDefaultValueMetadata => metadata[nameof(ValueInputDefinition.hasDefaultValue)];
        private Metadata defaultValueMetadata => metadata[nameof(ValueInputDefinition.defaultValue)];
        private Metadata typedDefaultValueMetadata => defaultValueMetadata.Cast((Type)typeMetadata.value);

        private float GetHasDefaultValueHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, hasDefaultValueMetadata, width);
        }

        private float GetDefaultValueHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, typedDefaultValueMetadata, width);
        }

        private void OnHasDefaultValueGUI(Rect position)
        {
            LudiqGUI.Inspector(hasDefaultValueMetadata, position);
        }

        private void OnDefaultValueGUI(Rect position)
        {
            LudiqGUI.Inspector(typedDefaultValueMetadata, position);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = base.GetHeight(width, label);

            if (typeMetadata.value != null && ValueInput.SupportsDefaultValue((Type)typeMetadata.value))
            {
                height += EditorGUIUtility.standardVerticalSpacing;

                height += GetHasDefaultValueHeight(width);

                if ((bool)hasDefaultValueMetadata.value)
                {
                    height += EditorGUIUtility.standardVerticalSpacing;

                    height += GetDefaultValueHeight(width);
                }
            }

            return height;
        }

        protected override void OnFieldsGUI(Rect position)
        {
            base.OnFieldsGUI(position);

            if (typeMetadata.value != null && ValueInput.SupportsDefaultValue((Type)typeMetadata.value))
            {
                y += EditorGUIUtility.standardVerticalSpacing;

                OnHasDefaultValueGUI(position.VerticalSection(ref y, GetHasDefaultValueHeight(position.width)));

                if ((bool)hasDefaultValueMetadata.value)
                {
                    y += EditorGUIUtility.standardVerticalSpacing;

                    OnDefaultValueGUI(position.VerticalSection(ref y, GetDefaultValueHeight(position.width)));
                }
            }
        }
    }
}
