using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.Merge;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal class ChangeTreeViewItem : TreeViewItem
    {
        internal MergeChangeInfo ChangeInfo { get; private set; }

        internal ChangeTreeViewItem(int id, MergeChangeInfo change)
            : base(id, 1)
        {
            ChangeInfo = change;

            displayName = id.ToString();
        }
    }
}
