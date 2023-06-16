using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class ContinuousNumberInspector<T> : Inspector
    {
        protected ContinuousNumberInspector(Metadata metadata) : base(metadata) { }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            T newValue;

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

                newValue = (T)Convert.ChangeType(EditorGUI.Slider(fieldPosition, Convert.ToSingle(metadata.value), rangeAttribute.min, rangeAttribute.max), typeof(T));
            }
            else
            {
                if (Is64Bits(metadata.value))
                {
                    newValue = (T)Convert.ChangeType(LudiqGUI.DraggableLongField(fieldPosition, Convert.ToInt64(metadata.value)), typeof(T));
                }
                else
                {
                    newValue = (T)Convert.ChangeType(LudiqGUI.DraggableFloatField(fieldPosition, Convert.ToSingle(metadata.value)), typeof(T));
                }
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

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight - 2
                );

            if (metadata.HasAttribute<InspectorRangeAttribute>())
            {
                var rangeAttribute = metadata.GetAttribute<InspectorRangeAttribute>();

                newValue = (T)Convert.ChangeType(EditorGUI.Slider(fieldPosition, label, Convert.ToSingle(metadata.value), rangeAttribute.min, rangeAttribute.max), typeof(T));
            }
            else
            {
                if (Is64Bits(metadata.value))
                {
                    newValue = (T)Convert.ChangeType(LudiqGUI.DraggableLongField(fieldPosition, Convert.ToInt64(metadata.value), label), typeof(T));
                }
                else
                {
                    newValue = (T)Convert.ChangeType(LudiqGUI.DraggableFloatField(fieldPosition, Convert.ToSingle(metadata.value), label), typeof(T));
                }
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        static bool Is64Bits(object value)
        {
            return value is long || value is ulong;
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
