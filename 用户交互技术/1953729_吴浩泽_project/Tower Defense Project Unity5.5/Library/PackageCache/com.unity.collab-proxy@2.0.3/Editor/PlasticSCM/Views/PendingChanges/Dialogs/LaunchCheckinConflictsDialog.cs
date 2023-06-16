using System.Collections.Generic;

using UnityEditor;

using Codice.Client.GameUI.Checkin;
using GluonGui.Dialog;
using GluonGui.WorkspaceWindow.Views.Checkin.Operations;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    internal class LaunchCheckinConflictsDialog : CheckinUIOperation.ICheckinConflictsDialog
    {
        internal LaunchCheckinConflictsDialog(EditorWindow window)
        {
            mWindow = window;
        }

        Result CheckinUIOperation.ICheckinConflictsDialog.Show(
            IList<CheckinConflict> conflicts,
            PlasticLocalization.Name dialogTitle,
            PlasticLocalization.Name dialogExplanation,
            PlasticLocalization.Name okButtonCaption)
        {
            ResponseType responseType = CheckinConflictsDialog.Show(
                conflicts, dialogTitle, dialogExplanation,
                okButtonCaption, mWindow);

            return responseType == ResponseType.Ok ?
                Result.Ok : Result.Cancel;
        }

        EditorWindow mWindow;
    }
}
