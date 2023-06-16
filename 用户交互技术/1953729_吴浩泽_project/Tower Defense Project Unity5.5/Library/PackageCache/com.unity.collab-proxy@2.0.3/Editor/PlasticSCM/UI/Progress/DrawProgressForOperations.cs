using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal static class DrawProgressForOperations
    {
        internal static void For(
            WorkspaceWindow workspaceWindow,
            OperationProgressData operationProgressData,
            float width)
        {
            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox, GUILayout.Height(60));

            GUILayout.Label(
                operationProgressData.ProgressHeader ?? string.Empty,
                EditorStyles.miniLabel);

            DoProgressBar(
                operationProgressData.TotalProgressMessage,
                (float)operationProgressData.TotalProgressPercent,
                operationProgressData.CanCancelProgress, width);

            if (operationProgressData.CanCancelProgress)
                DoCancelButton(workspaceWindow);

            if (operationProgressData.ShowCurrentBlock)
            {
                GUILayout.Space(6);
                DoProgressBar(
                    operationProgressData.CurrentBlockProgressMessage,
                    (float)operationProgressData.CurrentBlockProgressPercent,
                    operationProgressData.CanCancelProgress, width);
            }

            EditorGUILayout.EndVertical();
        }

        static void DoProgressBar(
            string message,
            float progressPercent,
            bool canCancel,
            float width)
        {
            Rect progressRect = GUILayoutUtility.GetRect(width, 15);

            if (canCancel)
                progressRect.width -= UnityConstants.CANCEL_BUTTON_SIZE + 2;

            EditorGUI.ProgressBar(progressRect, progressPercent, string.Empty);

            progressRect.xMin += 4;

            GUI.Label(progressRect, message, EditorStyles.miniLabel);
        }

        static void DoCancelButton(
            WorkspaceWindow workspaceWindow)
        {
            Rect beginRect = GUILayoutUtility.GetLastRect();
            Rect endRect = GUILayoutUtility.GetLastRect();

            float freeVerticalSpace = endRect.yMax - beginRect.yMin;

            Rect cancelButtonRect = new Rect(
                endRect.xMax - UnityConstants.CANCEL_BUTTON_SIZE + 1,
                beginRect.yMin + (freeVerticalSpace - UnityConstants.CANCEL_BUTTON_SIZE) / 2,
                UnityConstants.CANCEL_BUTTON_SIZE, UnityConstants.CANCEL_BUTTON_SIZE);

            if (!GUI.Button(cancelButtonRect, GUIContent.none, UnityStyles.CancelButton))
                return;

            workspaceWindow.CancelCurrentOperation();
        }
    }
}
