using System.Collections.Generic;

using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon
{
    internal static class IncomingChangesSelection
    {
        internal static SelectedIncomingChangesGroupInfo GetSelectedGroupInfo(
            IncomingChangesTreeView treeView)
        {
            List<IncomingChangeInfo> selectedIncomingChanges =
                treeView.GetSelectedIncomingChanges();

            return GetSelectedIncomingChangesGroupInfo.For(
                selectedIncomingChanges);
        }

        internal static List<IncomingChangeInfo> GetSelectedFileConflictsIncludingMeta(
            IncomingChangesTreeView treeView)
        {
            List<IncomingChangeInfo> result = treeView.GetSelectedFileConflicts();
            treeView.FillWithMeta(result);
            return result;
        }

        internal static IncomingChangeInfo GetSingleSelectedIncomingChange(
            IncomingChangesTreeView treeView)
        {
            return treeView.GetSelectedIncomingChange();
        }
    }
}
