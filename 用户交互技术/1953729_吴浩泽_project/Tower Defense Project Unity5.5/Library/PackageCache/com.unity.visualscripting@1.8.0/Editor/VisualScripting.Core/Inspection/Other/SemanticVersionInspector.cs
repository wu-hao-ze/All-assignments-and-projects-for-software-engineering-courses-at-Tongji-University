using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(SemanticVersion))]
    public sealed class SemanticVersionInspector : Inspector
    {
        public SemanticVersionInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            SemanticVersion newSemanticVersion;

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            if (!SemanticVersion.TryParse(EditorGUI.DelayedTextField(fieldPosition, ((SemanticVersion)metadata.value).ToString()), out newSemanticVersion))
            {
                newSemanticVersion = (SemanticVersion)metadata.value;
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newSemanticVersion;
            }
        }
    }
}
