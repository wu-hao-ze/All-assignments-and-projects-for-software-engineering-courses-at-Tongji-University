using System;
using System.Collections.Generic;

using Codice.Client.BaseCommands;
using Codice.Client.Commands.CheckIn;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.Client.GameUI.Checkin;
using Codice.CM.Common;

using GluonGui;

using PlasticGui;
using PlasticGui.Gluon;
using PlasticGui.WorkspaceWindow.PendingChanges;

namespace Unity.PlasticSCM.Editor.AssetMenu.Dialogs
{
    internal static class CheckinDialogOperations
    {
        internal static void CheckinPaths(
            WorkspaceInfo wkInfo,
            List<string> paths,
            string comment,
            IWorkspaceWindow workspaceWindow,
            CheckinDialog dialog,
            GuiMessage.IGuiMessage guiMessage,
            IProgressControls progressControls,
            IMergeViewLauncher mergeViewLauncher)
        {
            BaseCommandsImpl baseCommands = new BaseCommandsImpl();

            progressControls.ShowProgress("Checkin in files");

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(50);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    CheckinParams ciParams = new CheckinParams();
                    ciParams.paths = paths.ToArray();
                    ciParams.comment = comment;
                    ciParams.time = DateTime.MinValue;
                    ciParams.flags = CheckinFlags.Recurse | CheckinFlags.ProcessSymlinks;

                    baseCommands.CheckIn(ciParams);
                },
                /*afterOperationDelegate*/ delegate
                {
                    progressControls.HideProgress();
                    ((IPlasticDialogCloser)dialog).CloseDialog();

                    if (waiter.Exception is CmClientMergeNeededException)
                    {
                        // we need to explicitly call EditorWindow.Close() to ensure
                        // that the dialog is closed before asking the user
                        dialog.Close();

                        if (!UserWantsToShowIncomingView(guiMessage))
                            return;

                        ShowIncomingChanges.FromCheckin(
                            wkInfo,
                            mergeViewLauncher,
                            progressControls);

                        return;
                    }

                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    workspaceWindow.RefreshView(ViewType.PendingChangesView);
                    workspaceWindow.RefreshView(ViewType.HistoryView);
                });
        }

        internal static void CheckinPathsPartial(
            WorkspaceInfo wkInfo,
            List<string> paths,
            string comment,
            ViewHost viewHost,
            CheckinDialog dialog,
            GuiMessage.IGuiMessage guiMessage,
            IProgressControls progressControls,
            IGluonViewSwitcher gluonViewSwitcher)
        {
            BaseCommandsImpl baseCommands = new BaseCommandsImpl();

            progressControls.ShowProgress(PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinInFilesProgress));

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(50);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    baseCommands.PartialCheckin(wkInfo, paths, comment);
                },
                /*afterOperationDelegate*/ delegate
                {
                    progressControls.HideProgress();

                    ((IPlasticDialogCloser)dialog).CloseDialog();

                    if (waiter.Exception is CheckinConflictsException)
                    {
                        // we need to explicitly call EditorWindow.Close() to ensure
                        // that the dialog is closed before asking the user
                        dialog.Close();

                        if (!UserWantsToShowIncomingView(guiMessage))
                            return;

                        gluonViewSwitcher.ShowIncomingChangesView();
                        return;
                    }

                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    viewHost.RefreshView(ViewType.CheckinView);
                    viewHost.RefreshView(ViewType.HistoryView);
                });
        }

        static bool UserWantsToShowIncomingView(GuiMessage.IGuiMessage guiMessage)
        {
            GuiMessage.GuiMessageResponseButton result = guiMessage.ShowQuestion(
                PlasticLocalization.GetString(PlasticLocalization.Name.CheckinConflictsTitle),
                PlasticLocalization.GetString(PlasticLocalization.Name.UnityCheckinConflictsExplanation),
                PlasticLocalization.GetString(PlasticLocalization.Name.CheckinShowIncomingChangesView),
                PlasticLocalization.GetString(PlasticLocalization.Name.CancelButton),
                null);

            return result == GuiMessage.GuiMessageResponseButton.Positive;
        }
    }
}
