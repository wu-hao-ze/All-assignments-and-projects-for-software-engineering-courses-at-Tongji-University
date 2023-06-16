using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal class MergeCategoryTreeViewItem : TreeViewItem
    {
        internal CategoryGroup Category { get; private set; }

        internal MergeCategoryTreeViewItem(
            int id, int depth, CategoryGroup categoryGroup)
            : base(id, depth, categoryGroup.GetHeaderText())
        {
            Category = categoryGroup;
        }
    }
}
