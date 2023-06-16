using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal static class DrawProgressForViews
    {
        internal static void ForNotificationArea(
            ProgressControlsForViews.Data data)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.HelpBox(
                data.NotificationMessage,
                data.NotificationType);

            EditorGUILayout.EndHorizontal();
        }

        internal static void ForIndeterminateProgress(
            ProgressControlsForViews.Data data)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            DoProgressBar(data.ProgressPercent);

            GUILayout.Space(3);

            DoProgressLabel(data.ProgressMessage);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        static void DoProgressBar(float progressPercent)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            Rect progressRect = GUILayoutUtility.GetRect(30, 10);

            EditorGUI.ProgressBar(progressRect, progressPercent, string.Empty);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();
        }

        static void DoProgressLabel(string progressMessage)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.Label(progressMessage, UnityStyles.ProgressLabel);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();
        }
    }
}