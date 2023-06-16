using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class NullableInspector : Inspector
    {
        public NullableInspector(Metadata metadata) : base(metadata)
        {
            var underlyingType = Nullable.GetUnderlyingType(metadata.definedType);

            underlyingMetadata = metadata.Object("__underlying", metadata.value ?? Activator.CreateInstance(underlyingType), underlyingType);
        }

        private readonly Metadata underlyingMetadata;

        protected override float GetHeight(float width, GUIContent label)
        {
            return LudiqGUI.GetInspectorHeight(this, underlyingMetadata, width, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var togglePosition = new Rect
                (
                position.x,
                position.y,
                EditorGUIUtility.singleLineHeight,
                position.height
                );

            var fieldPosition = new Rect
                (
                togglePosition.xMax + Styles.toggleSpacing,
                position.y,
                position.width - togglePosition.width - Styles.toggleSpacing,
                position.height
                );

            var hasValue = EditorGUI.Toggle(togglePosition, GUIContent.none, metadata.value != null);

            EditorGUI.BeginDisabledGroup(!hasValue);
            LudiqGUI.Inspector(underlyingMetadata, fieldPosition, GUIContent.none);
            EditorGUI.EndDisabledGroup();

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = hasValue ? underlyingMetadata.value : null;
            }
        }

        public static class Styles
        {
            public static readonly int toggleSpacing = 3;
        }
    }
}
