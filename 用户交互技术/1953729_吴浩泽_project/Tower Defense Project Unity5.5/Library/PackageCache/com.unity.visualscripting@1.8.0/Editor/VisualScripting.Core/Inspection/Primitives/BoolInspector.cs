using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(bool))]
    public class BoolInspector : Inspector
    {
        public BoolInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            bool newValue;
            var oldValue = Convert.ToBoolean(metadata.value);

            if (metadata.HasAttribute<InspectorToggleLeftAttribute>())
            {
                BeginLabeledBlock(metadata, position, new GUIContent("", null, label.tooltip));
                var togglePosition = position.VerticalSection(ref y, EditorGUIUtility.singleLineHeight);
                var labelStyle = new GUIStyle(ProcessLabelStyle(metadata, null));
                labelStyle.padding.left = 2;
                newValue = EditorGUI.ToggleLeft(togglePosition, label, oldValue, labelStyle);
            }
            else
            {
                position = BeginLabeledBlock(metadata, position, label);
                var togglePosition = position.VerticalSection(ref y, EditorGUIUtility.singleLineHeight);
                newValue = EditorGUI.Toggle(togglePosition, oldValue);
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
            var togglePosition = position.VerticalSection(ref y, EditorGUIUtility.singleLineHeight);
            bool newValue = EditorGUI.Toggle(togglePosition, label, (bool)metadata.value);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        public override float GetAdaptiveWidth()
        {
            if (metadata.HasAttribute<InspectorToggleLeftAttribute>())
            {
                return 20;
            }
            else
            {
                return 14;
            }
        }
    }
}
