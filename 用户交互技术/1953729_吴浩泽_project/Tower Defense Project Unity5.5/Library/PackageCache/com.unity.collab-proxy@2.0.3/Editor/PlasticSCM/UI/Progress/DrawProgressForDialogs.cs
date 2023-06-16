using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal static class DrawProgressForDialogs
    {
        internal static void For(ProgressControlsForDialogs.Data data)
        {
            Rect rect = GUILayoutUtility.GetRect(
                GUILayoutUtility.GetLastRect().width, 30);

            if (!string.IsNullOrEmpty(data.StatusMessage))
            {
                EditorGUI.HelpBox(rect, data.StatusMessage, data.StatusType);
                return;
            }

            if (data.IsWaitingAsyncResult)
                DoProgressBar(rect, data.ProgressMessage, data.ProgressPercent);
        }

        static void DoProgressBar(
            Rect rect,
            string progressMessage,
            float progressPercent)
        {
            Rect messageRect = new Rect(
                rect.xMin, rect.yMin + 2, rect.width, 16);
            Rect progresRect = new Rect(
                rect.xMin, rect.yMin + 20, rect.width, 6);

            GUI.Label(messageRect, progressMessage);

            EditorGUI.ProgressBar(progresRect, progressPercent, string.Empty);
        }
    }
}
