using System.Collections.Generic;

using UnityEditor;

using Codice.Client.BaseCommands;
using Codice.CM.Common;
using GluonGui.WorkspaceWindow.Views.Checkin.Operations;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    internal class LaunchDependenciesDialog : DependenciesHandler.IDependenciesDialog
    {
        internal LaunchDependenciesDialog(string operation, EditorWindow parentWindow)
        {
            mOperation = operation;
            mParentWindow = parentWindow;
        }

        bool DependenciesHandler.IDependenciesDialog.IncludeDependencies(
            WorkspaceInfo wkInfo, IList<ChangeDependencies<ChangeInfo>> dependencies)
        {
            return DependenciesDialog.IncludeDependencies(
                wkInfo, dependencies, mOperation, mParentWindow);
        }

        string mOperation;
        EditorWindow mParentWindow;
    }
}
