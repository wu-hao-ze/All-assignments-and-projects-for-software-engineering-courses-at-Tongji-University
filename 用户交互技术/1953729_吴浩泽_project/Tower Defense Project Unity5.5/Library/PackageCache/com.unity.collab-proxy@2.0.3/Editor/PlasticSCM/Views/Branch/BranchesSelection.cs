using System.Collections.Generic;
using System.Linq;

using Codice.CM.Common;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    internal static class BranchesSelection
    {
        internal static void SelectBranches(
            BranchesListView listView,
            List<RepObjectInfo> branchesToSelect,
            int defaultRow)
        {
            if (branchesToSelect == null || branchesToSelect.Count == 0)
            {
                TableViewOperations.SelectFirstRow(listView);
                return;
            }

            listView.SelectRepObjectInfos(branchesToSelect);

            if (listView.HasSelection())
                return;

            TableViewOperations.SelectDefaultRow(listView, defaultRow);

            if (listView.HasSelection())
                return;

            TableViewOperations.SelectFirstRow(listView);
        }

        internal static List<RepObjectInfo> GetSelectedRepObjectInfos(
            BranchesListView listView)
        {
            return listView.GetSelectedRepObjectInfos();
        }

        internal static int GetSelectedBranchesCount(
            BranchesListView listView)
        {
            return listView.GetSelection().Count;
        }

        internal static BranchInfo GetSelectedBranch(
            BranchesListView listView)
        {
            List<RepObjectInfo> selectedRepObjectsInfos = listView.GetSelectedRepObjectInfos();

            if (selectedRepObjectsInfos.Count == 0)
                return null;

            return (BranchInfo)selectedRepObjectsInfos[0];
        }

        internal static List<BranchInfo> GetSelectedBranches(
            BranchesListView listView)
        {
            return listView.GetSelectedRepObjectInfos().Cast<BranchInfo>().ToList();
        }

        internal static RepositorySpec GetSelectedRepository(
            BranchesListView listView)
        {
            List<RepositorySpec> selectedRepositories = listView.GetSelectedRepositories();

            if (selectedRepositories.Count == 0)
                return null;

            return selectedRepositories[0];
        }

        internal static List<RepositorySpec> GetSelectedRepositories(
            BranchesListView listView)
        {
            return listView.GetSelectedRepositories();
        }
    }
}
