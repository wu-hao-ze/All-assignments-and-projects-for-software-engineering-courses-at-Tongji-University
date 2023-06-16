using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawActionToolbar
    {
        internal static void Begin(EditorWindow parentWindow)
        {
            Rect result = GUILayoutUtility.GetRect(parentWindow.position.width, 1);
            EditorGUI.DrawRect(result, UnityStyles.Colors.BarBorder);

            EditorGUILayout.BeginVertical(UnityStyles.ActionToolbar);
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
        }

        internal static void End()
        {
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }
}