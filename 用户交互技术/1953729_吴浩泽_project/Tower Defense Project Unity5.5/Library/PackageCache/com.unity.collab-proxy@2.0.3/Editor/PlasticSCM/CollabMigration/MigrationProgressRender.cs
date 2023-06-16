using System;
using System.IO;

using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.Client.BaseCommands;
using Codice.LogWrapper;
using PlasticGui;
using Codice.Client.BaseCommands.Sync;


namespace Unity.PlasticSCM.Editor.CollabMigration
{
    internal  class MigrationProgressRender
    {
        internal static string FixNotificationPath(string wkPath, string notification)
        {
            if (notification == null)
                return string.Empty;

            int position = notification.ToLower().IndexOf(wkPath.ToLower());

            if (position < 0)
                return notification;

            return notification.Remove(position, wkPath.Length + 1);
        }

        internal static string GetProgressString(
            CreateWorkspaceFromCollab.Progress status,
            BuildProgressSpeedAndRemainingTime.ProgressData progressData,
            DateTime now,
            double smoothingFactor,
            string updateProgressCalculatingMessage,
            string updateProgressSingularMessage,
            string updateProgressPluralMessage,
            string remainingMessage)
        {
            if (status.CurrentStatus == CreateWorkspaceFromCollab.Progress.Status.Starting)
                return updateProgressCalculatingMessage;

            progressData.StartTimerIfNotStarted(now);

            string updatedSize;
            string totalSize;
            GetFormattedSizes.ForTransfer(
                status.ProcessedSize,
                status.TotalSize,
                out updatedSize,
                out totalSize);
          
            string details = string.Format(
                status.TotalFiles == 1 ?
                    updateProgressSingularMessage :
                    updateProgressPluralMessage,
                updatedSize,
                totalSize,
                status.ProcessedFiles,
                status.TotalFiles,
                BuildProgressSpeedAndRemainingTime.ForTransfer(
                    progressData,
                    now,
                    status.TotalSize,
                    status.ProcessedSize,
                    smoothingFactor,
                    remainingMessage));

            return details;
        }

        static ILog mLog = LogManager.GetLogger("MigrationProgressRender");
    }
}
