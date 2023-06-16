using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using PlasticGui;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.Configuration.OAuth;
using System.Collections.Generic;
using Codice.Client.Common.Servers;
using Codice.Client.Common;
using Codice.Utils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.Welcome;

using Codice.CM.Common;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal interface IWelcomeWindowNotify
    {
        void SuccessForConfigure(List<string> organizations);
        void Back();
    }

    internal class CloudEditionWelcomeWindow :
        EditorWindow,
        OAuthSignIn.INotify,
        IWelcomeWindowNotify
    {
        internal static void ShowWindow(
            IPlasticWebRestApi restApi,
            CmConnection cmConnection,
            WelcomeView welcomeView,
            bool autoLogin = false)
        {
            sRestApi = restApi;
            sCmConnection = cmConnection;
            sAutoLogin = autoLogin;
            CloudEditionWelcomeWindow window = GetWindow<CloudEditionWelcomeWindow>();

            window.titleContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.SignInToUnityVCS));
            window.minSize = window.maxSize = new Vector2(450, 300);

            window.mWelcomeView = welcomeView;

            window.Show();
        }

        internal static CloudEditionWelcomeWindow GetWelcomeWindow()
        {
            return GetWindow<CloudEditionWelcomeWindow>();
        }

        void OnEnable()
        {
            BuildComponents();
        }

        internal void CancelJoinOrganization()
        {
            if (sAutoLogin)
            {
                GetWindow<PlasticWindow>().GetWelcomeView().autoLoginState = AutoLogin.State.Started;
            }
        }

        internal void JoinOrganizationAndWelcomePage(string organization)
        {
            JoinCloudServer(organization,
                mUserName,
                mAccessToken);

            GetWelcomePage.Run(sRestApi, organization);
        }

        internal static void JoinCloudServer(
            string cloudServer,
            string username,
            string accessToken)
        {
            SaveCloudServer.ToPlasticGuiConfig(cloudServer);
            SaveCloudServer.ToPlasticGuiConfigFile(
                cloudServer, GetPlasticConfigFileToSaveOrganization());
            SaveCloudServer.ToPlasticGuiConfigFile(
                cloudServer, GetGluonConfigFileToSaveOrganization());

            KnownServers.ServersFromCloud.InitializeForWindows(
                PlasticGuiConfig.Get().Configuration.DefaultCloudServer);

            CloudEditionWelcome.WriteToTokensConf(
                cloudServer, username, accessToken);

            SetupUnityEditionToken.CreateCloudEditionTokenIfNeeded();

            if (sAutoLogin)
            {
                ClientConfigData clientConfigData = ConfigurationChecker.GetClientConfigData();
                clientConfigData.WorkspaceServer = cloudServer;
                clientConfigData.WorkingMode = SEIDWorkingMode.SSOWorkingMode.ToString();
                clientConfigData.SecurityConfig = username;
                ClientConfig.Get().Save(clientConfigData);

                GetWindow<PlasticWindow>().GetWelcomeView().autoLoginState = AutoLogin.State.OrganizationChoosed;
            }
        }

        internal void ReplaceRootPanel(VisualElement panel)
        {
            rootVisualElement.Clear();
            rootVisualElement.Add(panel);
        }

        void OnDestroy()
        {
            Dispose();

            if (mWelcomeView != null)
                mWelcomeView.OnUserClosedConfigurationWindow();
        }

        void Dispose()
        {
            if (mSignInPanel != null)
                mSignInPanel.Dispose();

            if (mOrganizationPanel != null)
                mOrganizationPanel.Dispose();
        }

        void OAuthSignIn.INotify.SuccessForConfigure(
            List<string> organizations,
            bool canCreateAnOrganization,
            string userName,
            string accessToken)
        {
            ShowOrganizationPanel(
                GetWindowTitle(),
                organizations);

            Focus();

            mUserName = userName;
            mAccessToken = accessToken;
        }

        internal void ShowOrganizationPanel(
            string title,
            List<string> organizations)
        {
            mOrganizationPanel = new OrganizationPanel(
                this,
                sRestApi,
                title,
                organizations);

            ReplaceRootPanel(mOrganizationPanel);
        }

        void OAuthSignIn.INotify.SuccessForSSO(string organization)
        {
            // empty implementation
        }

        void OAuthSignIn.INotify.SuccessForProfile(string email)
        {
            // empty implementation
        }

        void OAuthSignIn.INotify.SuccessForHomeView(string homeView)
        {
            // empty implementation
        }

        void OAuthSignIn.INotify.SuccessForCredentials(
            string email,
            string accessToken)
        {
            // empty implementation
        }

        void OAuthSignIn.INotify.Cancel(string errorMessage)
        {
            Focus();
        }

        void IWelcomeWindowNotify.SuccessForConfigure(
            List<string> organizations)
        {
            ShowOrganizationPanel(
                GetWindowTitle(),
                organizations);
        }

        internal void FillUserAndToken(
            string userName,
            string accessToken)
        {
            mUserName = userName;
            mAccessToken = accessToken;
        }

        internal void ShowOrganizationPanelFromAutoLogin(
            List<string> organizations)
        {
            ShowOrganizationPanel(
                GetWindowTitle(),
                organizations);
        }

        void IWelcomeWindowNotify.Back()
        {
            rootVisualElement.Clear();
            rootVisualElement.Add(mSignInPanel);
        }

        internal string GetWindowTitle()
        {
            return PlasticLocalization.Name.SignInToUnityVCS.GetString();
        }

        internal static string GetPlasticConfigFileToSaveOrganization()
        {
            if (PlatformIdentifier.IsMac())
            {
                return "macgui.conf";
            }

            return "plasticgui.conf";
        }

        internal static string GetGluonConfigFileToSaveOrganization()
        {
            if (PlatformIdentifier.IsMac())
            {
                return "gluon.conf";
            }

            return "gameui.conf";
        }

        void BuildComponents()
        {
            VisualElement root = rootVisualElement;

            root.Clear();

            mSignInPanel = new SignInPanel(
                this,
                sRestApi,
                sCmConnection);

            titleContent = new GUIContent(GetWindowTitle());

            root.Add(mSignInPanel);
            if (sAutoLogin)
                mSignInPanel.SignInWithUnityIdButtonAutoLogin();
        }

        OrganizationPanel mOrganizationPanel;
        SignInPanel mSignInPanel;
        WelcomeView mWelcomeView;

        string mUserName;
        string mAccessToken;

        static IPlasticWebRestApi sRestApi;
        static CmConnection sCmConnection;
        static bool sAutoLogin = false;
    }
}