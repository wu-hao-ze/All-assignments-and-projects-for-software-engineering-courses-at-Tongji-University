using UnityEditor.IMGUI.Controls;

using Codice.Client.BaseCommands;

namespace Unity.PlasticSCM.Editor.Gluon.UpdateReport
{
    internal class ErrorListViewItem : TreeViewItem
    {
        internal ErrorMessage ErrorMessage { get; private set; }

        internal ErrorListViewItem(int id, ErrorMessage errorMessage)
            : base(id, 0)
        {
            ErrorMessage = errorMessage;

            displayName = errorMessage.Path;
        }
    }
}

