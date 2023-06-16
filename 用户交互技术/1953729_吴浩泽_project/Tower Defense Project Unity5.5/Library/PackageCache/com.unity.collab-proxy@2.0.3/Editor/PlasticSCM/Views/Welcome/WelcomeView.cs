using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using PlasticGui;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.CreateWorkspace;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;
using Codice.Client.BaseCommands;
using Unity.PlasticSCM.Editor.Configuration.TeamEdition;
using Codice.CM.Common;

namespace Unity.PlasticSCM.Editor.Views.Welcome
{
    internal class WelcomeView
    {
        internal WelcomeView(
            PlasticWindow parentWindow,
            CreateWorkspaceView.ICreateWorkspaceListener createWorkspaceListener,
            IPlasticAPI plasticApi,
            IPlasticWebRestApi plasticWebRestApi,
            CmConnection cmConnection)
        {
            mParentWindow = parentWindow;
            mCreateWorkspaceListener = createWorkspaceListener;
            mPlasticApi = plasticApi;
            mPlasticWebRestApi = plasticWebRestApi;
            mCmConnection = cmConnection;

            mGuiMessage = new UnityPlasticGuiMessage();
            mConfigureProgress = new ProgressControlsForViews();

            mInstallerFile = GetInstallerTmpFileName.ForPlatform();
            autoLoginState = AutoLogin.State.Off;
        }

        internal void Update()
        {
            if (mCreateWorkspaceView != null)
                mCreateWorkspaceView.Update();

            mConfigureProgress.UpdateDeterminateProgress(mParentWindow);
        }

        internal void OnGUI(bool clientNeedsConfiguration)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(LEFT_MARGIN);

            DoContentViewArea(
                clientNeedsConfiguration,
                mIsCreateWorkspaceButtonClicked,
                mInstallerFile,
                mGuiMessage,
                mConfigureProgress);

            GUILayout.EndHorizontal();
        }

        internal void OnUserClosedConfigurationWindow()
        {
            ((IProgressControls)mConfigureProgress).HideProgress();

            ClientConfig.Reset();
            CmConnection.Reset();
            ClientHandlers.Register();
        }

        void DoContentViewArea(
            bool clientNeedsConfiguration,
            bool isCreateWorkspaceButtonClicked,
            string installerFile,
            GuiMessage.IGuiMessage guiMessage,
            ProgressControlsForViews configureProgress)
        {
            GUILayout.BeginVertical();

            GUILayout.Space(TOP_MARGIN);

            if (isCreateWorkspaceButtonClicked)
                GetCreateWorkspaceView().OnGUI();
            else
                DoSetupViewArea(
                    clientNeedsConfiguration,
                    mInstallerFile,
                    mGuiMessage,
                    mConfigureProgress);

            GUILayout.EndVertical();
        }

        void DoSetupViewArea(
            bool clientNeedsConfiguration,
            string installerFile,
            GuiMessage.IGuiMessage guiMessage,
            ProgressControlsForViews configureProgress)
        {
            DoTitleLabel();

            GUILayout.Space(STEPS_TOP_MARGIN);

            bool isStep1Completed =
                !clientNeedsConfiguration &&
                !configureProgress.ProgressData.IsOperationRunning;

            DoStepsArea(isStep1Completed, configureProgress.ProgressData);

            GUILayout.Space(BUTTON_MARGIN);

            DoActionButtonsArea(
                isStep1Completed,
                installerFile,
                guiMessage,
                configureProgress);

            DoNotificationArea(configureProgress.ProgressData);
        }

        void DoActionButtonsArea(
            bool isStep1Completed,
            string installerFile,
            GuiMessage.IGuiMessage guiMessage,
            ProgressControlsForViews configureProgress)
        {
            DoActionButton(
                isStep1Completed,
                installerFile,
                guiMessage,
                configureProgress);
        }

        void DoActionButton(
            bool isStep1Completed,
            string installerFile,
            GuiMessage.IGuiMessage guiMessage,
            ProgressControlsForViews configureProgress)
        {
            if (!isStep1Completed)
            {
                DoConfigureButton(configureProgress);
                return;
            }

            if (GUILayout.Button(
                PlasticLocalization.GetString(PlasticLocalization.Name.CreateWorkspace),
                GUILayout.Width(BUTTON_WIDTH)))
                mIsCreateWorkspaceButtonClicked = true;
        }

        void DoConfigureButton(ProgressControlsForViews configureProgress)
        {
            bool isAutoLoginRunning = autoLoginState > AutoLogin.State.Running && autoLoginState <= AutoLogin.State.InitializingPlastic;
            GUI.enabled = !(configureProgress.ProgressData.IsOperationRunning || isAutoLoginRunning);

            if (GUILayout.Button(PlasticLocalization.GetString(
                PlasticLocalization.Name.LoginOrSignUp),
                GUILayout.Width(BUTTON_WIDTH)))
            {
                if (autoLoginState > AutoLogin.State.Off && autoLoginState <= AutoLogin.State.InitializingPlastic)
                {
                    autoLoginState = AutoLogin.State.Running;
                    AutoLogin autoLogin = new AutoLogin();
                    autoLogin.Run();
                    return;
                }

                ((IProgressControls)configureProgress).ShowProgress(string.Empty);

                // Login button defaults to Cloud sign up
                CloudEditionWelcomeWindow.ShowWindow(
                        mPlasticWebRestApi,
                        mCmConnection,
                        this);

                GUIUtility.ExitGUI();
            }

            // If client configuration cannot be determined, keep login button default as Cloud
            // sign in window, but show Enterprise option as well
            GUILayout.FlexibleSpace();

            var anchorStyle = new GUIStyle(GUI.skin.label);
            anchorStyle.normal.textColor = new Color(0.129f, 0.588f, 0.953f);
            anchorStyle.hover.textColor = new Color(0.239f, 0.627f, 0.949f);
            anchorStyle.active.textColor = new Color(0.239f, 0.627f, 0.949f);

            if (GUILayout.Button(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.NeedEnterprise),
                    anchorStyle,
                    GUILayout.Width(BUTTON_WIDTH),
                    GUILayout.Height(20)))
                TeamEditionConfigurationWindow.ShowWindow(mPlasticWebRestApi, this);

            GUILayout.Space(BUTTON_MARGIN);

            GUI.enabled = true;
        }

        static void DoStepsArea(
            bool isStep1Completed,
            ProgressControlsForViews.Data configureProgressData)
        {
            DoLoginOrSignUpStep(isStep1Completed, configureProgressData);

            DoCreatePlasticWorkspaceStep();
        }

        static void DoLoginOrSignUpStep(
            bool isStep1Completed,
            ProgressControlsForViews.Data progressData)
        {
            Texture2D stepImage = (isStep1Completed) ? Images.GetStepOkIcon() : Images.GetStep1Icon();

            string stepText = GetConfigurationStepText(progressData, isStep1Completed);

            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.richText = true;

            GUILayout.BeginHorizontal();

            DoStepLabel(stepText, stepImage, style);

            GUILayout.EndHorizontal();
        }

        static void DoCreatePlasticWorkspaceStep()
        {
            GUILayout.BeginHorizontal();

            DoStepLabel(
                PlasticLocalization.GetString(PlasticLocalization.Name.CreateAUnityVersionControlWorkspace),
                Images.GetStep2Icon(),
                EditorStyles.label);

            GUILayout.EndHorizontal();
        }

        static void DoStepLabel(
            string text,
            Texture2D image,
            GUIStyle style)
        {
            GUILayout.Space(STEPS_LEFT_MARGIN);

            GUIContent stepLabelContent = new GUIContent(
                string.Format(" {0}", text),
                image);

            GUILayout.Label(
                stepLabelContent,
                style,
                GUILayout.Height(STEP_LABEL_HEIGHT));
        }

        static void DoTitleLabel()
        {
            GUIContent labelContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.NextStepsToSetup),
                Images.GetInfoIcon());

            GUILayout.Label(labelContent, EditorStyles.boldLabel);
        }

        static void DoNotificationArea(ProgressControlsForViews.Data configureProgressData)
        {
            if (!string.IsNullOrEmpty(configureProgressData.NotificationMessage))
                DrawProgressForViews.ForNotificationArea(configureProgressData);
        }

        static string GetConfigurationStepText(
            ProgressControlsForViews.Data progressData,
            bool isStep1Completed)
        {
            string result = PlasticLocalization.GetString(
                PlasticLocalization.Name.LoginOrSignUpUnityVersionControl);

            if (isStep1Completed)
                return result;

            if (!progressData.IsOperationRunning)
                return result;

            return string.Format("<b>{0}</b>", result);
        }

        CreateWorkspaceView GetCreateWorkspaceView()
        {
            if (mCreateWorkspaceView != null)
                return mCreateWorkspaceView;

            string workspacePath = ProjectPath.FromApplicationDataPath(
                ApplicationDataPath.Get());

            mCreateWorkspaceView = new CreateWorkspaceView(
                mParentWindow,
                mCreateWorkspaceListener,
                mPlasticApi,
                mPlasticWebRestApi,
                workspacePath);

            return mCreateWorkspaceView;
        }

        internal AutoLogin.State autoLoginState = AutoLogin.State.Off;

        string mInstallerFile;
        bool mIsCreateWorkspaceButtonClicked = false;

        CreateWorkspaceView mCreateWorkspaceView;
        readonly ProgressControlsForViews mConfigureProgress;
        readonly GuiMessage.IGuiMessage mGuiMessage;
        readonly CmConnection mCmConnection;
        readonly IPlasticAPI mPlasticApi;
        readonly IPlasticWebRestApi mPlasticWebRestApi;
        readonly CreateWorkspaceView.ICreateWorkspaceListener mCreateWorkspaceListener;
        readonly PlasticWindow mParentWindow;

        const int LEFT_MARGIN = 30;
        const int TOP_MARGIN = 20;
        const int STEPS_TOP_MARGIN = 5;
        const int STEPS_LEFT_MARGIN = 12;
        const int BUTTON_MARGIN = 10;
        const int STEP_LABEL_HEIGHT = 20;
        const int BUTTON_WIDTH = 170;

        const string DOWNLOAD_URL = @"https://www.plasticscm.com/download";
    }
}