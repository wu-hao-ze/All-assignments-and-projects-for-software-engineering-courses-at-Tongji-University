using System.Collections.Generic;

using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon
{
    internal class UnityIncomingChangesTree
    {
        internal static UnityIncomingChangesTree BuildIncomingChangeCategories(
            IncomingChangesTree tree)
        {
            return new UnityIncomingChangesTree(tree);
        }

        UnityIncomingChangesTree(IncomingChangesTree tree)
        {
            mInnerTree = tree; 

            mMetaCache.Build(mInnerTree.GetNodes());
        }

        internal List<IncomingChangeCategory> GetNodes()
        {
            return mInnerTree.GetNodes();
        }

        internal bool HasMeta(IncomingChangeInfo changeInfo)
        {
            return mMetaCache.ContainsMeta(changeInfo);
        }

        internal IncomingChangeInfo GetMetaChange(IncomingChangeInfo change)
        {
            return mMetaCache.GetExistingMeta(change);
        }

        internal void FillWithMeta(List<IncomingChangeInfo> changes)
        {
            changes.AddRange(
                mMetaCache.GetExistingMeta(changes));
        }

        internal void Sort(string key, bool isAscending)
        {
            mInnerTree.Sort(key, isAscending);
        }

        MetaCache mMetaCache = new MetaCache();
        IncomingChangesTree mInnerTree;

        class MetaCache
        {
            internal bool ContainsMeta(IncomingChangeInfo changeInfo)
            {
                string key = BuildKey.ForMetaChange(changeInfo);

                return mCache.ContainsKey(key);
            }

            internal IncomingChangeInfo GetExistingMeta(IncomingChangeInfo change)
            {
                IncomingChangeInfo result;

                if (!mCache.TryGetValue(BuildKey.ForMetaChange(change), out result))
                    return null;

                return result;
            }

            internal List<IncomingChangeInfo> GetExistingMeta(
                List<IncomingChangeInfo> changes)
            {
                List<IncomingChangeInfo> result = new List<IncomingChangeInfo>();

                foreach (IncomingChangeInfo change in changes)
                {
                    string key = BuildKey.ForMetaChange(change);

                    IncomingChangeInfo metaChange;
                    if (!mCache.TryGetValue(key, out metaChange))
                        continue;

                    result.Add(metaChange);
                }

                return result;
            }

            internal void Build(List<IncomingChangeCategory> incomingChangesCategories)
            {
                mCache.Clear();

                foreach (IncomingChangeCategory category in incomingChangesCategories)
                {
                    ExtractMetaToCache(category, mCache);
                }
            }

            static void ExtractMetaToCache(
                IncomingChangeCategory category,
                Dictionary<string, IncomingChangeInfo> cache)
            {
                List<IncomingChangeInfo> changes = category.GetChanges();

                HashSet<string> indexedKeys = BuildIndexedKeys(
                    changes);

                for (int i = changes.Count - 1; i >= 0; i--)
                {
                    IncomingChangeInfo currentChange = changes[i];

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
                List<IncomingChangeInfo> changes)
            {
                HashSet<string> result = new HashSet<string>();

                foreach (IncomingChangeInfo change in changes)
                {
                    if (MetaPath.IsMetaPath(change.GetPath()))
                        continue;

                    result.Add(BuildKey.ForChange(change));
                }

                return result;
            }

            Dictionary<string, IncomingChangeInfo> mCache =
                new Dictionary<string, IncomingChangeInfo>();

            static class BuildKey
            {
                internal static string ForChange(
                    IncomingChangeInfo change)
                {
                    return BuildCacheKey(
                        change.CategoryType,
                        change.GetPath());
                }

                internal static string ForMetaChange(
                    IncomingChangeInfo change)
                {
                    return BuildCacheKey(
                        change.CategoryType,
                        MetaPath.GetMetaPath(change.GetPath()));
                }

                internal static string BuildCacheKey(
                    IncomingChangeCategory.Type type,
                    string path)
                {
                    return string.Concat(type, ":", path);
                }
            }
        }
    }
}
