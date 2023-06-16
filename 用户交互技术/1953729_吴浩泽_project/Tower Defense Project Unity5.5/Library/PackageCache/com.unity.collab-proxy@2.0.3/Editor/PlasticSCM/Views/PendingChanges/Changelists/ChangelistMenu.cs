using UnityEditor;
using UnityEngine;

using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Changelists
{
    internal class ChangelistMenu
    {
        internal ChangelistMenu(
            IChangelistMenuOperations changelistMenuOperations,
            bool isGluonMode)
        {
            mChangelistMenuOperations = changelistMenuOperations;

            BuildComponents(isGluonMode);
        }

        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        internal bool ProcessKeyActionIfNeeded(Event e)
        {
            ChangelistMenuOperations operationToExecute = GetMenuOperations(e);

            if (operationToExecute == ChangelistMenuOperations.None)
                return false;

            ChangelistMenuOperations operations =
                ChangelistMenuUpdater.GetAvailableMenuOperations(
                    mChangelistMenuOperations.GetSelectedChangesGroupInfo(),
                    mChangelistMenuOperations.GetSelectedChangelistInfos());

            if (!operations.HasFlag(operationToExecute))
                return false;

            ProcessMenuOperation(operationToExecute, mChangelistMenuOperations);
            return true;
        }

        void CheckinChangelistMenuItem_Click()
        {
            mChangelistMenuOperations.Checkin();
        }

        void ShelveChangelistMenuItem_Click()
        {
            mChangelistMenuOperations.Shelve();
        }

        void UndoChangelistMenuItem_Click()
        {
            mChangelistMenuOperations.Undo();
        }

        void CreateNewChangelistMenuItem_Click()
        {
            mChangelistMenuOperations.CreateNew();
        }

        void EditChangelistMenuItem_Click()
        {
            mChangelistMenuOperations.Edit();
        }

        void DeleteChangelistMenuItem_Click()
        {
            mChangelistMenuOperations.Delete();
        }

        void UpdateMenuItems(GenericMenu menu)
        {
            ChangelistMenuOperations operations = ChangelistMenuUpdater.GetAvailableMenuOperations(
                mChangelistMenuOperations.GetSelectedChangesGroupInfo(),
                mChangelistMenuOperations.GetSelectedChangelistInfos());

            if (operations == ChangelistMenuOperations.None)
            {
                menu.AddDisabledItem(GetNoActionMenuItemContent());
                return;
            }

            if (operations.HasFlag(ChangelistMenuOperations.Checkin))
                menu.AddItem(mCheckinChangelistMenuItemContent, false, CheckinChangelistMenuItem_Click);
            else
                menu.AddDisabledItem(mCheckinChangelistMenuItemContent);

            if (mShelveChangelistMenuItemContent != null)
            {
                if (operations.HasFlag(ChangelistMenuOperations.Shelve))
                    menu.AddItem(mShelveChangelistMenuItemContent, false, ShelveChangelistMenuItem_Click);
                else
                    menu.AddDisabledItem(mShelveChangelistMenuItemContent);
            }

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(ChangelistMenuOperations.Undo))
                menu.AddItem(mUndoChangelistMenuItemContent, false, UndoChangelistMenuItem_Click);
            else
                menu.AddDisabledItem(mUndoChangelistMenuItemContent);

            menu.AddSeparator(string.Empty);

            if (operations.HasFlag(ChangelistMenuOperations.CreateNew))
                menu.AddItem(mCreateNewChangelistMenuItemContent, false, CreateNewChangelistMenuItem_Click);
            else
                menu.AddDisabledItem(mCreateNewChangelistMenuItemContent);

            if (operations.HasFlag(ChangelistMenuOperations.Edit))
                menu.AddItem(mEditChangelistMenuItemContent, false, EditChangelistMenuItem_Click);
            else
                menu.AddDisabledItem(mEditChangelistMenuItemContent);
            
            if (operations.HasFlag(ChangelistMenuOperations.Delete))
                menu.AddItem(mDeleteChangelistMenuItemContent, false, DeleteChangelistMenuItem_Click);
            else
                menu.AddDisabledItem(mDeleteChangelistMenuItemContent);
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

        static ChangelistMenuOperations GetMenuOperations(Event e)
        {
            if (Keyboard.IsKeyPressed(e, KeyCode.Delete))
                return ChangelistMenuOperations.Delete;

            return ChangelistMenuOperations.None;
        }

        static void ProcessMenuOperation(
            ChangelistMenuOperations operationToExecute,
            IChangelistMenuOperations changelistMenuOperations)
        {
            if (operationToExecute == ChangelistMenuOperations.Delete)
            {
                changelistMenuOperations.Delete();
                return;
            }
        }

        void BuildComponents(bool isGluonMode)
        {
            mCheckinChangelistMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.CheckinChangelist));

            if (!isGluonMode) 
                mShelveChangelistMenuItemContent = new GUIContent(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ShelveChangelist));

            mUndoChangelistMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.UndoChangesChangelist));
            mCreateNewChangelistMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.CreateNewChangelist));
            mEditChangelistMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.EditChangelist));
            mDeleteChangelistMenuItemContent = new GUIContent(string.Format("{0} {1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.DeleteChangelist),
                GetPlasticShortcut.ForDelete()));
        }

        GUIContent mNoActionMenuItemContent;
        GUIContent mCheckinChangelistMenuItemContent;
        GUIContent mShelveChangelistMenuItemContent;
        GUIContent mUndoChangelistMenuItemContent;
        GUIContent mCreateNewChangelistMenuItemContent;
        GUIContent mEditChangelistMenuItemContent;
        GUIContent mDeleteChangelistMenuItemContent;

        readonly IChangelistMenuOperations mChangelistMenuOperations;
        bool mIsGluonMode;
    }
}