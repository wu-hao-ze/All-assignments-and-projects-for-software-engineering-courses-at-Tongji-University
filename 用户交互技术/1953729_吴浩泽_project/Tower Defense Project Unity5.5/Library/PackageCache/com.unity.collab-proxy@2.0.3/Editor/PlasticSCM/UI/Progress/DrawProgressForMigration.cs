using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal static class DrawProgressForMigration
    {
        internal static void For(ProgressControlsForMigration.Data data)
        {
            Rect rect = GUILayoutUtility.GetRect(
                GUILayoutUtility.GetLastRect().width, 30);

            if (!string.IsNullOrEmpty(data.NotificationMessage))
            {
                DoNotificationMessage(rect, data.NotificationMessage, data.NotificationType);
                return;
            }

            if (!data.IsOperationRunning)
                return;

            if (data.ProgressPercent == 0)
            {
                DoProgressMessage(rect, data.HeaderMessage);
                return;
            }

            DoProgressMessage(rect, data.ProgressMessage);
            DoProgressBar(rect, data.HeaderMessage, data.ProgressPercent);
        }

        static void DoNotificationMessage(
            Rect rect,
            string notificationMessage,
            MessageType notificationType)
        {
            Rect notificationRect = new Rect(
                rect.xMin + 5, rect.yMin + 10, rect.width - 10, 30);
            EditorGUI.HelpBox(notificationRect, notificationMessage, notificationType);
        }

        static void DoProgressMessage(
             Rect rect,
             string progressMessage)
        {
            Rect messageRect = new Rect(
                rect.xMin + 10, rect.yMin, rect.width - 20, 16);

            GUI.Label(messageRect, progressMessage, EditorStyles.miniLabel);
        }

        static void DoProgressBar(
            Rect rect,
            string progressMessage,
             float progressPercent)
        {
            Rect messageRect = new Rect(
                rect.xMin+10, rect.yMin + 35, rect.width-20, 16);
            Rect progresRect = new Rect(
                rect.xMin+10, rect.yMin + 27, rect.width-20, 6);
 
            EditorGUI.ProgressBar(progresRect, progressPercent, string.Empty);
            GUI.Label(messageRect, progressMessage, EditorStyles.miniLabel);
        }
    }
}
