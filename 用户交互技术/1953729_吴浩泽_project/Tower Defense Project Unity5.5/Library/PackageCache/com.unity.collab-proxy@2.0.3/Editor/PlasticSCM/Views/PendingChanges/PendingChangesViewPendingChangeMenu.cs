using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Codice.Client.Commands;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using PlasticGui.WorkspaceWindow.Open;
using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Changelists;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class PendingChangesViewPendingChangeMenu
    {
        internal interface IMetaMenuOperations
        {
            void DiffMeta();
            void OpenMeta();
            void OpenMetaWith();
            void OpenMetaInExplorer();
            void HistoryMeta();
            bool SelectionHasMeta();
        }

        internal PendingChangesViewPendingChangeMenu(
            WorkspaceInfo wkInfo,
            IPendingChangesMenuOperations pendingChangesMenuOperations,
            IChangelistMenuOperations changelistMenuOperations,
            IOpenMenuOperations openMenuOperations,
            IMetaMenuOperations metaMenuOperations,
            IFilesFilterPatternsMenuOperations filterMenuOperations)
        {
            mPendingChangesMenuOperations = pendingChangesMenuOperations;
            mChangelistMenuOperations = changelistMenuOperations;
            mOpenMenuOperations = openMenuOperations;
            mMetaMenuOperations = metaMenuOperations;

            mFilterMenuBuilder = new FilesFilterPatternsMenuBuilder(filterMenuOperations);
            mMoveToChangelistMenuBuilder = new MoveToChangelistMenuBuilder(
                wkInfo,
                changelistMenuOperations);

            BuildComponents();
        }

        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        internal bool ProcessKeyActionIfNeeded(Event e)
        {
            PendingChangesMenuOperations operationToExecute =
                GetPendingChangesMenuOperation(e);

            OpenMenuOperations openOperationToExecute =
                GetOpenMenuOperation(e);

            if (operationToExecute == PendingChangesMenuOperations.None &&
                openOperationToExecute == OpenMenuOperations.None)
                return false;

            SelectedChangesGroupInfo info =
                mPendingChangesMenuOperations.GetSelectedChangesGroupInfo();

            if (operationToExecute != PendingChangesMenuOperations.None)
                return ProcessKeyActionForPendingChangesMenu(
                    operationToExecute, mPendingChangesMenuOperations, info);

            return ProcessKeyActionForOpenMenu(
                openOperationToExecute, mOpenMenuOperations, info);
        }

        void OpenMenuItem_Click()
        {
            mOpenMenuOperations.Open();
        }

        void OpenWithMenuItem_Click()
        {
            mOpenMenuOperations.OpenWith();
        }

        void OpenInExplorerMenuItem_Click()
        {
            mOpenMenuOperations.OpenInExplorer();
        }

        void OpenMetaMenuItem_Click()
        {
            mMetaMenuOperations.OpenMeta();
        }

        void OpenMetaWithMenuItem_Click()
        {
            mMetaMenuOperations.OpenMetaWith();
        }

        void OpenMetaInExplorerMenuItem_Click()
        {
            mMetaMenuOperations.OpenMetaInExplorer();
        }

        void DiffMenuItem_Click()
        {
            mPendingChangesMenuOperations.Diff();
        }

        void DiffMetaMenuItem_Click()
        {
            mMetaMenuOperations.DiffMeta();
        }

        void UndoChangesMenuItem_Click()
        {
            mPendingChangesMenuOperations.UndoChanges();
        }

        void CheckoutMenuItem_Click()
        {
            mPendingChangesMenuOperations.ApplyLocalChanges();
        }

        void DeleteMenuItem_Click()
        {
            mPendingChangesMenuOperations.Delete();
        }

        void HistoryMenuItem_Click()
        {
            mPendingChangesMenuOperations.History();
        }

        void HistoryMetaMenuItem_Click()
        {
            mMetaMenuOperations.HistoryMeta();
        }

        void UpdateMenuItems(GenericMenu menu)
        {
            SelectedChangesGroupInfo info =
                mPendingChangesMenuOperations.GetSelectedChangesGroupInfo();

            PendingChangesMenuOperations operations =
                PendingChangesMenuUpdater.GetAvailableMenuOperations(info);

            ChangelistMenuOperations changelistOperations =
                ChangelistMenuOperations.None;

            OpenMenuOperations openOperations =
                GetOpenMenuOperations.ForPendingChangesView(info);

            bool useChangelists = PlasticGuiConfig.Get().
                Configuration.CommitUseChangeLists;

            if (useChangelists)
            {
                List<ChangeListInfo> selectedChangelists =
                    mChangelistMenuOperations.GetSelectedChangelistInfos();

                changelistOperations = ChangelistMenuUpdater.
                    GetAvailableMenuOperations(info, selectedChangelists);
            }

            if (operations == PendingChangesMenuOperations.None &&
                changelistOperations == ChangelistMenuOperations.None &&
                openOperations == OpenMenuOperations.None)
            {
                menu.AddDisabledItem(GetNoActionMenuItemContent());
                return;
            }

            UpdateOpenMenuItems(menu, openOperations);

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.DiffWorkspaceContent))
                menu.AddItem(mDiffMenuItemContent, false, DiffMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffMenuItemContent);

            if (mMetaMenuOperations.SelectionHasMeta())
            {
                if (operations.HasFlag(PendingChangesMenuOperations.DiffWorkspaceContent))
                    menu.AddItem(mDiffMetaMenuItemContent, false, DiffMetaMenuItem_Click);
                else
                    menu.AddDisabledItem(mDiffMetaMenuItemContent);
            }

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.UndoChanges))
                menu.AddItem(mUndoChangesMenuItemContent, false, UndoChangesMenuItem_Click);
            else
                menu.AddDisabledItem(mUndoChangesMenuItemContent);

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.ApplyLocalChanges))
                menu.AddItem(mCheckoutMenuItemContent, false, CheckoutMenuItem_Click);
            else
                menu.AddDisabledItem(mCheckoutMenuItemContent);

            if (operations.HasFlag(PendingChangesMenuOperations.Delete))
                menu.AddItem(mDeleteMenuItemContent, false, DeleteMenuItem_Click);
            else
                menu.AddDisabledItem(mDeleteMenuItemContent);

            if (useChangelists)
            {
                menu.AddSeparator(string.Empty);
                
                mMoveToChangelistMenuBuilder.UpdateMenuItems(
                    menu,
                    changelistOperations,
                    info.SelectedChanges,
                    info.ChangelistsWithSelectedChanges);
            }

            menu.AddSeparator(string.Empty);

            mFilterMenuBuilder.UpdateMenuItems(
                menu, FilterMenuUpdater.GetMenuActions(info));

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(PendingChangesMenuOperations.History))
                menu.AddItem(mViewHistoryMenuItemContent, false, HistoryMenuItem_Click);
            else
                menu.AddDisabledItem(mViewHistoryMenuItemContent, false);

            if (mMetaMenuOperations.SelectionHasMeta())
            {
                if (operations.HasFlag(PendingChangesMenuOperations.History))
                    menu.AddItem(mViewHistoryMetaMenuItemContent, false, HistoryMetaMenuItem_Click);
                else
                    menu.AddDisabledItem(mViewHistoryMetaMenuItemContent);
            }
        }

        void UpdateOpenMenuItems(GenericMenu menu, OpenMenuOperations operations)
        {
            if (!operations.HasFlag(OpenMenuOperations.Open) &&
                !operations.HasFlag(OpenMenuOperations.OpenWith) &&
                !operations.HasFlag(OpenMenuOperations.OpenInExplorer))
            {
                menu.AddDisabledItem(mOpenSubmenuItemContent);
                return;
            }

            if (operations.HasFlag(OpenMenuOperations.Open))
                menu.AddItem(mOpenMenuItemContent, false, OpenMenuItem_Click);
            else
                menu.AddDisabledItem(mOpenMenuItemContent);

            if (operations.HasFlag(OpenMenuOperations.OpenWith))
                menu.AddItem(mOpenWithMenuItemContent, false, OpenWithMenuItem_Click);
            else
                menu.AddDisabledItem(mOpenWithMenuItemContent);

            if (operations.HasFlag(OpenMenuOperations.OpenInExplorer))
                menu.AddItem(mOpenInExplorerMenuItemContent, false, OpenInExplorerMenuItem_Click);
            else
                menu.AddDisabledItem(mOpenInExplorerMenuItemContent);

            if (!mMetaMenuOperations.SelectionHasMeta())
                return;

            menu.AddSeparator(PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen) + "/");

            if (operations.HasFlag(OpenMenuOperations.Open))
                menu.AddItem(mOpenMetaMenuItemContent, false, OpenMetaMenuItem_Click);
            else
                menu.AddDisabledItem(mOpenMetaMenuItemContent);

            if (operations.HasFlag(OpenMenuOperations.OpenWith))
                menu.AddItem(mOpenMetaWithMenuItemContent, false, OpenMetaWithMenuItem_Click);
            else
                menu.AddDisabledItem(mOpenMetaWithMenuItemContent);

            if (operations.HasFlag(OpenMenuOperations.OpenInExplorer))
                menu.AddItem(mOpenMetaInExplorerMenuItemContent, false, OpenMetaInExplorerMenuItem_Click);
            else
                menu.AddDisabledItem(mOpenMetaInExplorerMenuItemContent);
        }

        GUIContent GetNoActionMenuItemContent()
        {
            if (mNoActionMenuItemContent == null)
            {
                mNoActionMenuItemContent = new GUIContent(PlasticLocalization.GetString(
                    PlasticLocalization.Name.NoActionMenuItem));
            }

            return mNoActionMenuItemContent;
        }

        void BuildComponents()
        {
            mOpenSubmenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen));
            mOpenMenuItemContent = new GUIContent(
                UnityMenuItem.GetText(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                    string.Format("{0} {1}",
                        PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                        GetPlasticShortcut.ForOpen())));
            mOpenWithMenuItemContent = new GUIContent(
                UnityMenuItem.GetText(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpenWith)));
            mOpenInExplorerMenuItemContent = new GUIContent(
                UnityMenuItem.GetText(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                    PlasticLocalization.GetString(PlasticLocalization.Name.OpenInExplorerMenuItem)));
            mOpenMetaMenuItemContent = new GUIContent(
                UnityMenuItem.GetText(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                    PlasticLocalization.GetString(PlasticLocalization.Name.OpenMeta)));
            mOpenMetaWithMenuItemContent = new GUIContent(
                UnityMenuItem.GetText(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                    PlasticLocalization.GetString(PlasticLocalization.Name.OpenMetaWith)));
            mOpenMetaInExplorerMenuItemContent = new GUIContent(
                UnityMenuItem.GetText(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsMenuItemOpen),
                    PlasticLocalization.GetString(PlasticLocalization.Name.OpenMetaInExplorer)));
            mDiffMenuItemContent = new GUIContent(string.Format("{0} {1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.DiffMenuItem),
                GetPlasticShortcut.ForDiff()));
            mDiffMetaMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.DiffMetaMenuItem));
            mUndoChangesMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.PendingChangesMenuItemUndoChanges));
            mCheckoutMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.PendingChangesMenuItemCheckout));
            mDeleteMenuItemContent = new GUIContent(string.Format("{0} {1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.PendingChangesMenuItemDelete),
                GetPlasticShortcut.ForDelete()));
            mViewHistoryMenuItemContent = new GUIContent(string.Format("{0} {1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.ViewHistoryMenuItem),
                GetPlasticShortcut.ForHistory()));
            mViewHistoryMetaMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.ViewHistoryMetaMenuItem));

            mFilterMenuBuilder.BuildIgnoredSubmenuItem();
            mFilterMenuBuilder.BuildHiddenChangesSubmenuItem();

            mMoveToChangelistMenuBuilder.BuildComponents();
        }

        static bool ProcessKeyActionForPendingChangesMenu(
            PendingChangesMenuOperations operationToExecute,
            IPendingChangesMenuOperations pendingChangesMenuOperations,
            SelectedChangesGroupInfo info)
        {
            PendingChangesMenuOperations operations =
                    PendingChangesMenuUpdater.GetAvailableMenuOperations(info);

            if (!operations.HasFlag(operationToExecute))
                return false;

            ProcessPendingChangesMenuOperation(
                operationToExecute, pendingChangesMenuOperations);

            return true;
        }

        static bool ProcessKeyActionForOpenMenu(
            OpenMenuOperations openOperationToExecute,
            IOpenMenuOperations openMenuOperations,
            SelectedChangesGroupInfo info)
        {
            OpenMenuOperations openOperations =
                GetOpenMenuOperations.ForPendingChangesView(info);

            if (!openOperations.HasFlag(openOperationToExecute))
                return false;

            ProcessOpenMenuOperation(
                openOperationToExecute, openMenuOperations);

            return true;
        }

        static void ProcessPendingChangesMenuOperation(
            PendingChangesMenuOperations operationToExecute,
            IPendingChangesMenuOperations pendingChangesMenuOperations)
        {
            if (operationToExecute == PendingChangesMenuOperations.DiffWorkspaceContent)
            {
                pendingChangesMenuOperations.Diff();
                return;
            }

            if (operationToExecute == PendingChangesMenuOperations.Delete)
            {
                pendingChangesMenuOperations.Delete();
                return;
            }

            if (operationToExecute == PendingChangesMenuOperations.History)
            {
                pendingChangesMenuOperations.History();
                return;
            }
        }

        static void ProcessOpenMenuOperation(
            OpenMenuOperations openOperationToExecute,
            IOpenMenuOperations openMenuOperations)
        {
            if (openOperationToExecute == OpenMenuOperations.Open)
            {
                openMenuOperations.Open();
                return;
            }
        }

        static PendingChangesMenuOperations GetPendingChangesMenuOperation(Event e)
        {
            if (Keyboard.IsControlOrCommandKeyPressed(e) && Keyboard.IsKeyPressed(e, KeyCode.D))
                return PendingChangesMenuOperations.DiffWorkspaceContent;

            if (Keyboard.IsKeyPressed(e, KeyCode.Delete))
                return PendingChangesMenuOperations.Delete;

            if (Keyboard.IsControlOrCommandKeyPressed(e) && Keyboard.IsKeyPressed(e, KeyCode.H))
                return PendingChangesMenuOperations.History;

            return PendingChangesMenuOperations.None;
        }

        static OpenMenuOperations GetOpenMenuOperation(Event e)
        {
            if (Keyboard.IsShiftPressed(e) && Keyboard.IsKeyPressed(e, KeyCode.O))
                return OpenMenuOperations.Open;

            return OpenMenuOperations.None;
        }

        GUIContent mNoActionMenuItemContent;

        GUIContent mOpenSubmenuItemContent;
        GUIContent mOpenMenuItemContent;
        GUIContent mOpenWithMenuItemContent;
        GUIContent mOpenInExplorerMenuItemContent;
        GUIContent mOpenMetaMenuItemContent;
        GUIContent mOpenMetaWithMenuItemContent;
        GUIContent mOpenMetaInExplorerMenuItemContent;
        GUIContent mDiffMenuItemContent;
        GUIContent mDiffMetaMenuItemContent;
        GUIContent mUndoChangesMenuItemContent;
        GUIContent mCheckoutMenuItemContent;
        GUIContent mDeleteMenuItemContent;
        GUIContent mViewHistoryMenuItemContent;
        GUIContent mViewHistoryMetaMenuItemContent;

        readonly WorkspaceInfo mWkInfo;
        readonly IMetaMenuOperations mMetaMenuOperations;
        readonly IOpenMenuOperations mOpenMenuOperations;
        readonly IChangelistMenuOperations mChangelistMenuOperations;
        readonly IPendingChangesMenuOperations mPendingChangesMenuOperations;
        readonly FilesFilterPatternsMenuBuilder mFilterMenuBuilder;
        readonly MoveToChangelistMenuBuilder mMoveToChangelistMenuBuilder;
        readonly bool mIsGluonMode;
    }
}