using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(string))]
    public class StringInspector : Inspector
    {
        public StringInspector(Metadata metadata) : base(metadata) { }

        protected override bool cacheHeight => !metadata.HasAttribute<InspectorTextAreaAttribute>();

        private float GetFieldHeight(float width, GUIContent label)
        {
            if (metadata.HasAttribute<InspectorTextAreaAttribute>())
            {
                var attribute = metadata.GetAttribute<InspectorTextAreaAttribute>();

                var height = LudiqStyles.textAreaWordWrapped.CalcHeight(new GUIContent((string)metadata.value), WidthWithoutLabel(metadata, width, label));

                if (attribute.hasMinLines)
                {
                    var minHeight = EditorStyles.textArea.lineHeight * attribute.minLines + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

                    height = Mathf.Max(height, minHeight);
                }

                if (attribute.hasMaxLines)
                {
                    var maxHeight = EditorStyles.textArea.lineHeight * attribute.maxLines + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

                    height = Mathf.Min(height, maxHeight);
                }

                return height;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, GetFieldHeight(width, label), label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var fieldPosition = position.VerticalSection(ref y, GetFieldHeight(position.width, GUIContent.none));

            string newValue;

            if (metadata.HasAttribute<InspectorTextAreaAttribute>())
            {
                newValue = EditorGUI.TextArea(fieldPosition, (string)metadata.value, EditorStyles.textArea);
            }
            else if (metadata.HasAttribute<InspectorDelayedAttribute>())
            {
                newValue = EditorGUI.DelayedTextField(fieldPosition, (string)metadata.value, EditorStyles.textField);
            }
            else
            {
                newValue = EditorGUI.TextField(fieldPosition, (string)metadata.value, EditorStyles.textField);
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        public override float GetAdaptiveWidth()
        {
            return LudiqGUI.GetTextFieldAdaptiveWidth(metadata.value);
        }
    }
}
