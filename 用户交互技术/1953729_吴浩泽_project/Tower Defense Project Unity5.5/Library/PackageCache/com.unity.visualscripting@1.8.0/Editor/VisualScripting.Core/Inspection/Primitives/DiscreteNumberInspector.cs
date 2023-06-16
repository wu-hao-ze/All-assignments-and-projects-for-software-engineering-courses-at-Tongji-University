using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class DiscreteNumberInspector<T> : Inspector
    {
        protected DiscreteNumberInspector(Metadata metadata) : base(metadata) { }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            T newValue;
            var oldValue = Convert.ToInt32(metadata.value);

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            if (metadata.HasAttribute<InspectorRangeAttribute>())
            {
                var rangeAttribute = metadata.GetAttribute<InspectorRangeAttribute>();

                newValue = (T)Convert.ChangeType(EditorGUI.IntSlider(fieldPosition, oldValue, (int)rangeAttribute.min, (int)rangeAttribute.max), typeof(T));
            }
            else
            {
                newValue = (T)Convert.ChangeType(LudiqGUI.DraggableIntField(fieldPosition, oldValue), typeof(T));
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        protected override void OnEditorPrefGUI(Rect position, GUIContent label)
        {
            BeginBlock(metadata, position);

            T newValue;
            var oldValue = Convert.ToInt32(metadata.value);

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            if (metadata.HasAttribute<InspectorRangeAttribute>())
            {
                var rangeAttribute = metadata.GetAttribute<InspectorRangeAttribute>();

                newValue = (T)Convert.ChangeType(EditorGUI.IntSlider(fieldPosition, label, oldValue, (int)rangeAttribute.min, (int)rangeAttribute.max), typeof(T));
            }
            else
            {
                newValue = (T)Convert.ChangeType(LudiqGUI.DraggableIntField(fieldPosition, oldValue, label), typeof(T));
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        public override float GetAdaptiveWidth()
        {
            if (metadata.HasAttribute<InspectorRangeAttribute>())
            {
                return 100;
            }
            else
            {
                return LudiqGUI.GetTextFieldAdaptiveWidth(metadata.value);
            }
        }
    }
}
