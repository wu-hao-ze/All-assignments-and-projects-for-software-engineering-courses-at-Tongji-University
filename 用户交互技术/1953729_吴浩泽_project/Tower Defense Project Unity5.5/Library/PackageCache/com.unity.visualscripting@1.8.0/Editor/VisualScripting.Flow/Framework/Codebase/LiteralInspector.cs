using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Literal))]
    public sealed class LiteralInspector : Inspector
    {
        public LiteralInspector(Metadata metadata) : base(metadata) { }

        private Metadata typeMetadata => metadata[nameof(Literal.type)];
        private Metadata valueMetadata => metadata[nameof(Literal.value)];
        private Metadata typedValueMetadata => valueMetadata.Cast((Type)typeMetadata.value);

        private bool hasType => typeMetadata.value != null;

        protected override float GetHeight(float width, GUIContent label)
        {
            if (hasType)
            {
                return LudiqGUI.GetInspectorHeight(this, typedValueMetadata, width, label);
            }
            else
            {
                return LudiqGUI.GetInspectorHeight(this, typeMetadata, width, label);
            }
        }

        protected override bool cacheHeight => false;

        protected override void OnGUI(Rect position, GUIContent label)
        {
            if (hasType)
            {
                LudiqGUI.Inspector(typedValueMetadata, position, label);
            }
            else
            {
                LudiqGUI.Inspector(typeMetadata, position, label);
            }
        }

        public override float GetAdaptiveWidth()
        {
            if (hasType)
            {
                return typedValueMetadata.Inspector().GetAdaptiveWidth();
            }
            else
            {
                return typeMetadata.Inspector().GetAdaptiveWidth();
            }
        }
    }
}
