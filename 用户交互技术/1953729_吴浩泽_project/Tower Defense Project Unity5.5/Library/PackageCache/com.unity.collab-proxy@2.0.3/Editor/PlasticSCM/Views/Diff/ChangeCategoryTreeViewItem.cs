using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal class ChangeCategoryTreeViewItem : TreeViewItem
    {
        internal ChangeCategory Category { get; private set; }

        internal ChangeCategoryTreeViewItem(
            int id, int depth, ChangeCategory category)
            : base(id, depth, category.GetHeaderText())
        {
            Category = category;
        }
    }
}
