using UnityEditor;
using UnityEngine;

using Codice.Client.Commands;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    class CreateChangelistDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 710, 290);
            }
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(PlasticLocalization.Name.CreateChangelistTitle);
        }

        protected override void OnModalGUI()
        {
            DoTitleArea();

            DoFieldsArea();

            DoButtonsArea();
        }

        internal static ChangelistCreationData CreateChangelist(
            WorkspaceInfo wkInfo,
            EditorWindow parentWindow)
        {
            CreateChangelistDialog dialog = Create(wkInfo);
            ResponseType dialogueResult = dialog.RunModal(parentWindow);

            ChangelistCreationData result = dialog.BuildCreationData();
            result.Result = dialogueResult == ResponseType.Ok;
            return result;
        }

        internal static ChangelistCreationData EditChangelist(
            WorkspaceInfo wkInfo,
            ChangeListInfo changelistToEdit,
            EditorWindow parentWindow)
        {
            CreateChangelistDialog dialog = Edit(wkInfo, changelistToEdit);
            ResponseType dialogueResult = dialog.RunModal(parentWindow);

            ChangelistCreationData result = dialog.BuildCreationData();
            result.Result = dialogueResult == ResponseType.Ok;
            return result;
        }

        void DoTitleArea()
        {
            GUILayout.BeginVertical();

            Title(PlasticLocalization.GetString(mIsCreateMode ?
                PlasticLocalization.Name.CreateChangelistTitle :
                PlasticLocalization.Name.EditChangelistTitle));

            GUILayout.Space(5);

            Paragraph(PlasticLocalization.GetString(mIsCreateMode ?
                PlasticLocalization.Name.CreateChangelistExplanation :
                PlasticLocalization.Name.EditChangelistExplanation));

            GUILayout.EndVertical();
        }

        void DoFieldsArea()
        {
            GUILayout.BeginVertical();

            DoNameFieldArea();

            GUILayout.Space(5);

            DoDescriptionFieldArea();

            GUILayout.Space(5);

            DoPersistentFieldArea();

            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        void DoNameFieldArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ChangelistNameEntry),
                    GUILayout.Width(100));

                GUI.SetNextControlName(NAME_FIELD_CONTROL_NAME);
                mChangelistName = GUILayout.TextField(mChangelistName);

                if (!mWasNameFieldFocused)
                {
                    EditorGUI.FocusTextInControl(NAME_FIELD_CONTROL_NAME);
                    mWasNameFieldFocused = true;
                }

                GUILayout.Space(5);
            }
        }

        void DoDescriptionFieldArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(100)))
                {
                    GUILayout.Space(49);
                    GUILayout.Label(
                        PlasticLocalization.GetString(PlasticLocalization.Name.ChangelistDescriptionEntry),
                        GUILayout.Width(100));
                }

                mChangelistDescription = GUILayout.TextArea(mChangelistDescription, GUILayout.Height(100));

                GUILayout.Space(5);
            }
        }

        void DoPersistentFieldArea()
        {
            mIsPersistent = GUILayout.Toggle(
                mIsPersistent, 
                PlasticLocalization.GetString(PlasticLocalization.Name.ChangelistPersistentCheckBoxEntry));
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.MinWidth(500)))
                {
                    GUILayout.Space(2);
                    DrawProgressForDialogs.For(
                        mProgressControls.ProgressData);
                    GUILayout.Space(2);
                }

                GUILayout.FlexibleSpace();

                DoCreateButton();
                DoCancelButton();
            }
        }

        void DoCancelButton()
        {
            if (NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
            {
                CancelButtonAction();
            }
        }

        void DoCreateButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(mIsCreateMode ?
                    PlasticLocalization.Name.CreateButton :
                    PlasticLocalization.Name.EditButton)))
                return;

            ChangelistCreationValidation.Validation(
                mWkInfo,
                mChangelistName,
                mIsCreateMode || !mChangelistName.Equals(mChangelistToEdit.Name),
                this,
                mProgressControls);
        }

        static CreateChangelistDialog Create(WorkspaceInfo wkInfo)
        {
            var instance = CreateInstance<CreateChangelistDialog>();
            instance.IsResizable = false;
            instance.mEscapeKeyAction = instance.CloseButtonAction;
            instance.mWkInfo = wkInfo;
            instance.mChangelistToEdit = null;
            instance.mChangelistName = string.Empty;
            instance.mChangelistDescription = string.Empty;
            instance.mIsPersistent = false;
            instance.mProgressControls = new ProgressControlsForDialogs();
            instance.mIsCreateMode = true;
            return instance;
        }

        static CreateChangelistDialog Edit(
            WorkspaceInfo wkInfo, 
            ChangeListInfo changelistToEdit)
        {
            var instance = CreateInstance<CreateChangelistDialog>();
            instance.IsResizable = false;
            instance.mEscapeKeyAction = instance.CloseButtonAction;
            instance.mWkInfo = wkInfo;
            instance.mChangelistToEdit = changelistToEdit;
            instance.mChangelistName = changelistToEdit.Name;
            instance.mChangelistDescription = changelistToEdit.Description;
            instance.mIsPersistent = changelistToEdit.IsPersistent;
            instance.mProgressControls = new ProgressControlsForDialogs();
            instance.mIsCreateMode = false;
            return instance;
        }

        ChangelistCreationData BuildCreationData()
        {
            ChangeListInfo changelistInfo = new ChangeListInfo();
            changelistInfo.Name = mChangelistName;
            changelistInfo.Description = mChangelistDescription;
            changelistInfo.IsPersistent = mIsPersistent;
            changelistInfo.Type = ChangeListType.UserDefined;

            return new ChangelistCreationData(changelistInfo);
        }

        ProgressControlsForDialogs mProgressControls;

        WorkspaceInfo mWkInfo;
        ChangeListInfo mChangelistToEdit;

        string mChangelistName;
        string mChangelistDescription;
        bool mIsPersistent;

        bool mIsCreateMode;

        bool mWasNameFieldFocused;
        const string NAME_FIELD_CONTROL_NAME = "CreateChangelistNameField";
    }
}