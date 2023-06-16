using System.Collections.Generic;

using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.CM.Common;
using Codice.Utils;
using PlasticGui;
using PlasticGui.Diff;
using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal class UnityDiffTree
    {
        internal UnityDiffTree()
        {
            mInnerTree = new DiffTree();
            mMetaCache = new MetaCache();
        }

        internal void BuildCategories(
            WorkspaceInfo wkInfo,
            List<ClientDiff> diffs,
            BranchResolver brResolver,
            bool skipMergeTracking)
        {
            mInnerTree.BuildCategories(
                RevisionInfoCodeReviewAdapter.CalculateCodeReviewEntries(
                        wkInfo,
                        diffs,
                        brResolver,
                        skipMergeTracking),
                brResolver);
            mMetaCache.Build(mInnerTree.GetNodes());
        }

        internal List<IDiffCategory> GetNodes()
        {
            return mInnerTree.GetNodes();
        }

        internal bool HasMeta(ClientDiffInfo difference)
        {
            return mMetaCache.ContainsMeta(difference);
        }

        internal ClientDiffInfo GetMetaDiff(ClientDiffInfo diff)
        {
            return mMetaCache.GetExistingMeta(diff);
        }

        internal void FillWithMeta(List<ClientDiffInfo> diffs)
        {
            diffs.AddRange(
                mMetaCache.GetExistingMeta(diffs));
        }

        internal void Sort(string key, bool sortAscending)
        {
            mInnerTree.Sort(key, sortAscending);
        }

        internal void Filter(Filter filter, List<string> columnNames)
        {
            mInnerTree.Filter(filter, columnNames);
        }

        MetaCache mMetaCache = new MetaCache();
        DiffTree mInnerTree;

        class MetaCache
        {
            internal void Build(List<IDiffCategory> categories)
            {
                mCache.Clear();

                HashSet<string> indexedKeys = BuildIndexedKeys(
                    GetClientDiffInfos.FromCategories(categories));

                for (int i = 0; i < categories.Count; i++)
                {
                    ExtractToMetaCache(
                        (ITreeViewNode)categories[i],
                        i,
                        mCache,
                        indexedKeys);
                }
            }

            internal bool ContainsMeta(ClientDiffInfo diff)
            {
                return mCache.ContainsKey(
                    BuildKey.ForMetaDiff(diff));
            }

            internal ClientDiffInfo GetExistingMeta(ClientDiffInfo diff)
            {
                ClientDiffInfo result;

                if (!mCache.TryGetValue(BuildKey.ForMetaDiff(diff), out result))
                    return null;

                return result;
            }

            internal List<ClientDiffInfo> GetExistingMeta(List<ClientDiffInfo> diffs)
            {
                List<ClientDiffInfo> result = new List<ClientDiffInfo>();

                foreach (ClientDiffInfo diff in diffs)
                {
                    string key = BuildKey.ForMetaDiff(diff);

                    ClientDiffInfo metaDiff;
                    if (!mCache.TryGetValue(key, out metaDiff))
                        continue;

                    result.Add(metaDiff);
                }

                return result;
            }

            static void ExtractToMetaCache(
                ITreeViewNode node,
                int nodeIndex,
                Dictionary<string, ClientDiffInfo> cache,
                HashSet<string> indexedKeys)
            {
                if (node is ClientDiffInfo)
                {
                    ClientDiffInfo diff = (ClientDiffInfo)node;

                    string path = diff.DiffWithMount.Difference.Path;

                    if (!MetaPath.IsMetaPath(path))
                        return;

                    string realPath = MetaPath.GetPathFromMetaPath(path);

                    if (!indexedKeys.Contains(BuildKey.BuildCacheKey(
                        BuildKey.GetCategoryGroup(diff),
                        BuildKey.GetChangeCategory(diff),
                        realPath)))
                        return;

                    // found foo.c and foo.c.meta
                    // with the same chage types - move .meta to cache
                    cache.Add(BuildKey.ForDiff(diff), diff);
                    ((ChangeCategory)node.GetParent()).RemoveDiffAt(nodeIndex);
                }

                for (int i = node.GetChildrenCount() - 1; i >= 0; i--)
                {
                    ExtractToMetaCache(
                        node.GetChild(i),
                        i,
                        cache,
                        indexedKeys);
                }
            }

            HashSet<string> BuildIndexedKeys(List<ClientDiffInfo> diffs)
            {
                HashSet<string> result = new HashSet<string>();

                foreach (ClientDiffInfo diff in diffs)
                {
                    if (MetaPath.IsMetaPath(diff.DiffWithMount.Difference.Path))
                        continue;

                    result.Add(BuildKey.ForDiff(diff));
                }

                return result;
            }

            Dictionary<string, ClientDiffInfo> mCache =
                new Dictionary<string, ClientDiffInfo>();

            static class BuildKey
            {
                internal static string ForDiff(
                    ClientDiffInfo diff)
                {
                    return BuildCacheKey(
                        GetCategoryGroup(diff),
                        GetChangeCategory(diff),
                        diff.DiffWithMount.Difference.Path);
                }

                internal static string ForMetaDiff(
                    ClientDiffInfo diff)
                {
                    return BuildCacheKey(
                        GetCategoryGroup(diff),
                        GetChangeCategory(diff),
                        MetaPath.GetMetaPath(diff.DiffWithMount.Difference.Path));
                }

                internal static string BuildCacheKey(
                    CategoryGroup categoryGroup,
                    ChangeCategory changeCategory,
                    string path)
                {
                    string result = string.Concat(changeCategory.Type, ":", path);

                    if (categoryGroup == null)
                        return result;

                    return string.Concat(categoryGroup.GetHeaderText(), ":", result);
                }

                internal static ChangeCategory GetChangeCategory(ClientDiffInfo diff)
                {
                    return (ChangeCategory)diff.GetParent();
                }

                internal static CategoryGroup GetCategoryGroup(ClientDiffInfo diff)
                {
                    ChangeCategory changeCategory = GetChangeCategory(diff);

                    ITreeViewNode categoryGroup = changeCategory.GetParent();

                    if (categoryGroup == null)
                        return null;

                    return (CategoryGroup)categoryGroup;
                }
            }
        }
    }
}
