using UnityEditor;
using UnityEngine;

using Codice.CM.Common;

using PlasticGui;
using PlasticGui.WorkspaceWindow.QueryViews.Branches;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.Branches.Dialogs
{
    internal class RenameBranchDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 500, 200);
            }
        }

        internal static BranchRenameData GetBranchRenameData(
            RepositorySpec repSpec,
            BranchInfo branchInfo,
            EditorWindow parentWindow)
        {
            RenameBranchDialog dialog = Create(
                repSpec,
                branchInfo,
                new ProgressControlsForDialogs());

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            BranchRenameData result = dialog.BuildRenameData();

            result.Result = dialogResult == ResponseType.Ok;
            return result;
        }

        static RenameBranchDialog Create(
            RepositorySpec repSpec,
            BranchInfo branchInfo,
            ProgressControlsForDialogs progressControls)
        {
            var instance = CreateInstance<RenameBranchDialog>();
            instance.mRepSpec = repSpec;
            instance.mBranchInfo = branchInfo;
            instance.mBranchName = BranchRenameUserInfo.GetShortBranchName(branchInfo.BranchName);
            instance.mTitle = PlasticLocalization.GetString(
               PlasticLocalization.Name.RenameBranchTitle);
            instance.mProgressControls = progressControls;
            return instance;
        }

        protected override string GetTitle()
        {
            return mTitle;
        }

        protected override void OnModalGUI()
        {
            Title(mTitle);

            GUILayout.Space(10f);

            DoInputArea();

            GUILayout.Space(10f);

            DrawProgressForDialogs.For(mProgressControls.ProgressData);

            GUILayout.Space(10f);

            DoButtonsArea();
        }

        void DoInputArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(
                    PlasticLocalization.GetString(PlasticLocalization.Name.NewName),
                    GUILayout.ExpandWidth(false));

                GUILayout.Space(10f);

                GUI.SetNextControlName(RENAME_BRANCH_TEXTAREA_NAME);

                mBranchName = GUILayout.TextField(
                    mBranchName,
                    GUILayout.ExpandWidth(true));

                if (!mTextAreaFocused)
                {
                    EditorGUI.FocusTextInControl(RENAME_BRANCH_TEXTAREA_NAME);
                    mTextAreaFocused = true;
                }
            }
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoOkButton();
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoOkButton();
            }
        }

        void DoOkButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.RenameButton)))
                return;

            OkButtonWithValidationAction();
        }

        void DoCancelButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
                return;

            CancelButtonAction();
        }

        void OkButtonWithValidationAction()
        {
            BranchRenameValidation.AsyncValidation(
                BuildRenameData(),
                this,
                mProgressControls);
        }

        BranchRenameData BuildRenameData()
        {
            return new BranchRenameData(mRepSpec, mBranchInfo, mBranchName);
        }

        string mTitle;
        string mBranchName;

        bool mTextAreaFocused;

        RepositorySpec mRepSpec;
        BranchInfo mBranchInfo;

        ProgressControlsForDialogs mProgressControls;

        const string RENAME_BRANCH_TEXTAREA_NAME = "rename_branch_textarea";
    }
}