using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal class ClientDiffTreeViewItem : TreeViewItem
    {
        internal ClientDiffInfo Difference { get; private set; }

        internal ClientDiffTreeViewItem(
            int id, int depth, ClientDiffInfo diff)
            : base(id, depth)
        {
            Difference = diff;

            displayName = diff.PathString;
        }
    }
}
