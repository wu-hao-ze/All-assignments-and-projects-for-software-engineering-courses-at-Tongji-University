using PlasticGui.WorkspaceWindow;

namespace Unity.PlasticSCM.Editor.Developer
{
    internal class GenericProgress
    {
        internal GenericProgress(WorkspaceWindow workspaceWindow)
        {
            mWorkspaceWindow = workspaceWindow;
            mWorkspaceWindow.Progress.CanCancelProgress = false;
        }

        internal void RefreshProgress(ProgressData progressData)
        {
            var progress = mWorkspaceWindow.Progress;

            progress.ProgressHeader = progressData.Status;
            progress.TotalProgressMessage = progressData.Details;
            progress.TotalProgressPercent = progressData.ProgressValue / 100f;
        }

        WorkspaceWindow mWorkspaceWindow;
    }
}
