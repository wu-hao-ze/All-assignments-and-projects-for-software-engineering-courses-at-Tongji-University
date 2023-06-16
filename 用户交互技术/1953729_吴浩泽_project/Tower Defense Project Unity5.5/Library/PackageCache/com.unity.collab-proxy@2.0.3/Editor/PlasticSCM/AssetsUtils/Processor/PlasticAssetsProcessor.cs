using System;
using System.Collections.Generic;

using Codice.LogWrapper;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    internal class PlasticAssetsProcessor : WorkspaceOperationsMonitor.IDisableAssetsProcessor
    {
        internal void SetWorkspaceOperationsMonitor(
            WorkspaceOperationsMonitor workspaceOperationsMonitor)
        {
            mWorkspaceOperationsMonitor = workspaceOperationsMonitor;
        }

        internal void AddToSourceControl(List<string> paths)
        {
            if (paths.Count == 0)
                return;

            if (IsDisableBecauseExceptionHappened(DateTime.Now))
            {
                mLog.Warn(
                    "PlasticAssetsProcessor skipping AddToSourceControl operation " +
                    "because an exception happened in the last 60 seconds");
                return;
            }

            foreach (string path in paths)
                mLog.DebugFormat("AddToSourceControl: {0}", path);

            mWorkspaceOperationsMonitor.AddAssetsProcessorPathsToAdd(paths);
        }

        internal void DeleteFromSourceControl(List<string> paths)
        {
            if (paths.Count == 0)
                return;

            if (IsDisableBecauseExceptionHappened(DateTime.Now))
            {
                mLog.Warn(
                    "PlasticAssetsProcessor skipping DeleteFromSourceControl operation " +
                    "because an exception happened in the last 60 seconds");
                return;
            }

            foreach (string path in paths)
                mLog.DebugFormat("DeleteFromSourceControl: {0}", path);

            mWorkspaceOperationsMonitor.AddAssetsProcessorPathsToDelete(paths);
        }

        internal void MoveOnSourceControl(List<AssetPostprocessor.PathToMove> paths)
        {
            if (paths.Count == 0)
                return;

            if (IsDisableBecauseExceptionHappened(DateTime.Now))
            {
                mLog.Warn(
                    "PlasticAssetsProcessor skipping MoveOnSourceControl operation " +
                    "because an exception happened in the last 60 seconds");
                return;
            }

            foreach (AssetPostprocessor.PathToMove path in paths)
                mLog.DebugFormat("MoveOnSourceControl: {0} to {1}", path.SrcPath, path.DstPath);

            mWorkspaceOperationsMonitor.AddAssetsProcessorPathsToMove(paths);
        }

        internal void CheckoutOnSourceControl(List<string> paths)
        {
            if (paths.Count == 0)
                return;

            if (IsDisableBecauseExceptionHappened(DateTime.Now))
            {
                mLog.Warn(
                    "PlasticAssetsProcessor skipping CheckoutOnSourceControl operation " + 
                    "because an exception happened in the last 60 seconds");
                return;
            }

            foreach (string path in paths)
                mLog.DebugFormat("CheckoutOnSourceControl: {0}", path);

            mWorkspaceOperationsMonitor.AddAssetsProcessorPathsToCheckout(paths);
        }

        void WorkspaceOperationsMonitor.IDisableAssetsProcessor.Disable()
        {
            mLastExceptionDateTime = DateTime.Now;
        }

        bool IsDisableBecauseExceptionHappened(DateTime now)
        {
            return (now - mLastExceptionDateTime).TotalSeconds < 5;
        }

        DateTime mLastExceptionDateTime = DateTime.MinValue;
        WorkspaceOperationsMonitor mWorkspaceOperationsMonitor;

        static readonly ILog mLog = LogManager.GetLogger("PlasticAssetsProcessor");
    }
}