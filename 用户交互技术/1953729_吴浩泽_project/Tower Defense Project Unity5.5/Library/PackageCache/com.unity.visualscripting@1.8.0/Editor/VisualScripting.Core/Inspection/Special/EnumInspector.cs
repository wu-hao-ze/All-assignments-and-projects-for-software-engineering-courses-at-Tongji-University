using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class EnumInspector : Inspector
    {
        public EnumInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            metadata.instantiate = true;

            base.Initialize();
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            var enumType = metadata.value.GetType();

            Enum newValue;

            if (enumType.HasAttribute<FlagsAttribute>(false))
            {
                newValue = EditorGUI.EnumFlagsField(fieldPosition, (Enum)metadata.value);
            }
            else
            {
                newValue = EditorGUI.EnumPopup(fieldPosition, (Enum)metadata.value);
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

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            var enumType = metadata.value.GetType();

            Enum newValue;

            if (enumType.HasAttribute<FlagsAttribute>(false))
            {
                newValue = EditorGUI.EnumFlagsField(fieldPosition, label, (Enum)metadata.value);
            }
            else
            {
                newValue = EditorGUI.EnumPopup(fieldPosition, label, (Enum)metadata.value);
            }

            if (EndBlock(metadata))
                metadata.RecordUndo();
            metadata.value = newValue;
        }

        public override float GetAdaptiveWidth()
        {
            return Mathf.Max(18, EditorStyles.popup.CalcSize(LudiqGUI.GetEnumPopupContent((Enum)metadata.value)).x + 1);
        }
    }
}
