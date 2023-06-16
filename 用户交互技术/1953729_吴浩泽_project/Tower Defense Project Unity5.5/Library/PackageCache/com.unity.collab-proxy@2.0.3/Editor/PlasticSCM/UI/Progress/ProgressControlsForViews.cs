using UnityEditor;

using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI.Progress
{
    internal class ProgressControlsForViews : IProgressControls
    {
        internal class Data
        {
            internal bool IsOperationRunning;
            internal float ProgressPercent;
            internal string ProgressMessage;

            internal MessageType NotificationType;
            internal string NotificationMessage;

            internal void CopyInto(Data other)
            {
                other.IsOperationRunning = IsOperationRunning;
                other.ProgressPercent = ProgressPercent;
                other.ProgressMessage = ProgressMessage;
                other.NotificationType = NotificationType;
                other.NotificationMessage = NotificationMessage;
            }
        }

        internal Data ProgressData { get { return mData; } }

        internal bool IsOperationRunning()
        {
            return mData.IsOperationRunning;
        }

        internal bool HasNotification()
        {
            return !string.IsNullOrEmpty(mData.NotificationMessage);
        }

        internal void UpdateDeterminateProgress(EditorWindow parentWindow)
        {
            if (IsOperationRunning() || mRequestedRepaint)
            {
                parentWindow.Repaint();

                mRequestedRepaint = false;
            }
        }

        internal void UpdateProgress(EditorWindow parentWindow)
        {
            if (IsOperationRunning() || mRequestedRepaint)
            {
                if (IsOperationRunning())
                    UpdateIndeterminateProgress();

                parentWindow.Repaint();

                mRequestedRepaint = false;
            }
        }

        void IProgressControls.HideProgress()
        {
            HideNotification();

            mData.IsOperationRunning = false;
            mData.ProgressMessage = string.Empty;

            mRequestedRepaint = true;
        }

        void IProgressControls.ShowProgress(string message)
        {
            HideNotification();

            mData.IsOperationRunning = true;
            mData.ProgressMessage = message;

            mRequestedRepaint = true;
        }

        void IProgressControls.ShowError(string message)
        {
            mData.NotificationMessage = message;
            mData.NotificationType = MessageType.Error;

            mRequestedRepaint = true;
        }

        void IProgressControls.ShowNotification(string message)
        {
            mData.NotificationMessage = message;
            mData.NotificationType = MessageType.Info;

            mRequestedRepaint = true;
        }

        void IProgressControls.ShowSuccess(string message)
        {
            mData.NotificationMessage = message;
            mData.NotificationType = MessageType.Info;

            mRequestedRepaint = true;
        }

        void IProgressControls.ShowWarning(string message)
        {
            mData.NotificationMessage = message;
            mData.NotificationType = MessageType.Warning;

            mRequestedRepaint = true;
        }

        void HideNotification()
        {
            mData.NotificationMessage = string.Empty;
            mData.NotificationType = MessageType.None;

            mRequestedRepaint = true;
        }

        void UpdateIndeterminateProgress()
        {
            // NOTE(rafa): there is no support for indeterminate progress bar
            // i use this neverending progress bar as workaround

            mData.ProgressPercent += .003f;

            if (mData.ProgressPercent > 1f)
                mData.ProgressPercent = .1f;
        }

        Data mData = new Data();

        bool mRequestedRepaint;
    }
}
