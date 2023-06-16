using UnityEditor.IMGUI.Controls;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    class ChangesetListViewItem : TreeViewItem
    {
        internal object ObjectInfo { get; private set; }

        internal ChangesetListViewItem(int id, object objectInfo)
            : base(id, 1)
        {
            ObjectInfo = objectInfo;

            displayName = id.ToString();
        }
    }
}
