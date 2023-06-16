using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using PlasticGui;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.Configuration.CloudEdition;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.UI.UIElements;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class SignInWithEmailPanel :
        VisualElement,
        Login.INotify
    {
        internal SignInWithEmailPanel(
            CloudEditionWelcomeWindow parentWindow,
            IWelcomeWindowNotify notify,
            IPlasticWebRestApi restApi)
        {
            mParentWindow = parentWindow;
            mNotify = notify;
            mRestApi = restApi;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal void Dispose()
        {
            mSignInButton.clicked -= SignInButton_Clicked;
            mBackButton.clicked -= BackButton_Clicked;
            mSignUpButton.clicked -= SignUpButton_Clicked;
        }

        void Login.INotify.SuccessForConfigure(
            List<string> organizations,
            bool canCreateAnOrganization,
            string userName,
            string password)
        {
            mNotify.SuccessForConfigure(
                organizations);
        }

        void Login.INotify.SuccessForSSO(
            string organization)
        {
            // Do nothing
        }

        void Login.INotify.SuccessForProfile(
            string userName)
        {
            // Do nothing
        }

        void Login.INotify.SuccessForCredentials(string userName, string password)
        {
            // Do nothing
        }

        void Login.INotify.SuccessForHomeView(string userName)
        {
            // Do nothing
        }

        void Login.INotify.ValidationFailed(
            Login.ValidationResult validationResult)
        {
            if (validationResult.UserError != null)
            {
                mEmailNotificationLabel.text = validationResult.UserError;
            }

            if (validationResult.PasswordError != null)
            {
                mPasswordNotificationLabel.text = validationResult.PasswordError;
            }
        }

        void Login.INotify.SignUpNeeded(
            Login.Data loginData)
        {
            ShowSignUpNeeded();
        }

        void ShowSignUpNeeded()
        {
            mSignUpNeededNotificationContainer.Show();
        }

        void HideSignUpNeeded()
        {
            mSignUpNeededNotificationContainer.Collapse();
        }

        void Login.INotify.Error(
            string message)
        {
            HideSignUpNeeded();
            mProgressControls.ShowError(message);
        }

        void CleanNotificationLabels()
        {
            mEmailNotificationLabel.text = string.Empty;
            mPasswordNotificationLabel.text = string.Empty;

            HideSignUpNeeded();
        }

        void SignInButton_Clicked()
        {
            CleanNotificationLabels();

            Login.Run(
                mRestApi,
                new SaveCloudEditionCreds(),
                mEmailField.text,
                mPasswordField.text,
                string.Empty,
                string.Empty,
                Login.Mode.Configure,
                mProgressControls,
                this);
        }

        void BackButton_Clicked()
        {
            mNotify.Back();
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(SignInWithEmailPanel).Name);
            this.LoadStyle(typeof(SignInWithEmailPanel).Name);
        }
        
        void SignUpButton_Clicked()
        {
            Application.OpenURL(UnityUrl.DevOps.GetSignUp());
        }

        void BuildComponents()
        {
            mEmailField = this.Q<TextField>("email");
            mPasswordField = this.Q<TextField>("password");
            mEmailNotificationLabel = this.Q<Label>("emailNotification");
            mPasswordNotificationLabel = this.Q<Label>("passwordNotification");
            mSignInButton = this.Q<Button>("signIn");
            mBackButton = this.Q<Button>("back");
            mSignUpButton = this.Q<Button>("signUpButton");
            mProgressContainer = this.Q<VisualElement>("progressContainer");
            mSignUpNeededNotificationContainer = this.Q<VisualElement>("signUpNeededNotificationContainer");

            mSignInButton.clicked += SignInButton_Clicked;
            mBackButton.clicked += BackButton_Clicked;
            mSignUpButton.clicked += SignUpButton_Clicked;
            mEmailField.FocusOnceLoaded();

            mProgressControls = new ProgressControlsForDialogs(new VisualElement[] { mSignInButton });
            mProgressContainer.Add((VisualElement)mProgressControls);

            this.SetControlText<Label>("signInLabel",
                PlasticLocalization.Name.SignInWithEmail);
            this.SetControlLabel<TextField>("email",
                PlasticLocalization.Name.Email);
            this.SetControlLabel<TextField>("password",
                PlasticLocalization.Name.Password);
            this.SetControlText<Button>("signIn",
                PlasticLocalization.Name.SignIn);
            this.SetControlText<Button>("back",
                PlasticLocalization.Name.BackButton);
            this.SetControlText<Label>("signUpNeededNotificationLabel",
                PlasticLocalization.Name.SignUpNeededNoArgs);
            this.SetControlText<Button>("signUpButton",
                PlasticLocalization.Name.SignUp);
        }

        TextField mEmailField;
        TextField mPasswordField;

        Label mEmailNotificationLabel;
        Label mPasswordNotificationLabel;

        Button mSignInButton;
        Button mBackButton;
        Button mSignUpButton;

        VisualElement mProgressContainer;
        VisualElement mSignUpNeededNotificationContainer;

        IProgressControls mProgressControls;

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IWelcomeWindowNotify mNotify;
        readonly IPlasticWebRestApi mRestApi;
    }
}