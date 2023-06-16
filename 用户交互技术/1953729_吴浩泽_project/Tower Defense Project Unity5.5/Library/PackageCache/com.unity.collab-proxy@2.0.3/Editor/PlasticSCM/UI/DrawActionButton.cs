using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawActionButton
    {
        internal static bool For(string buttonText)
        {
            GUIContent buttonContent = new GUIContent(buttonText);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

            buttonStyle.stretchWidth = false;

            Rect rt = GUILayoutUtility.GetRect(
                buttonContent,
                buttonStyle,
                GUILayout.MinWidth(UnityConstants.REGULAR_BUTTON_WIDTH));

            return GUI.Button(rt, buttonText, buttonStyle);
        }

        internal static bool ForCommentSection(string buttonText)
        {
            GUIContent buttonContent = new GUIContent(buttonText);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

            buttonStyle.stretchWidth = false;

            var width = MeasureMaxWidth.ForTexts(buttonStyle, buttonText);

            Rect rt = GUILayoutUtility.GetRect(
                buttonContent,
                buttonStyle,
                GUILayout.MinWidth(width),
                GUILayout.MaxWidth(width));

            return GUI.Button(rt, buttonText, buttonStyle);
        }
    }
}
