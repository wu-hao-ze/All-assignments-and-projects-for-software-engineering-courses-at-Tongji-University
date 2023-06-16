using System;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Update;

namespace Unity.PlasticSCM.Editor.Developer
{
    internal class UpdateProgress
    {
        internal UpdateProgress(
            UpdateNotifier notifier, string wkPath, string title,
            WorkspaceWindow workspaceWindow)
        {
            mNotifier = notifier;
            mWkPath = wkPath;
            mWorkspaceWindow = workspaceWindow;

            mProgressData = new BuildProgressSpeedAndRemainingTime.ProgressData(DateTime.Now);

            mWorkspaceWindow.Progress.ProgressHeader = title;
            mWorkspaceWindow.Progress.CanCancelProgress = false;
        }

        internal void OnUpdateProgress()
        {
            var progress = mWorkspaceWindow.Progress;

            progress.ProgressHeader = UpdateProgressRender.FixNotificationPath(
                mWkPath, mNotifier.GetNotificationMessage());

            UpdateOperationStatus status = mNotifier.GetUpdateStatus();

            progress.TotalProgressMessage = UpdateProgressRender.GetProgressString(
                status, mProgressData);

            progress.TotalProgressPercent = GetProgressBarPercent.ForTransfer(
                status.UpdatedSize, status.TotalSize) / 100f;
        }

        readonly BuildProgressSpeedAndRemainingTime.ProgressData mProgressData;
        readonly WorkspaceWindow mWorkspaceWindow;
        readonly string mWkPath;
        readonly UpdateNotifier mNotifier;
    }
}
