using UnityEngine;

namespace Unity.VisualScripting
{
    public class DictionaryInspector : Inspector
    {
        public DictionaryInspector(Metadata metadata) : base(metadata)
        {
            adaptor = new MetadataDictionaryAdaptor(metadata, this);
        }

        protected MetadataDictionaryAdaptor adaptor { get; private set; }

        protected override float GetHeight(float width, GUIContent label)
        {
            return adaptor.GetHeight(width, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            adaptor.Field(position, label);
        }

        public override float GetAdaptiveWidth()
        {
            return adaptor.GetAdaptiveWidth();
        }

        public static class Styles
        {
            static Styles() { }

            public static readonly float invocationSpacing = 7;
            public static readonly float addButtonWidth = 29;
        }
    }
}
