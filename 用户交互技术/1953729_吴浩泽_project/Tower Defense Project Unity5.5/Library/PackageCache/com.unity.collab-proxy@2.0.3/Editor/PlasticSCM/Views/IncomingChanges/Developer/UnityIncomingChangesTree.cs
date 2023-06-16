using System.Collections.Generic;

using PlasticGui.WorkspaceWindow.Merge;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal class UnityIncomingChangesTree
    {
        internal static UnityIncomingChangesTree BuildIncomingChangeCategories(
            MergeChangesTree tree)
        {
            return new UnityIncomingChangesTree(tree);
        }

        UnityIncomingChangesTree(
            MergeChangesTree tree)
        {
            mInnerTree = tree;

            mMetaCache.Build(mInnerTree.GetNodes());
        }

        internal List<MergeChangesCategory> GetNodes()
        {
            return mInnerTree.GetNodes();
        }

        internal bool HasMeta(MergeChangeInfo changeInfo)
        {
            return mMetaCache.ContainsMeta(changeInfo);
        }

        internal MergeChangeInfo GetMetaChange(MergeChangeInfo change)
        {
            return mMetaCache.GetExistingMeta(change);
        }

        internal void FillWithMeta(List<MergeChangeInfo> changes)
        {
            changes.AddRange(
                mMetaCache.GetExistingMeta(changes));
        }

        internal void Sort(string key, bool isAscending)
        {
            mInnerTree.Sort(key, isAscending);
        }

        internal void ResolveUserNames(
            MergeChangesTree.ResolveUserName resolveUserName)
        {
            mInnerTree.ResolveUserNames(resolveUserName);
        }

        MetaCache mMetaCache = new MetaCache();
        MergeChangesTree mInnerTree;

        class MetaCache
        {
            internal bool ContainsMeta(MergeChangeInfo changeInfo)
            {
                string key = BuildKey.ForMetaChange(changeInfo);

                return mCache.ContainsKey(key);
            }

            internal MergeChangeInfo GetExistingMeta(MergeChangeInfo change)
            {
                MergeChangeInfo result;

                if (!mCache.TryGetValue(BuildKey.ForMetaChange(change), out result))
                    return null;

                return result;
            }

            internal List<MergeChangeInfo> GetExistingMeta(
                List<MergeChangeInfo> changes)
            {
                List<MergeChangeInfo> result = new List<MergeChangeInfo>();

                foreach (MergeChangeInfo change in changes)
                {
                    string key = BuildKey.ForMetaChange(change);

                    MergeChangeInfo metaChange;
                    if (!mCache.TryGetValue(key, out metaChange))
                        continue;

                    result.Add(metaChange);
                }

                return result;
            }

            internal void Build(List<MergeChangesCategory> incomingChangesCategories)
            {
                mCache.Clear();

                foreach (MergeChangesCategory category in incomingChangesCategories)
                {
                    ExtractMetaToCache(category, mCache);
                }
            }

            static void ExtractMetaToCache(
                MergeChangesCategory category,
                Dictionary<string, MergeChangeInfo> cache)
            {
                List<MergeChangeInfo> changes = category.GetChanges();

                HashSet<string> indexedKeys = BuildIndexedKeys(
                    changes);

                for (int i = changes.Count - 1; i >= 0; i--)
                {
                    MergeChangeInfo currentChange = changes[i];

                    string path = currentChange.GetPath();

                    if (!MetaPath.IsMetaPath(path))
                        continue;

                    string realPath = MetaPath.GetPathFromMetaPath(path);

                    if (!indexedKeys.Contains(BuildKey.BuildCacheKey(
                        currentChange.CategoryType, realPath)))
                        continue;

                    // found foo.c and foo.c.meta - move .meta to cache
                    cache.Add(BuildKey.ForChange(currentChange), currentChange);
                    changes.RemoveAt(i);
                }
            }

            static HashSet<string> BuildIndexedKeys(
                List<MergeChangeInfo> changes)
            {
                HashSet<string> result = new HashSet<string>();

                foreach (MergeChangeInfo change in changes)
                {
                    if (MetaPath.IsMetaPath(change.GetPath()))
                        continue;

                    result.Add(BuildKey.ForChange(change));
                }

                return result;
            }

            Dictionary<string, MergeChangeInfo> mCache =
                new Dictionary<string, MergeChangeInfo>();

            static class BuildKey
            {
                internal static string ForChange(
                    MergeChangeInfo change)
                {
                    return BuildCacheKey(
                        change.CategoryType,
                        change.GetPath());
                }

                internal static string ForMetaChange(
                    MergeChangeInfo change)
                {
                    return BuildCacheKey(
                        change.CategoryType,
                        MetaPath.GetMetaPath(change.GetPath()));
                }

                internal static string BuildCacheKey(
                    MergeChangesCategory.Type type,
                    string path)
                {
                    return string.Concat(type, ":", path);
                }
            }
        }
    }
}
