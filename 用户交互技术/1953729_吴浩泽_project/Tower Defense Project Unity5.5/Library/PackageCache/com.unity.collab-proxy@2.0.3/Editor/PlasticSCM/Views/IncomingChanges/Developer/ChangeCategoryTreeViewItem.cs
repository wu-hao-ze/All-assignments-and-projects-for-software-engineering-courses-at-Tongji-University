using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.Merge;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal class ChangeCategoryTreeViewItem : TreeViewItem
    {
        internal MergeChangesCategory Category { get; private set; }

        internal ChangeCategoryTreeViewItem(int id, MergeChangesCategory category)
            : base(id, 0, category.CategoryType.ToString())
        {
            Category = category;
        }
    }
}
