using System;

using UnityEngine;
using UnityEngine.UIElements;

using Codice.Client.Common;
using Codice.Client.Common.OAuth;
using PlasticGui;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.Configuration.OAuth;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class SignInPanel : VisualElement
    {
        internal SignInPanel(
            CloudEditionWelcomeWindow parentWindow,
            IPlasticWebRestApi restApi,
            CmConnection cmConnection)
        {
            mParentWindow = parentWindow;
            mRestApi = restApi;
            mCmConnection = cmConnection;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal void Dispose()
        {
            mSignInWithUnityIdButton.clicked -= SignInWithUnityIdButton_Clicked;
            mSignInWithEmailButton.clicked -= SignInWithEmailButton_Clicked;
            mPrivacyPolicyStatementButton.clicked -= PrivacyPolicyStatementButton_Clicked;
            mSignUpButton.clicked -= SignUpButton_Clicked;

            if (mSignInWithEmailPanel != null)
                mSignInWithEmailPanel.Dispose();

            if (mWaitingSignInPanel != null)
                mWaitingSignInPanel.Dispose();
        }

        void SignInWithEmailButton_Clicked()
        {
            mSignInWithEmailPanel = new SignInWithEmailPanel(
                mParentWindow,
                mParentWindow,
                mRestApi);

            mParentWindow.ReplaceRootPanel(mSignInWithEmailPanel);
        }

        void SignUpButton_Clicked()
        {
            Application.OpenURL(UnityUrl.DevOps.GetSignUp());
        }

        internal void SignInWithUnityIdButton_Clicked()
        {
            mWaitingSignInPanel = new WaitingSignInPanel(
                mParentWindow,
                mParentWindow,
                mRestApi,
                mCmConnection);

            mParentWindow.ReplaceRootPanel(mWaitingSignInPanel);

            Guid state = Guid.NewGuid();
            mWaitingSignInPanel.OAuthSignInForConfigure(
                GetCloudSsoProviders.BuildAuthInfoForUnityId(string.Empty, state).SignInUrl,
                state,
                new GetCloudSsoToken(mRestApi));
        }

        internal void SignInWithUnityIdButtonAutoLogin()
        {
            mWaitingSignInPanel = new WaitingSignInPanel(
                mParentWindow,
                mParentWindow,
                mRestApi,
                mCmConnection);

            mParentWindow.ReplaceRootPanel(mWaitingSignInPanel);
        }

        void PrivacyPolicyStatementButton_Clicked()
        {
            Application.OpenURL(SignUp.PRIVACY_POLICY_URL);
        }

        void BuildComponents()
        {
            BuildSignUpArea();
            BuildSignInUnityIdArea();
            BuildSignInEmailArea();
            BuildPrivatePolicyArea();
        }

        void BuildPrivatePolicyArea()
        {
            this.SetControlText<Label>(
                "privacyStatementText",
                PlasticLocalization.Name.PrivacyStatementText,
                PlasticLocalization.GetString(PlasticLocalization.Name.PrivacyStatement));

            mPrivacyPolicyStatementButton = this.Query<Button>("privacyStatement");
            mPrivacyPolicyStatementButton.text = PlasticLocalization.Name.PrivacyStatement.GetString();
            mPrivacyPolicyStatementButton.clicked += PrivacyPolicyStatementButton_Clicked;
        }

        void BuildSignInEmailArea()
        {
            this.SetControlImage(
                "iconEmail",
                Images.Name.ButtonSsoSignInEmail);

            mSignInWithEmailButton = this.Query<Button>("emailButton");
            mSignInWithEmailButton.text = PlasticLocalization.Name.SignInWithEmail.GetString();
            mSignInWithEmailButton.clicked += SignInWithEmailButton_Clicked;
        }

        void BuildSignInUnityIdArea()
        {
            this.SetControlImage(
                "iconUnity",
                Images.Name.ButtonSsoSignInUnity);

            mSignInWithUnityIdButton = this.Query<Button>("unityIDButton");
            mSignInWithUnityIdButton.text = PlasticLocalization.Name.SignInWithUnityID.GetString();
            mSignInWithUnityIdButton.clicked += SignInWithUnityIdButton_Clicked;
        }

        void BuildSignUpArea()
        {
            Label signUpLabel = this.Query<Label>("signUpLabel");
            signUpLabel.text = PlasticLocalization.Name.LoginOrSignUp.GetString();

            mSignUpButton = this.Query<Button>("signUpButton");
            mSignUpButton.text = PlasticLocalization.Name.SignUpButton.GetString();
            mSignUpButton.clicked += SignUpButton_Clicked;
        }

        void InitializeLayoutAndStyles()
        {
            AddToClassList("grow");
            
            this.LoadLayout(typeof(SignInPanel).Name);
            this.LoadStyle(typeof(SignInPanel).Name);
        }

        SignInWithEmailPanel mSignInWithEmailPanel;
        WaitingSignInPanel mWaitingSignInPanel;
        Button mSignInWithUnityIdButton;
        Button mSignInWithEmailButton;
        Button mPrivacyPolicyStatementButton;
        Button mSignUpButton;

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IPlasticWebRestApi mRestApi;
        readonly CmConnection mCmConnection;
    }
}