using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using PlasticGui;
using Codice.CM.Common;
using Codice.Client.Common;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.Configuration.TeamEdition;
using PlasticGui.Configuration;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.Views.Welcome;

namespace Unity.PlasticSCM.Editor.Configuration.TeamEdition
{
    internal class TeamEditionConfigurationWindow : EditorWindow
    {
        internal static void ShowWindow(IPlasticWebRestApi restApi, WelcomeView welcomeView)
        {
            TeamEditionConfigurationWindow window = GetWindow<TeamEditionConfigurationWindow>();
            window.mRestApi = restApi;
            window.mWelcomeView = welcomeView;
            window.titleContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.WelcomeToUnityVCS));
            window.minSize = window.maxSize = new Vector2(650, 300);
            window.Show();
        }

        void OnEnable()
        {
            InitializeLayoutAndStyles();

            BuildComponents();
        }

        void Dispose()
        {
            mConnectButton.clicked -= ConnectButton_Clicked;
            mCheckConnectionButton.clicked -= CheckConnectionButton_Clicked;
            mOkButton.clicked -= OkButton_Clicked;
            mCancelButton.clicked -= CancelButton_Clicked;
            mServerTextField.UnregisterValueChangedCallback(OnServerTextFieldChanged);
            mUseSslToggle.UnregisterValueChangedCallback(OnUseSslToggleChanged);

            mLoadingSpinner.Dispose();
        }

        void ConnectButton_Clicked()
        {
            ConfigurationConnectServerButtonClickEvent.Click(
                server: mUserAssistant.GetProposedServer(),
                HideValidation: HideValidation,
                ShowError: ShowServerValidationError,
                ShowProgress: ShowProgress,
                HideProgress: HideProgress,
                ShowNotification: ShowServerNotificationMessage,
                DisableButtons: () => { mConnectButton.SetEnabled(false); },
                EnableButtons: () => { mConnectButton.SetEnabled(true); },
                UpdatePasswordEntries: (seidWorkingMode) =>
                {
                    UpdatePasswordEntries(ValidateServerAndCreds.
                        IsPasswordRequired(seidWorkingMode));
                },
                NotifyWorkingMode: (mode) => { mSEIDWorkingMode = mode; },
                NotifyConnectedStatus: (b) => { });

            mUserTextField.SetEnabled(true);
        }

        void OnDestroy()
        {
            Dispose();

            if (mWelcomeView != null)
                mWelcomeView.OnUserClosedConfigurationWindow();
        }

        void CheckConnectionButton_Clicked()
        {
            ConfigurationCheckCredentialsButtonClickEvent.Click(
                mSEIDWorkingMode,
                mUserTextField.value,
                mPasswordTextField.value,
                Edition.Team,
                mUserAssistant,
                HideCredentialsValidationError,
                ShowCheckCredentialsError,
                ShowProgress,
                HideProgress,
                ShowNotification: ShowCredentialsNotificationMessage,
                DisableButtons: () => { mCheckConnectionButton.SetEnabled(false); mConnectButton.SetEnabled(false); },
                EnableButtons: () => { mCheckConnectionButton.SetEnabled(true); mConnectButton.SetEnabled(true); },
                NotifyWorkingMode: (mode) => { mSEIDWorkingMode = mode; },
                NotifyConnectedStatus: (status) => { },
                restApi: mRestApi,
                cmConnection: CmConnection.Get());
        }

        void CancelButton_Clicked()
        {
            Close();
        }

        void OkButton_Clicked()
        {
            if (!ValidateServerAndCreds.IsValidInput(
                mUserAssistant.GetProposedServer(),
                mUserTextField.value,
                mSEIDWorkingMode,
                mPasswordTextField.value,
                ShowCheckCredentialsError))
            {
                return;
            }

            ConfigurationActions.SaveClientConfig(
                mServerTextField.value,
                mSEIDWorkingMode,
                mUserTextField.value,
                mPasswordTextField.value);

            Close();
        }

        void HideCredentialsValidationError()
        {
            mCredentialsLabel.RemoveFromClassList("error");
            mCredentialsLabel.Hide();
        }

        void BuildComponents()
        {
            VisualElement root = rootVisualElement;

            root.Query<Label>("plasticConfigurationTitle").First().text =
                PlasticLocalization.GetString(PlasticLocalization.Name.PlasticConfigurationTitleUnityVCS);

            root.SetControlText<Label>(
                "plasticConfigurationExplanation",
                PlasticLocalization.Name.PlasticConfigurationExplanation);

            root.SetControlText<Label>("configurationServerInfo",
                PlasticLocalization.Name.PlasticSCMServerLabel);

            root.SetControlText<Button>(
                "connect",
                PlasticLocalization.Name.Connect);

            root.SetControlText<Label>("useSsl",
                PlasticLocalization.Name.UseSsl);

            root.SetControlText<Label>("configurationUserNameInfo",
               PlasticLocalization.Name.ConfigurationUserNameInfo);

            root.SetControlText<Label>("configurationCredentialsInfo",
                PlasticLocalization.Name.ConfigurationCredentialsInfo);

            root.SetControlText<Button>("check",
                PlasticLocalization.Name.Check);

            root.SetControlText<Label>("credentialsOk",
                PlasticLocalization.Name.CredentialsOK);

            root.SetControlText<Button>("okButton",
                PlasticLocalization.Name.OkButton);

            root.SetControlText<Button>("cancelButton",
                PlasticLocalization.Name.CancelButton);

            mSpinnerContainer = root.Query<VisualElement>("spinnerContainer").First();
            mSpinnerLabel = root.Query<Label>("spinnerLabel").First();

            mLoadingSpinner = new LoadingSpinner();
            mSpinnerContainer.Add(mLoadingSpinner);
            mSpinnerContainer.Hide();

            mCheckConnectionButton = root.Query<Button>("check").First();
            mCheckConnectionButton.clicked += CheckConnectionButton_Clicked;

            mConnectButton = root.Query<Button>("connect").First();
            mConnectButton.clicked += ConnectButton_Clicked;

            mServerTextField = root.Query<TextField>("serverTextField").First();
            mServerTextField.RegisterValueChangedCallback(OnServerTextFieldChanged);

            mUseSslToggle = root.Query<Toggle>("useSslToogle").First();
            mUseSslToggle.RegisterValueChangedCallback(OnUseSslToggleChanged);

            mUserTextField = root.Query<TextField>("userTextField").First();
            mUserTextField.SetEnabled(false);

            mPasswordTextField = root.Query<TextField>("passwordTextField").First();
            mPasswordTextField.isPasswordField = true;

            mConnectedLabel = root.Query<Label>("connectedLabel").First();

            mCredentialsLabel = root.Query<Label>("credentialsOk").First();

            mOkButton = root.Query<Button>("okButton").First();
            mOkButton.clicked += OkButton_Clicked;

            mCancelButton = root.Query<Button>("cancelButton").First();
            mCancelButton.clicked += CancelButton_Clicked;
        }

        void OnUseSslToggleChanged(ChangeEvent<bool> evt)
        {
            mUserAssistant.OnSslChanged(mServerTextField.value, evt.newValue);
            mServerTextField.value = mUserAssistant.GetProposedServer();
        }

        void InitializeLayoutAndStyles()
        {
            VisualElement root = rootVisualElement;

            root.LoadLayout(typeof(TeamEditionConfigurationWindow).Name);

            root.LoadStyle(typeof(TeamEditionConfigurationWindow).Name);
        }

        void OnServerTextFieldChanged(ChangeEvent<string> evt)
        {
            mUserAssistant.OnServerChanged(evt.newValue);
            mUseSslToggle.value = mUserAssistant.IsSslServer(evt.newValue);
        }

        void ShowServerNotificationMessage(string message)
        {
            mConnectedLabel.text = message;
            mConnectedLabel.Show();
        }

        void ShowServerValidationError(string message)
        {
            mConnectedLabel.text = message;
            mConnectedLabel.AddToClassList("error");
            mConnectedLabel.Show();
        }

        void ShowCredentialsNotificationMessage(string message)
        {
            mCredentialsLabel.text = message;
            mCredentialsLabel.Show();
        }

        void ShowCheckCredentialsError(string message)
        {
            mCredentialsLabel.text = message;
            mCredentialsLabel.AddToClassList("error");
            mCredentialsLabel.Show();
        }

        void HideValidation()
        {
            mConnectedLabel.RemoveFromClassList("error");
            mConnectedLabel.Hide();
        }

        void ShowProgress(string text)
        {
            mSpinnerLabel.text = text;

            mSpinnerContainer.Show();
            mSpinnerLabel.Show();
            mLoadingSpinner.Start();
        }

        void HideProgress()
        {
            mLoadingSpinner.Stop();
            mSpinnerContainer.Hide();
            mSpinnerLabel.Hide();
        }

        void UpdatePasswordEntries(bool bIsPasswordRequired)
        {
            if (!bIsPasswordRequired)
            {
                mPasswordTextField.Collapse();
                mUserTextField.SetEnabled(false);
                mUserTextField.value = Environment.UserName;
                return;
            }

            mUserTextField.SetEnabled(true);
            mPasswordTextField.Show();
            mUserTextField.SelectAll();
            mUserTextField.FocusWorkaround();
        }

        Button mConnectButton;
        Label mConnectedLabel;
        TextField mServerTextField;
        TextField mPasswordTextField;
        Toggle mUseSslToggle;
        LoadingSpinner mLoadingSpinner;
        Label mSpinnerLabel;
        VisualElement mSpinnerContainer;
        Button mCheckConnectionButton;
        Label mCredentialsLabel;
        Button mOkButton;
        Button mCancelButton;

        SEIDWorkingMode mSEIDWorkingMode = SEIDWorkingMode.UPWorkingMode;

        ConfigurationDialogUserAssistant mUserAssistant =
            new ConfigurationDialogUserAssistant();

        IPlasticWebRestApi mRestApi;
        WelcomeView mWelcomeView;
        TextField mUserTextField;
    }
}