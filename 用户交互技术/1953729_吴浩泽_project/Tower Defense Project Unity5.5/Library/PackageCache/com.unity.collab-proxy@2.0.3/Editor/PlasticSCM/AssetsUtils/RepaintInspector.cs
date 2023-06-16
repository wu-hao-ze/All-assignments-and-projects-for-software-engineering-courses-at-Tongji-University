using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class RepaintInspector
    {
        internal static void All()
        {
            UnityEditor.Editor[] editors =
                Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();

            foreach (UnityEditor.Editor editor in editors)
                editor.Repaint();
        }
    }
}
