using System;
using System.Collections.Generic;

using Codice.Client.Commands.WkTree;
using Codice.Client.Common;
using Codice.Client.Common.Locks;
using Codice.Client.Common.WkTree;
using Codice.CM.Common;
using Codice.CM.WorkspaceServer.DataStore.Guids;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal static class SearchLocks
    {
        internal static Dictionary<WorkspaceTreeNode, LockInfo> GetLocksInfo(
            WorkspaceInfo wkInfo,
            Dictionary<RepositorySpec, List<WorkspaceTreeNode>> locksCandidates)
        {
            Dictionary<WorkspaceTreeNode, LockInfo> result =
                new Dictionary<WorkspaceTreeNode, LockInfo>();

            Dictionary<string, Dictionary<Guid, LockInfo>> locksByItemByServer =
                new Dictionary<string, Dictionary<Guid, LockInfo>>(
                    StringComparer.InvariantCultureIgnoreCase);

            foreach (KeyValuePair<RepositorySpec, List<WorkspaceTreeNode>> each in locksCandidates)
            {
                FillRepositoryLocks(
                    wkInfo, each.Key, each.Value,
                    locksByItemByServer, result);
            }

            return result;
        }

        static void FillRepositoryLocks(
            WorkspaceInfo wkInfo,
            RepositorySpec repSpec,
            List<WorkspaceTreeNode> candidates,
            Dictionary<string, Dictionary<Guid, LockInfo>> locksByItemByServer,
            Dictionary<WorkspaceTreeNode, LockInfo> locks)
        {
            if (candidates.Count == 0)
                return;

            LockRule lockRule = ServerLocks.GetLockRule(repSpec);

            if (lockRule == null)
                return;

            candidates = GetLockableCandidates(candidates, lockRule);

            if (candidates.Count == 0)
                return;

            string lockServer = string.IsNullOrEmpty(lockRule.LockServer) ?
                repSpec.Server : lockRule.LockServer;

            Dictionary<Guid, LockInfo> serverlocksByItem =
                ServerLocks.GetServerLocksByItem(
                    lockServer, locksByItemByServer);

            if (serverlocksByItem == null || serverlocksByItem.Count == 0)
                return;

            IList<Guid> candidatesGuids = GetCandidatesGuids(
                wkInfo, repSpec, candidates);

            for (int index = 0; index < candidates.Count; index++)
            {
                LockInfo serverLock;
                if (!serverlocksByItem.TryGetValue(
                        candidatesGuids[index], out serverLock))
                    continue;

                locks[candidates[index]] = serverLock;
            }
        }

        static List<WorkspaceTreeNode> GetLockableCandidates(
            List<WorkspaceTreeNode> candidates,
            LockRule lockRule)
        {
            List<WorkspaceTreeNode> result = new List<WorkspaceTreeNode>();

            LockedFilesFilter filter = new LockedFilesFilter(lockRule.Rules);

            foreach (WorkspaceTreeNode candidate in candidates)
            {
                string cmPath = WorkspaceNodeOperations.GetCmPath(candidate);

                if (cmPath == null)
                {
                    //The node could not be on the head tree (like copied items) so when we
                    //cannot calculate the path we assume that it's lockable.
                    result.Add(candidate);
                    continue;
                }

                if (filter.IsLockable(cmPath))
                    result.Add(candidate);
            }

            return result;
        }

        static IList<Guid> GetCandidatesGuids(
            WorkspaceInfo wkInfo,
            RepositorySpec repSpec,
            List<WorkspaceTreeNode> candidates)
        {
            RepositoryInfo repInfo = RepositorySpecResolverProvider.
                Get().GetRepInfo(repSpec);

            IList<long> ids = new List<long>(candidates.Count);

            foreach (WorkspaceTreeNode candidate in candidates)
                ids.Add(candidate.RevInfo.ItemId);

            return GuidResolver.Get().GetObjectGuids(repInfo, wkInfo, ids);
        }
    }
}
