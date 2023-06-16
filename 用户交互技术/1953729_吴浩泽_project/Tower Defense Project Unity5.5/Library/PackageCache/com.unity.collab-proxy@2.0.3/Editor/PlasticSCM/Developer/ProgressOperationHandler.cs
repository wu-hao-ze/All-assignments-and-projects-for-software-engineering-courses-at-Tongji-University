using Codice.Client.BaseCommands;
using Codice.Client.Commands.CheckIn;
using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow;

namespace Unity.PlasticSCM.Editor.Developer
{
    internal class ProgressOperationHandler
    {
        internal ProgressOperationHandler(WorkspaceInfo wkInfo, WorkspaceWindow workspaceWindow)
        {
            mWkInfo = wkInfo;
            mWorkspaceWindow = workspaceWindow;
        }

        internal void Update(double elapsedSeconds)
        {
            if (mUpdateProgress == null)
                return;

            mSecondsSinceLastProgressUpdate += elapsedSeconds;
            if (mSecondsSinceLastProgressUpdate > UPDATE_INTERVAL_SECONDS)
            {
                mUpdateProgress.OnUpdateProgress();
                mSecondsSinceLastProgressUpdate -= UPDATE_INTERVAL_SECONDS;
            }
        }

        internal bool CheckOperationInProgress()
        {
            if (IsOperationInProgress())
            {
                GuiMessage.ShowInformation(
                    PlasticLocalization.GetString(PlasticLocalization.Name.OperationRunning),
                    PlasticLocalization.GetString(PlasticLocalization.Name.OperationInProgress));
                return true;
            }

            return false;
        }

        internal bool IsOperationInProgress()
        {
            return mProgress != null
                || mUpdateProgress != null
                || mCheckinProgress != null;
        }

        internal void ShowProgress()
        {
            mProgress = new GenericProgress(mWorkspaceWindow);
        }

        internal void RefreshProgress(ProgressData progressData)
        {
            mProgress.RefreshProgress(progressData);
        }

        internal void EndProgress()
        {
            mProgress = null;
            mWorkspaceWindow.Progress.ResetProgress();
            mWorkspaceWindow.RequestRepaint();
        }

        internal void ShowUpdateProgress(string title, UpdateNotifier notifier)
        {
            mUpdateProgress = new UpdateProgress(
                notifier, mWkInfo.ClientPath, title, mWorkspaceWindow);
            mUpdateProgress.OnUpdateProgress();
            mSecondsSinceLastProgressUpdate = 0;
        }

        internal void ShowCheckinProgress()
        {
            mCheckinProgress = new CheckinProgress(mWkInfo, mWorkspaceWindow);
        }

        internal void RefreshCheckinProgress(
            CheckinStatus checkinStatus,
            BuildProgressSpeedAndRemainingTime.ProgressData progressData)
        {
            mCheckinProgress.Refresh(checkinStatus, progressData);
        }

        internal void CancelCheckinProgress()
        {
            mCheckinProgress.CancelPressed = true;
        }

        internal void EndUpdateProgress()
        {
            mUpdateProgress = null;
            mWorkspaceWindow.Progress.ResetProgress();
            mWorkspaceWindow.RequestRepaint();
        }

        internal void EndCheckinProgress()
        {
            mCheckinProgress = null;
            mWorkspaceWindow.Progress.ResetProgress();
            mWorkspaceWindow.RequestRepaint();
        }

        internal bool HasCheckinCancelled()
        {
            return mCheckinProgress.CancelPressed;
        }

        double mSecondsSinceLastProgressUpdate = 0;

        GenericProgress mProgress;
        UpdateProgress mUpdateProgress;
        CheckinProgress mCheckinProgress;
        WorkspaceInfo mWkInfo;

        WorkspaceWindow mWorkspaceWindow;

        const double UPDATE_INTERVAL_SECONDS = 0.5;
    }
}
