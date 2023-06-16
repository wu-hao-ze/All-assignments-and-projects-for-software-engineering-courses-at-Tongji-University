using System.Collections.Generic;
using System.IO;
using System.Linq;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal static class PendingChangesSelection
    {
        internal static void SelectChanges(
            PendingChangesTreeView treeView,
            List<ChangeInfo> changesToSelect)
        {
            if (changesToSelect == null || changesToSelect.Count == 0)
            {
                treeView.SelectFirstPendingChangeOnTree();
                return;
            }

            treeView.SelectPreviouslySelectedPendingChanges(changesToSelect);

            if (treeView.HasSelection())
                return;

            treeView.SelectFirstPendingChangeOnTree();
        }

        internal static List<string> GetSelectedPathsWithoutMeta(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChanges(false)
                .Select(change => change.GetFullPath()).ToList();
        }

        internal static List<string> GetSelectedPaths(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChanges(true)
                .Select(change => change.GetFullPath()).ToList();
        }

        internal static List<string> GetSelectedMetaPaths(
            PendingChangesTreeView treeView)
        {
            List<string> result = new List<string>();

            foreach (ChangeInfo change in GetSelectedChanges(treeView))
            {
                string path = change.GetFullPath();

                if (!MetaPath.IsMetaPath(path))
                    continue;

                result.Add(path);
            }

            return result;
        }

        internal static List<ChangeInfo> GetAllChanges(
            PendingChangesTreeView treeView)
        {
            return treeView.GetAllChanges();
        }

        internal static List<ChangeInfo> GetChangesToFocus(
            PendingChangesTreeView treeView)
        {
            List<ChangeInfo> selectedChanges = treeView.GetSelectedChanges(true);

            if (selectedChanges.Count == 0)
                return selectedChanges;

            List<ChangeInfo> changesToFocus =
                selectedChanges.Where(change => !IsAddedFile(change)).ToList();

            if (changesToFocus.Count() == 0)
            {
                ChangeInfo nearestAddedChange = treeView.GetNearestAddedChange();
                if (nearestAddedChange != null)
                    changesToFocus.Add(nearestAddedChange);
            }

            return changesToFocus;
        }

        internal static SelectedChangesGroupInfo GetSelectedChangesGroupInfo(
            string wkPath, PendingChangesTreeView treeView)
        {
            return SelectedChangesGroupInfo.BuildFromChangeInfos(
                wkPath,
                treeView.GetSelectedChanges(true),
                GetInvolvedChangelists(treeView.GetSelectedPendingChangeInfos()));
        }

        internal static List<ChangeInfo> GetSelectedChanges(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChanges(true);
        }

        internal static List<ChangeListInfo> GetSelectedChangeListInfos(
            PendingChangesTreeView treeView)
        {
            List<ChangeListInfo> result = new List<ChangeListInfo>();
            List<ChangelistNode> nodes = treeView.GetSelectedChangelistNodes();

            foreach (ChangelistNode node in nodes)
                result.Add(node.ChangelistInfo);

            return result;
        }

        internal static ChangeListInfo GetSelectedChangeListInfo(
            PendingChangesTreeView treeView)
        {
            List<ChangeListInfo> changeListInfos = GetSelectedChangeListInfos(treeView);

            if (changeListInfos.Count == 0)
                return null;

            return changeListInfos[0];
        }

        internal static List<ChangelistNode> GetSelectedChangelistNodes(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChangelistNodes();
        }

        internal static ChangeInfo GetSelectedChange(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedRow();
        }

        static List<ChangeListInfo> GetInvolvedChangelists(List<PendingChangeInfo> changes)
        {
            List<ChangeListInfo> result = new List<ChangeListInfo>();

            foreach (PendingChangeInfo pendingChangeInfo in changes)
            {
                ChangelistNode changelistNode =
                    (ChangelistNode)pendingChangeInfo.GetParent().GetParent();

                if (changelistNode == null)
                    continue;

                result.Add(changelistNode.ChangelistInfo);
            }

            return result;
        }

        static bool IsAddedFile(ChangeInfo change)
        {
            return ChangeTypesClassifier.IsInAddedCategory(change.ChangeTypes)
                && !(Directory.Exists(change.Path) || File.Exists(change.Path));
        }
    }
}
