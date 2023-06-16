using UnityEditor.IMGUI.Controls;

using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon
{
    internal class ChangeTreeViewItem : TreeViewItem
    {
        internal IncomingChangeInfo ChangeInfo { get; private set; }

        internal ChangeTreeViewItem(int id, IncomingChangeInfo change)
            : base(id, 1)
        {
            ChangeInfo = change;

            displayName = change.GetPathString();
        }
    }
}
