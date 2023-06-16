using UnityEngine;

namespace Unity.VisualScripting
{
    public enum GraphSource
    {
        Embed,
#if UNITY_2019_4_OR_NEWER
        [InspectorName("Graph")]
#endif
        Macro
    }
}
