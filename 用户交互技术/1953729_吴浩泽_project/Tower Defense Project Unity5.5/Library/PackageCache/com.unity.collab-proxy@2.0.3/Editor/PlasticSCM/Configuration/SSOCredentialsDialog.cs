using System;

using UnityEngine;
using UnityEditor;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Codice.CM.Common;
using Codice.Client.Common.OAuth;
using Codice.Client.Common.Connection;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.Configuration.OAuth;
using System.Collections.Generic;
using PlasticGui.WebApi.Responses;
using PlasticGui.Configuration.CloudEdition;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class SSOCredentialsDialog : PlasticDialog, OAuthSignIn.INotify, Login.INotify
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 525, 450);
            }
        }

        internal static AskCredentialsToUser.DialogData RequestCredentials(
            string cloudServer,
            EditorWindow parentWindow)
        {
            SSOCredentialsDialog dialog = Create(
                cloudServer, new ProgressControlsForDialogs());

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            return dialog.BuildCredentialsDialogData(dialogResult);
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
               PlasticLocalization.Name.CredentialsDialogTitle));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.CredentialsDialogExplanation, mServer));

            GUILayout.Space(20);

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
                mEmail, mPassword, false, SEIDWorkingMode.SSOWorkingMode);
        }

        void OAuthSignIn.INotify.SuccessForSSO(string organization)
        {
            OkButtonAction();
        }

        void OAuthSignIn.INotify.SuccessForProfile(string email)
        {
            OkButtonAction();
        }

        void OAuthSignIn.INotify.SuccessForCredentials(
            string email,
            string accessToken)
        {
            OkButtonAction();
        }

        void OAuthSignIn.INotify.SuccessForHomeView(string usrName)
        {
        }

        void OAuthSignIn.INotify.Cancel(string errorMessage)
        {
            CancelButtonAction();
        }
        void OAuthSignIn.INotify.SuccessForConfigure(
            List<string> organizations,
            bool canCreateAnOrganization,
            string userName,
            string accessToken)
        {
            mEmail = userName;
            mPassword = accessToken;

            if (!organizations.Contains(mServer))
            {
                CancelButtonAction();
                return;
            }

            CloudEditionWelcomeWindow.JoinCloudServer(
                mServer, userName, accessToken);

            GetWindow<PlasticWindow>().InitializePlastic();
            OkButtonAction();
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

        internal void OAuthSignInForConfigure(Uri signInUrl, Guid state, IGetOauthToken getOauthToken)
        {
            OAuthSignIn mSignIn = new OAuthSignIn();

            mSignIn.ForConfigure(
                signInUrl,
                state,
                mProgressControls,
                this,
                GetWindow<PlasticWindow>().CmConnectionForTesting,
                getOauthToken,
                PlasticGui.Plastic.WebRestAPI);
        }

        void DoUnityIDButton()
        {
            if (NormalButton("Sign in with Unity ID"))
            {
                Guid state = Guid.NewGuid();
                OAuthSignInForConfigure(
                    GetCloudSsoProviders.BuildAuthInfoForUnityId(string.Empty, state).SignInUrl,
                    state,
                    new GetCloudSsoToken(PlasticGui.Plastic.WebRestAPI));
            }
        }

        void DoEntriesArea()
        {
            Paragraph("Sign in with Unity ID");
            GUILayout.Space(5);

            DoUnityIDButton();

            GUILayout.Space(25);
            Paragraph("    --or--    ");

            Paragraph("Sign in with email");

            mEmail = TextEntry(PlasticLocalization.GetString(
                PlasticLocalization.Name.Email), mEmail,
                ENTRY_WIDTH, ENTRY_X);

            GUILayout.Space(5);

            mPassword = PasswordEntry(PlasticLocalization.GetString(
                PlasticLocalization.Name.Password), mPassword,
                ENTRY_WIDTH, ENTRY_X);
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
            Login.Run(
              PlasticGui.Plastic.WebRestAPI,
              new SaveCloudEditionCreds(),
              mEmail,
              mPassword,
              string.Empty,
              string.Empty,
              Login.Mode.Configure,
              mProgressControls,
              this);
        }

        void Login.INotify.SuccessForConfigure(
            List<string> organizations,
            bool canCreateAnOrganization,
            string userName,
            string password)
        {
            OkButtonAction();
        }

        void Login.INotify.SuccessForSSO(
            string organization)
        {
            OkButtonAction();
        }
        void Login.INotify.SuccessForCredentials(string userName, string password)
        {
            OkButtonAction();
        }


        void Login.INotify.SuccessForProfile(
            string userName)
        {
            OkButtonAction();
        }

        void Login.INotify.SuccessForHomeView(string userName)
        {
        }

        void Login.INotify.ValidationFailed(
            Login.ValidationResult validationResult)
        {
            CancelButtonAction();
        }

        void Login.INotify.SignUpNeeded(
            Login.Data loginData)
        {
            CancelButtonAction();
        }

        void Login.INotify.Error(
            string message)
        {
            CancelButtonAction();
        }

        static SSOCredentialsDialog Create(
            string server,
            ProgressControlsForDialogs progressControls)
        {
            var instance = CreateInstance<SSOCredentialsDialog>();
            instance.mServer = server;
            instance.mProgressControls = progressControls;
            instance.mEnterKeyAction = instance.OkButtonWithValidationAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            return instance;
        }

        string mEmail;
        string mPassword = string.Empty;

        ProgressControlsForDialogs mProgressControls;

        string mServer;

        const float ENTRY_WIDTH = 345f;
        const float ENTRY_X = 150f;
    }
}
