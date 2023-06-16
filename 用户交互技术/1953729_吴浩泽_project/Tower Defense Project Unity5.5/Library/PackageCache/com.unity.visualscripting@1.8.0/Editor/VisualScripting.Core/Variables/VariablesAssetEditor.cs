using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(VariablesAsset))]
    public sealed class VariablesAssetEditor : Inspector
    {
        public VariablesAssetEditor(Metadata metadata) : base(metadata) { }

        private Metadata declarationsMetadata => metadata[nameof(VariablesAsset.declarations)];

        protected override float GetHeight(float width, GUIContent label)
        {
            return LudiqGUI.GetInspectorHeight(this, declarationsMetadata, width, GUIContent.none);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            LudiqGUI.Inspector(declarationsMetadata, position, GUIContent.none);
        }
    }
}
