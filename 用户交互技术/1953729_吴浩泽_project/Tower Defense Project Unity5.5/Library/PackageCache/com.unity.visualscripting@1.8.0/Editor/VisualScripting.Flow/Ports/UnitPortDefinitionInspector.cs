using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(IUnitPortDefinition))]
    public class UnitPortDefinitionInspector : Inspector
    {
        public UnitPortDefinitionInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            metadata.instantiate = true;

            base.Initialize();
        }

        private Metadata keyMetadata => metadata[nameof(IUnitPortDefinition.key)];
        private Metadata labelMetadata => metadata[nameof(IUnitPortDefinition.label)];
        private Metadata descriptionMetadata => metadata[nameof(IUnitPortDefinition.summary)];
        private Metadata hideLabelMetadata => metadata[nameof(IUnitPortDefinition.hideLabel)];

        protected float GetKeyHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, keyMetadata, width);
        }

        protected float GetLabelHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, labelMetadata, width);
        }

        protected float GetDescriptionHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, descriptionMetadata, width);
        }

        protected float GetHideLabelHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, hideLabelMetadata, width);
        }

        protected void OnKeyGUI(Rect position)
        {
            LudiqGUI.Inspector(keyMetadata, position);
        }

        protected void OnLabelGUI(Rect position)
        {
            LudiqGUI.Inspector(labelMetadata, position);
        }

        protected void OnDescriptionGUI(Rect position)
        {
            LudiqGUI.Inspector(descriptionMetadata, position);
        }

        protected void OnHideLabelGUI(Rect position)
        {
            LudiqGUI.Inspector(hideLabelMetadata, position);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            height += GetKeyHeight(width);

            height += EditorGUIUtility.standardVerticalSpacing;

            height += GetLabelHeight(width);

            height += EditorGUIUtility.standardVerticalSpacing;

            height += GetDescriptionHeight(width);

            height += EditorGUIUtility.standardVerticalSpacing;

            height += GetHideLabelHeight(width);

            return height;
        }

        protected virtual void OnFieldsGUI(Rect position)
        {
            OnKeyGUI(position.VerticalSection(ref y, GetKeyHeight(position.width)));

            y += EditorGUIUtility.standardVerticalSpacing;

            OnLabelGUI(position.VerticalSection(ref y, GetLabelHeight(position.width)));

            y += EditorGUIUtility.standardVerticalSpacing;

            OnDescriptionGUI(position.VerticalSection(ref y, GetDescriptionHeight(position.width)));

            y += EditorGUIUtility.standardVerticalSpacing;

            OnHideLabelGUI(position.VerticalSection(ref y, GetHideLabelHeight(position.width)));
        }

        protected sealed override void OnGUI(Rect position, GUIContent label)
        {
            BeginLabeledBlock(metadata, position, label);

            OnFieldsGUI(position);

            EndBlock(metadata);
        }
    }
}
