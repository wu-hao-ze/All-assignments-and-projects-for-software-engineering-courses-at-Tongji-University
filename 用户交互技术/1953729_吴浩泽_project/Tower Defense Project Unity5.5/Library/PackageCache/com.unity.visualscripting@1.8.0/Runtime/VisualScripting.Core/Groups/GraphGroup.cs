using UnityEngine;

namespace Unity.VisualScripting
{
    [SerializationVersion("A")]
    public sealed class GraphGroup : GraphElement<IGraph>
    {
        [DoNotSerialize]
        public static readonly Color defaultColor = new Color(0, 0, 0);

        public GraphGroup() : base() { }

        [Serialize]
        public Rect position { get; set; }

        [Serialize]
        public string label { get; set; } = "Group";

        [Serialize]
        [InspectorTextArea(minLines = 1, maxLines = 10)]
        public string comment { get; set; }

        [Serialize]
        [Inspectable]
        public Color color { get; set; } = defaultColor;
    }
}
