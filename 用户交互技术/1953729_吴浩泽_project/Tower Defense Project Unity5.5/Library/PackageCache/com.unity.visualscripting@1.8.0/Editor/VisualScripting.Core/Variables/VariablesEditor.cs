using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(Variables))]
    public sealed class VariablesEditor : Inspector
    {
        public VariablesEditor(Metadata metadata) : base(metadata) { }

        private Metadata declarationsMetadata => metadata[nameof(Variables.declarations)];

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
