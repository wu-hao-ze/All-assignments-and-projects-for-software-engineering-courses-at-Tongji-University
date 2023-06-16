using GluonGui.WorkspaceWindow.Views.Checkin.Operations;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;

namespace Unity.PlasticSCM.Editor.Gluon
{
    internal class ProgressOperationHandler : IUpdateProgress, ICheckinProgress
    {
        internal ProgressOperationHandler(WorkspaceWindow workspaceWindow)
        {
            mWorkspaceWindow = workspaceWindow;
        }

        internal bool IsOperationInProgress()
        {
            return mUpdateProgress != null
                || mCheckinProgress != null;
        }

        internal void CancelUpdateProgress()
        {
            mUpdateProgress.Cancel();
        }

        void ICheckinProgress.ShowProgress()
        {
            mCheckinProgress = new CheckinProgress(mWorkspaceWindow);
        }

        void ICheckinProgress.RefreshProgress(CheckinProgressData progress)
        {
            mCheckinProgress.Refresh(progress);
        }

        void ICheckinProgress.EndProgress()
        {
            mCheckinProgress = null;
            mWorkspaceWindow.Progress.ResetProgress();
            mWorkspaceWindow.RequestRepaint();
        }

        void IUpdateProgress.ShowNoCancelableProgress()
        {
            mUpdateProgress = new UpdateProgress(mWorkspaceWindow);
            mUpdateProgress.SetCancellable(false);
        }

        void IUpdateProgress.ShowCancelableProgress()
        {
            mUpdateProgress = new UpdateProgress(mWorkspaceWindow);
            mUpdateProgress.SetCancellable(true);
        }

        void IUpdateProgress.RefreshProgress(
            Codice.Client.BaseCommands.UpdateProgress updateProgress,
            UpdateProgressData updateProgressData)
        {
            mUpdateProgress.RefreshProgress(updateProgress, updateProgressData);
        }

        void IUpdateProgress.EndProgress()
        {
            mUpdateProgress = null;
            mWorkspaceWindow.Progress.ResetProgress();
            mWorkspaceWindow.RequestRepaint();
        }

        UpdateProgress mUpdateProgress;
        CheckinProgress mCheckinProgress;

        WorkspaceWindow mWorkspaceWindow;
    }
}
