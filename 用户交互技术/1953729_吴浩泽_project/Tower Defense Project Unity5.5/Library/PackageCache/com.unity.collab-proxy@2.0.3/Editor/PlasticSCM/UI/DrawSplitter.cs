using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawSplitter
    {
        internal static void ForHorizontalIndicator()
        {
            ForWidth(EditorGUIUtility.currentViewWidth);
        }

        internal static void ForWidth(float width)
        {
            GUIStyle style = UnityStyles.SplitterIndicator;

            Rect splitterRect = GUILayoutUtility.GetRect(
                width,
                UnityConstants.SPLITTER_INDICATOR_HEIGHT,
                style);

            GUI.Label(splitterRect, string.Empty, style);   
        }
    }
}
