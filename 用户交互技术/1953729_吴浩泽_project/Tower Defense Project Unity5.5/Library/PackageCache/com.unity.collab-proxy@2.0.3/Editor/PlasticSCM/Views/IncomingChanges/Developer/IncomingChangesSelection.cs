using System.Collections.Generic;

using PlasticGui.WorkspaceWindow.IncomingChanges;
using PlasticGui.WorkspaceWindow.Merge;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal static class IncomingChangesSelection
    {
        internal static List<string> GetPathsFromSelectedFileConflictsIncludingMeta(
            IncomingChangesTreeView treeView)
        {
            List<string> result = new List<string>();

            List<MergeChangeInfo> selection =
                treeView.GetSelectedFileConflicts();

            treeView.FillWithMeta(selection);

            foreach (MergeChangeInfo incomingChange in selection)
            {
                result.Add(incomingChange.GetPath());
            }

            return result;
        }

        internal static SelectedIncomingChangesGroupInfo GetSelectedGroupInfo(
            IncomingChangesTreeView treeView)
        {
            List<MergeChangeInfo> selectedIncomingChanges =
                treeView.GetSelectedIncomingChanges();

            return GetSelectedIncomingChangesGroupInfo.For(
                selectedIncomingChanges);
        }

        internal static MergeChangeInfo GetSingleSelectedIncomingChange(
            IncomingChangesTreeView treeView)
        {
            return treeView.GetSelectedIncomingChange();
        }
    }
}
