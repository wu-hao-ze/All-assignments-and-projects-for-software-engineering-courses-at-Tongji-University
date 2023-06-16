using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class GUISpace
    {
        internal static void ForToolbar()
        {
#if UNITY_2019_1_OR_NEWER
            GUILayout.Space(5);
#endif
        }
    }
}
