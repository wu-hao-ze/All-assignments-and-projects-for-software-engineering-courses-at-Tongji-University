using System.IO;

using UnityEditor;
using UnityEngine;

using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Diff;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.Diff.Dialogs
{
    internal class GetRestorePathDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 600, 205);
            }
        }

        internal static GetRestorePathData GetRestorePath(
            string wkPath,
            string restorePath,
            string explanation,
            bool isDirectory,
            bool showSkipButton,
            EditorWindow parentWindow)
        {
            GetRestorePathDialog dialog = Create(
                new ProgressControlsForDialogs(),
                wkPath,
                GetProposedRestorePath.For(restorePath),
                explanation,
                isDirectory,
                showSkipButton);

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            GetRestorePathData result = dialog.BuildGetRestorePathResult();

            result.Result = GetRestorePathResultType(dialogResult);
            return result;
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.EnterRestorePathFormTitle));

            Paragraph(mExplanation);

            DoEntryArea();

            GUILayout.Space(10);

            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);

            DoButtonsArea();

            mProgressControls.ForcedUpdateProgress(this);
        }

        void DoEntryArea()
        {
            GUILayout.Label(PlasticLocalization.GetString(
                PlasticLocalization.Name.EnterRestorePathFormTextBoxExplanation),
                EditorStyles.label);

            GUILayout.BeginHorizontal();

            mRestorePath = GUILayout.TextField(
                mRestorePath, GUILayout.Width(TEXTBOX_WIDTH));

            if (GUILayout.Button("...", EditorStyles.miniButton))
            {
                mRestorePath = (mIsDirectory) ?
                    DoOpenFolderPanel(mRestorePath) :
                    DoOpenFilePanel(mRestorePath);
            }

            GUILayout.EndHorizontal();
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.EnterRestorePathFormTitle);
        }

        static string DoOpenFolderPanel(string actualPath)
        {
            string parentDirectory = null;
            string directoryName = null;

            if (!string.IsNullOrEmpty(actualPath))
            {
                parentDirectory = Path.GetDirectoryName(actualPath);
                directoryName = Path.GetFileName(actualPath);
            }

            string result = EditorUtility.SaveFolderPanel(
                PlasticLocalization.GetString(PlasticLocalization.Name.SelectPathToRestore),
                parentDirectory,
                directoryName);

            if (string.IsNullOrEmpty(result))
                return actualPath;

            return Path.GetFullPath(result);
        }

        static string DoOpenFilePanel(string actualPath)
        {
            string parentDirectory = null;
            string fileName = null;
            string extension = null;

            if (!string.IsNullOrEmpty(actualPath))
            {
                parentDirectory = Path.GetDirectoryName(actualPath);
                fileName = Path.GetFileName(actualPath);
                extension = Path.GetExtension(actualPath);
            }

            string result = EditorUtility.SaveFilePanel(
                PlasticLocalization.GetString(PlasticLocalization.Name.SelectPathToRestore),
                parentDirectory,
                fileName,
                extension);

            if (string.IsNullOrEmpty(result))
                return actualPath;

            return Path.GetFullPath(result);
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoOkButton();
                    DoSkipButton(mShowSkipButton);
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoSkipButton(mShowSkipButton);
                DoOkButton();
            }
        }

        void DoOkButton()
        {
            if (!AcceptButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.OkButton)))
                return;

            OkButtonWithValidationAction();
        }

        void DoSkipButton(bool showSkipButton)
        {
            if (!showSkipButton)
                return;

            if (!NormalButton(PlasticLocalization.GetString(PlasticLocalization.Name.SkipRestoreButton)))
                return;

            CloseButtonAction();
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
            GetRestorePathValidation.Validation(
                mWkPath, BuildGetRestorePathResult(),
                this, mProgressControls);
        }

        GetRestorePathData BuildGetRestorePathResult()
        {
            return new GetRestorePathData(mRestorePath);
        }

        static GetRestorePathData.ResultType GetRestorePathResultType(
            ResponseType dialogResult)
        {
            switch (dialogResult)
            {
                case ResponseType.None:
                    return GetRestorePathData.ResultType.Skip;
                case ResponseType.Ok:
                    return GetRestorePathData.ResultType.OK;
                case ResponseType.Cancel:
                    return GetRestorePathData.ResultType.Cancel;
            }

            return GetRestorePathData.ResultType.Cancel;
        }

        static GetRestorePathDialog Create(
            ProgressControlsForDialogs progressControls,
            string wkPath,
            string restorePath,
            string explanation,
            bool isDirectory,
            bool showSkipButton)
        {
            var instance = CreateInstance<GetRestorePathDialog>();
            instance.mWkPath = wkPath;
            instance.mRestorePath = restorePath;
            instance.mExplanation = explanation;
            instance.mIsDirectory = isDirectory;
            instance.mShowSkipButton = showSkipButton;
            instance.mEnterKeyAction = instance.OkButtonWithValidationAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            instance.mProgressControls = progressControls;
            return instance;
        }

        bool mIsDirectory;
        bool mShowSkipButton;
        string mExplanation = string.Empty;
        string mRestorePath = string.Empty;
        string mWkPath = string.Empty;

        ProgressControlsForDialogs mProgressControls;

        const float TEXTBOX_WIDTH = 520;
    }
}
