using UnityEditor;

using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal class ProgressControlsForMigration
    {
        internal class Data
        {
            internal bool IsOperationRunning;
            internal float ProgressPercent;
            internal string ProgressMessage;
            internal string HeaderMessage;

            internal MessageType NotificationType;
            internal string NotificationMessage;

            internal void CopyInto(Data other)
            {
                other.IsOperationRunning = IsOperationRunning;
                other.ProgressPercent = ProgressPercent;
                other.ProgressMessage = ProgressMessage;
                other.HeaderMessage = HeaderMessage;
                other.NotificationType = NotificationType;
                other.NotificationMessage = NotificationMessage;
            }
        }

        internal Data ProgressData { get { return mData; } }

        internal bool IsOperationRunning()
        {
            return mData.IsOperationRunning;
        }

        internal void UpdateDeterminateProgress(EditorWindow parentWindow)
        {
            if (IsOperationRunning() || mRequestedRepaint)
            {
                parentWindow.Repaint();

                mRequestedRepaint = false;
            }
        }

        internal void HideProgress()
        {
            HideNotification();

            mData.IsOperationRunning = false;
            mData.HeaderMessage = string.Empty;
            mData.ProgressMessage = string.Empty;
            mData.ProgressPercent = 0;

            mRequestedRepaint = true;
        }

        internal void ShowProgress(string header, string message, float progressPercent)
        {
            HideNotification();

            mData.IsOperationRunning = true;
            mData.HeaderMessage = header;
            mData.ProgressMessage = message;
            mData.ProgressPercent = progressPercent;

            mRequestedRepaint = true;
        }

        internal void ShowError(string message)
        {
            mData.NotificationMessage = message;
            mData.NotificationType = MessageType.Error;

            mRequestedRepaint = true;
        }

        internal void ShowSuccess(string message)
        {
            mData.NotificationMessage = message;
            mData.NotificationType = MessageType.Info;

            mRequestedRepaint = true;
        }
         
        void HideNotification()
        {
            mData.NotificationMessage = string.Empty;
            mData.NotificationType = MessageType.None;

            mRequestedRepaint = true;
        }

        Data mData = new Data();

        bool mRequestedRepaint;
    }
}
