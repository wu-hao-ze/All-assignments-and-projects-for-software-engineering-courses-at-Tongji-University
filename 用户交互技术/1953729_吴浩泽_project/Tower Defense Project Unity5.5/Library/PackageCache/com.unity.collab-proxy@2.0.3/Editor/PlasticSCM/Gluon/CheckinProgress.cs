using GluonGui.WorkspaceWindow.Views.Checkin.Operations;

namespace Unity.PlasticSCM.Editor.Gluon
{
    internal class CheckinProgress
    {
        internal CheckinProgress(WorkspaceWindow workspaceWindow)
        {
            mWorkspaceWindow = workspaceWindow;
        }

        internal void Refresh(CheckinProgressData progress)
        {
            mWorkspaceWindow.Progress.ProgressHeader = progress.ProgressText;

            mWorkspaceWindow.Progress.TotalProgressMessage = progress.TotalProgressText;
            mWorkspaceWindow.Progress.TotalProgressPercent = ((double)progress.TotalProgressValue) / 100;

            mWorkspaceWindow.Progress.ShowCurrentBlock = progress.bShowCurrentBlock;
            mWorkspaceWindow.Progress.CurrentBlockProgressMessage = progress.CurrentBlockText;
            mWorkspaceWindow.Progress.CurrentBlockProgressPercent = ((double)progress.CurrentBlockProgressValue) / 100;
        }

        WorkspaceWindow mWorkspaceWindow;
    }
}
