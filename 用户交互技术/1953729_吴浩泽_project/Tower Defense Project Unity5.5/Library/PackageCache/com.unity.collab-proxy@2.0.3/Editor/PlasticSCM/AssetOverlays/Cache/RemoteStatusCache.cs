using System;
using System.Collections.Generic;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.Client.GameUI;
using Codice.Client.GameUI.Update;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using Codice.Utils;
using GluonGui.WorkspaceWindow.Views;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal class RemoteStatusCache
    {
        internal RemoteStatusCache(
            WorkspaceInfo wkInfo,
            bool isGluonMode,
            Action repaintProjectWindow)
        {
            mWkInfo = wkInfo;
            mIsGluonMode = isGluonMode;
            mRepaintProjectWindow = repaintProjectWindow;
        }

        internal AssetStatus GetStatus(string fullPath)
        {
            if (!mIsGluonMode)
                return AssetStatus.UpToDate;

            lock(mLock)
            {
                if (mStatusByPathCache == null)
                {
                    mStatusByPathCache = BuildPathDictionary.ForPlatform<AssetStatus>();

                    mCurrentCancelToken.Cancel();
                    mCurrentCancelToken = new CancelToken();
                    AsyncCalculateStatus(mCurrentCancelToken);

                    return AssetStatus.UpToDate;
                }

                AssetStatus result;
                if (mStatusByPathCache.TryGetValue(fullPath, out result))
                    return result;

                return AssetStatus.UpToDate;
            }
        }

        internal void Clear()
        {
            lock (mLock)
            {
                mCurrentCancelToken.Cancel();
                mStatusByPathCache = null;
            }
        }

        void AsyncCalculateStatus(CancelToken cancelToken)
        {
            Dictionary<string, AssetStatus> statusByPathCache = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(50);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    OutOfDateItems outOfDateItems =
                        OutOfDateUpdater.CalculateOutOfDateItems(
                            mWkInfo, new List<ErrorMessage>(),
                            OutOfDateCalculator.Options.IsIncomingChanges);

                    if (cancelToken.IsCancelled())
                        return;

                    statusByPathCache = BuildStatusByPathCache.
                        ForOutOfDateItems(outOfDateItems, mWkInfo.ClientPath);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.LogException(
                            "RemoteStatusCache",
                            waiter.Exception);
                        return;
                    }

                    if (cancelToken.IsCancelled())
                        return;

                    lock (mLock)
                    {
                        mStatusByPathCache = statusByPathCache;
                    }

                    mRepaintProjectWindow();
                });
        }

        static class BuildStatusByPathCache
        {
            internal static Dictionary<string, AssetStatus> ForOutOfDateItems(
                OutOfDateItems outOfDateItems,
                string wkPath)
            {
                Dictionary<string, AssetStatus> result =
                    BuildPathDictionary.ForPlatform<AssetStatus>();

                if (outOfDateItems == null)
                    return result;

                foreach (OutOfDateItemsByMount diffs in
                    outOfDateItems.GetOutOfDateItemsByMountList())
                {
                    foreach (Difference diff in diffs.Changed)
                    {
                        if (diff is DiffXlinkChanged)
                            continue;

                        string path = GetPathForDiff(wkPath, diffs.Mount, diff.Path);
                        result.Add(path, AssetStatus.OutOfDate);
                    }

                    foreach (Difference diff in diffs.Deleted)
                    {
                        string path = GetPathForDiff(wkPath, diffs.Mount, diff.Path);
                        result.Add(path, AssetStatus.DeletedOnServer);
                    }
                }

                foreach (GluonFileConflict fileConflict in
                    outOfDateItems.GetFileConflicts())
                {
                    string path = GetPathForConflict(wkPath, fileConflict.CmPath);
                    result.Add(path, AssetStatus.Conflicted);
                }

                return result;
            }

            static string GetPathForDiff(
                string wkPath,
                MountPointWithPath mountPoint,
                string cmSubPath)
            {
                return WorkspacePath.GetWorkspacePathFromCmPath(
                    wkPath,
                    WorkspacePath.ComposeMountPath(mountPoint.MountPath, cmSubPath),
                    PathHelper.GetDirectorySeparatorChar(wkPath));
            }

            static string GetPathForConflict(
                string wkPath,
                string cmPath)
            {
                return WorkspacePath.GetWorkspacePathFromCmPath(
                    wkPath, cmPath,
                    PathHelper.GetDirectorySeparatorChar(wkPath));
            }
        }

        CancelToken mCurrentCancelToken = new CancelToken();

        Dictionary<string, AssetStatus> mStatusByPathCache;

        readonly Action mRepaintProjectWindow;
        readonly bool mIsGluonMode;
        readonly WorkspaceInfo mWkInfo;

        static object mLock = new object();
    }
}
