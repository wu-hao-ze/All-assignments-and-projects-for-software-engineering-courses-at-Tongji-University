using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Uses the USS style defined in `Editor/Stylesheets/Extensions/common.uss`.
    // See `ReadMe-USS-Styles.md` for more details.
    [CustomStyle("AnnotationStyle")]
    [DisplayName("Annotation")]
    public class AnnotationMarker : Marker // Represents the serialized data for a marker.
    {
        public string title;
        public Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        public bool showLineOverlay = true;
        [TextArea(10, 15)] public string description;
    }
}
