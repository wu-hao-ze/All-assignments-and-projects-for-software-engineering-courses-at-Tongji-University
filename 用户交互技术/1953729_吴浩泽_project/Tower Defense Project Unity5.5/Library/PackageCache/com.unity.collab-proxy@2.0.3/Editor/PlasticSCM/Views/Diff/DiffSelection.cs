using System.Collections.Generic;

using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal static class DiffSelection
    {
        internal static List<ClientDiffInfo> GetSelectedDiffs(
            DiffTreeView treeView)
        {
            return treeView.GetSelectedDiffs(true);
        }

        internal static List<ClientDiffInfo> GetSelectedDiffsWithoutMeta(
            DiffTreeView treeView)
        {
            return treeView.GetSelectedDiffs(false);
        }

        internal static ClientDiffInfo GetSelectedDiff(
            DiffTreeView treeView)
        {
            if (!treeView.HasSelection())
                return null;

            return treeView.GetSelectedDiffs(false)[0];
        }
    }
}
