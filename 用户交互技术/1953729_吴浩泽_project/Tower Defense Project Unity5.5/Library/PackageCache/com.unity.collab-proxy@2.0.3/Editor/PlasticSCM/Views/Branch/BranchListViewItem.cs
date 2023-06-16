using UnityEditor.IMGUI.Controls;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    class BranchListViewItem : TreeViewItem
    {
        internal object ObjectInfo { get; private set; }

        internal BranchListViewItem(int id, object objectInfo)
            : base(id, 1)
        {
            ObjectInfo = objectInfo;

            displayName = id.ToString();
        }
    }
}
