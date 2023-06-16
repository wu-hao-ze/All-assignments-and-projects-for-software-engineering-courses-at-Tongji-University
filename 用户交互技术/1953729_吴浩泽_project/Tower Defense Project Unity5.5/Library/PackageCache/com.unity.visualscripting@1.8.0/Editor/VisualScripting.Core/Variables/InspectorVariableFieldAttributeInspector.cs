using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(InspectorVariableNameAttribute))]
    public class VariableNameAttributeInspector : Inspector
    {
        public VariableNameAttributeInspector(Metadata metadata) : base(metadata)
        {
            if (metadata.definedType != typeof(string))
            {
                throw new NotSupportedException($"'{nameof(InspectorVariableNameAttribute)}' can only be used on strings.");
            }

            direction = metadata.GetAttribute<InspectorVariableNameAttribute>().direction;
        }

        public ActionDirection direction { get; set; }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var newValue = BoltGUI.VariableField(position, GUIContent.none, (string)metadata.value, direction);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }
    }
}
