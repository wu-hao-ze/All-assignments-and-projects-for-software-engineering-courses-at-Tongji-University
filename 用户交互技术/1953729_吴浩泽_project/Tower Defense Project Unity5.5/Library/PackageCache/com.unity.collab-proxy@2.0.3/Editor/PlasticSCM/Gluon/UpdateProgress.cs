using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;

namespace Unity.PlasticSCM.Editor.Gluon
{
    internal class UpdateProgress
    {
        internal UpdateProgress(WorkspaceWindow workspaceWindow)
        {
            mWorkspaceWindow = workspaceWindow;
        }

        internal void Cancel()
        {
            if (mUpdateProgress == null)
                return;

            mUpdateProgress.Cancel();
        }

        internal void SetCancellable(bool bCancelable)
        {
            mWorkspaceWindow.Progress.CanCancelProgress = bCancelable;
        }

        internal void RefreshProgress(
            Codice.Client.BaseCommands.UpdateProgress progress,
            UpdateProgressData updateProgressData)
        {
            mUpdateProgress = progress;

            mWorkspaceWindow.Progress.ProgressHeader = updateProgressData.Details;

            mWorkspaceWindow.Progress.TotalProgressMessage = updateProgressData.Status;
            mWorkspaceWindow.Progress.TotalProgressPercent = updateProgressData.ProgressValue / 100;
        }

        Codice.Client.BaseCommands.UpdateProgress mUpdateProgress;

        WorkspaceWindow mWorkspaceWindow;
    }
}
