using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(ValuePortDefinition))]
    public class ValuePortDefinitionInspector : UnitPortDefinitionInspector
    {
        public ValuePortDefinitionInspector(Metadata metadata) : base(metadata) { }

        protected Metadata typeMetadata => metadata[nameof(ValueInputDefinition.type)];

        protected float GetTypeHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, typeMetadata, width);
        }

        private void OnTypeGUI(Rect position)
        {
            LudiqGUI.Inspector(typeMetadata, position);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = base.GetHeight(width, label);

            height += EditorGUIUtility.standardVerticalSpacing;

            height += GetTypeHeight(width);

            return height;
        }

        protected override void OnFieldsGUI(Rect position)
        {
            base.OnFieldsGUI(position);

            y += EditorGUIUtility.standardVerticalSpacing;

            OnTypeGUI(position.VerticalSection(ref y, GetTypeHeight(position.width)));
        }
    }
}
