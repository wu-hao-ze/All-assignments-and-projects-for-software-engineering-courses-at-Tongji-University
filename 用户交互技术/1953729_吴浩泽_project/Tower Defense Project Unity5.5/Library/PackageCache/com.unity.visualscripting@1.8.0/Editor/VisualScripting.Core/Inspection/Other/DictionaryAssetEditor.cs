using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(DictionaryAsset))]
    public sealed class DictionaryAssetEditor : Inspector
    {
        public DictionaryAssetEditor(Metadata metadata) : base(metadata) { }

        private Metadata dictionaryMetadata => metadata[nameof(DictionaryAsset.dictionary)];

        protected override float GetHeight(float width, GUIContent label)
        {
            return LudiqGUI.GetInspectorHeight(this, dictionaryMetadata, width, GUIContent.none);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            LudiqGUI.Inspector(dictionaryMetadata, position, GUIContent.none);
        }
    }
}
