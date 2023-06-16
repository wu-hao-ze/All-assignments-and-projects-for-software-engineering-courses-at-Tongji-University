using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class ChangelistTreeViewItem : TreeViewItem
    {
        internal ChangelistNode Changelist { get; private set; }

        internal ChangelistTreeViewItem(int id, ChangelistNode changelist)
            : base(id, 0)
        {
            Changelist = changelist;
        }
    }
}
