using System;
using System.Collections.Generic;

using Codice;
using Codice.Client.BaseCommands;
using Codice.Client.Commands.WkTree;
using Codice.Client.Common;
using Codice.Client.Common.Locks;
using Codice.Client.Common.Threading;
using Codice.Client.Common.WkTree;
using Codice.CM.Common;
using Codice.Utils;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal class LockStatusCache
    {
        internal LockStatusCache(
            WorkspaceInfo wkInfo,
            Action repaintProjectWindow)
        {
            mWkInfo = wkInfo;
            mRepaintProjectWindow = repaintProjectWindow;
        }

        internal AssetStatus GetStatus(string fullPath)
        {
            LockStatusData lockStatusData = GetLockStatusData(fullPath);

            if (lockStatusData == null)
                return AssetStatus.None;

            return lockStatusData.Status;
        }

        internal LockStatusData GetLockStatusData(string fullPath)
        {
            lock (mLock)
            {
                if (mStatusByPathCache == null)
                {
                    mStatusByPathCache = BuildPathDictionary.ForPlatform<LockStatusData>();

                    mCurrentCancelToken.Cancel();
                    mCurrentCancelToken = new CancelToken();
                    AsyncCalculateStatus(mCurrentCancelToken);

                    return null;
                }

                LockStatusData result;

                if (mStatusByPathCache.TryGetValue(fullPath, out result))
                    return result;

                return null;
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
            Dictionary<string, LockStatusData> statusByPathCache = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(50);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {

                    Dictionary<RepositorySpec, List<WorkspaceTreeNode>> lockCandidates =
                        new Dictionary<RepositorySpec, List<WorkspaceTreeNode>>();

                    FillLockCandidates.ForTree(mWkInfo, lockCandidates);

                    if (cancelToken.IsCancelled())
                        return;

                    Dictionary<WorkspaceTreeNode, LockInfo> lockInfoByNode =
                        SearchLocks.GetLocksInfo(mWkInfo, lockCandidates);

                    if (cancelToken.IsCancelled())
                        return;

                    statusByPathCache = BuildStatusByNodeCache.
                        ForLocks(mWkInfo.ClientPath, lockInfoByNode);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.LogException(
                            "LockStatusCache",
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

        static class FillLockCandidates
        {
            internal static void ForTree(
                WorkspaceInfo wkInfo,
                Dictionary<RepositorySpec, List<WorkspaceTreeNode>> lockCandidates)
            {
                WorkspaceTreeNode rootNode = CmConnection.Get().GetWorkspaceTreeHandler().
                GetWorkspaceTree(wkInfo, wkInfo.ClientPath, true);

                Queue<WorkspaceTreeNode> pendingDirectories = new Queue<WorkspaceTreeNode>();
                pendingDirectories.Enqueue(rootNode);

                while (pendingDirectories.Count > 0)
                {
                    WorkspaceTreeNode directoryNode = pendingDirectories.Dequeue();

                    ForChildren(directoryNode, pendingDirectories, lockCandidates);
                }
            }

            static void ForChildren(
                WorkspaceTreeNode directoryNode,
                Queue<WorkspaceTreeNode> pendingDirectories,
                Dictionary<RepositorySpec, List<WorkspaceTreeNode>> lockCandidates)
            {
                if (!directoryNode.HasChildren)
                    return;

                foreach (WorkspaceTreeNode child in directoryNode.Children)
                {
                    if (CheckWorkspaceTreeNodeStatus.IsDirectory(child))
                    {
                        pendingDirectories.Enqueue(child);
                        continue;
                    }

                    if (CheckWorkspaceTreeNodeStatus.IsAdded(child))
                        continue;

                    List<WorkspaceTreeNode> nodes = null;
                    if (!lockCandidates.TryGetValue(child.RepSpec, out nodes))
                    {
                        nodes = new List<WorkspaceTreeNode>();
                        lockCandidates.Add(child.RepSpec, nodes);
                    }

                    nodes.Add(child);
                }
            }
        }

        static class BuildStatusByNodeCache
        {
            internal static Dictionary<string, LockStatusData> ForLocks(
                string wkPath,
                Dictionary<WorkspaceTreeNode, LockInfo> lockInfoByNode)
            {
                Dictionary<string, LockStatusData> result =
                    BuildPathDictionary.ForPlatform<LockStatusData>();

                LockOwnerNameResolver nameResolver = new LockOwnerNameResolver();

                foreach (WorkspaceTreeNode node in lockInfoByNode.Keys)
                {
                    LockStatusData lockStatusData = BuildLockStatusData(
                       node, lockInfoByNode[node], nameResolver);

                    string nodeWkPath = WorkspacePath.GetWorkspacePathFromCmPath(
                        wkPath,
                        WorkspaceNodeOperations.GetCmPath(node),
                        PathHelper.GetDirectorySeparatorChar(wkPath));

                    result.Add(nodeWkPath, lockStatusData);
                }

                return result;
            }

            static LockStatusData BuildLockStatusData(
                WorkspaceTreeNode node,
                LockInfo lockInfo,
                LockOwnerNameResolver nameResolver)
            {
                AssetStatus status = CheckWorkspaceTreeNodeStatus.IsCheckedOut(node) ?
                    AssetStatus.Locked : AssetStatus.LockedRemote;

                return new LockStatusData(
                    status,
                    nameResolver.GetSeidName(lockInfo.SEIDData),
                    LockWkInfo.GetWkCleanName(lockInfo));
            }
        }

        CancelToken mCurrentCancelToken = new CancelToken();

        Dictionary<string, LockStatusData> mStatusByPathCache;

        readonly WorkspaceInfo mWkInfo;
        readonly Action mRepaintProjectWindow;

        static object mLock = new object();
    }
}
