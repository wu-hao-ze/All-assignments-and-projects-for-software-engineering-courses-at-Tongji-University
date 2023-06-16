using UnityEditor.IMGUI.Controls;

using Codice.CM.Common;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs
{
    internal class RepositoryListViewItem : TreeViewItem
    {
        internal RepositoryInfo Repository { get; private set; }

        internal RepositoryListViewItem(int id, RepositoryInfo repository)
            : base(id, 0)
        {
            Repository = repository;

            displayName = repository.Name;
        }
    }
}
