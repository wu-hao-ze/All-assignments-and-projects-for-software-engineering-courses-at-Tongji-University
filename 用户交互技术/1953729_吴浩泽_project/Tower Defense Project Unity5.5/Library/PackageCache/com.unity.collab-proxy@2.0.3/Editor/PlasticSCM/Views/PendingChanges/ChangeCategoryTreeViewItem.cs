using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.PendingChanges;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class ChangeCategoryTreeViewItem : TreeViewItem
    {
        internal PendingChangeCategory Category { get; private set; }

        internal ChangeCategoryTreeViewItem(int id, PendingChangeCategory category, int depth)
            : base(id, depth)
        {
            Category = category;
        }
    }
}
