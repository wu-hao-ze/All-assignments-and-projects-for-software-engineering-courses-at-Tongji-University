using UnityEngine;

namespace Unity.VisualScripting
{
    public struct InspectorBlock
    {
        public InspectorBlock(Metadata metadata, Rect position)
        {
            this.metadata = metadata;
            this.position = position;
        }

        public Metadata metadata { get; private set; }
        public Rect position { get; private set; }
    }
}
