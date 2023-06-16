using UnityEngine;

using UnityEditor;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Codice.CM.Common;
using Codice.Client.Common.Connection;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class CredentialsDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 525, 250);
            }
        }

        internal static AskCredentialsToUser.DialogData RequestCredentials(
            string server,
            SEIDWorkingMode seidWorkingMode,
            EditorWindow parentWindow)
        {
            CredentialsDialog dialog = Create(
                server, seidWorkingMode, new ProgressControlsForDialogs());

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            return dialog.BuildCredentialsDialogData(dialogResult);
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.CredentialsDialogTitle));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.CredentialsDialogExplanation, mServer));

            GUILayout.Space(5);

            DoEntriesArea();

            GUILayout.Space(10);

            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);

            GUILayout.Space(10);

            DoButtonsArea();
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.CredentialsDialogTitle);
        }

        AskCredentialsToUser.DialogData BuildCredentialsDialogData(
            ResponseType dialogResult)
        {
            return new AskCredentialsToUser.DialogData(
                dialogResult == ResponseType.Ok,
                mUser, mPassword, mSaveProfile, mSeidWorkingMode);
        }

        void DoEntriesArea()
        {
            mUser = TextEntry(PlasticLocalization.GetString(
                PlasticLocalization.Name.UserName), mUser,
                ENTRY_WIDTH, ENTRY_X);

            GUILayout.Space(5);

            mPassword = PasswordEntry(PlasticLocalization.GetString(
                PlasticLocalization.Name.Password), mPassword,
                ENTRY_WIDTH, ENTRY_X);

            GUILayout.Space(5);

            mSaveProfile = ToggleEntry(PlasticLocalization.GetString(
                PlasticLocalization.Name.RememberCredentialsAsProfile),
                mSaveProfile, ENTRY_WIDTH, ENTRY_X);
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
            if (!AcceptButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.OkButton)))
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
            CredentialsDialogValidation.AsyncValidation(
                BuildCredentialsDialogData(ResponseType.Ok), this, mProgressControls);
        }

        static CredentialsDialog Create(
            string server,
            SEIDWorkingMode seidWorkingMode,
            ProgressControlsForDialogs progressControls)
        {
            var instance = CreateInstance<CredentialsDialog>();
            instance.mServer = server;
            instance.mSeidWorkingMode = seidWorkingMode;
            instance.mProgressControls = progressControls;
            instance.mEnterKeyAction = instance.OkButtonWithValidationAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            return instance;
        }

        string mUser;
        string mPassword = string.Empty;

        ProgressControlsForDialogs mProgressControls;
        bool mSaveProfile;

        string mServer;
        SEIDWorkingMode mSeidWorkingMode;

        const float ENTRY_WIDTH = 345f;
        const float ENTRY_X = 150f;
    }
}
