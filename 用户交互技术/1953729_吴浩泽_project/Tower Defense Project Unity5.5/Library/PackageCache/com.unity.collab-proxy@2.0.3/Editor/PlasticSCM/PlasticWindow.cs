using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common;
using Codice.Client.Common.FsNodeReaders.Watcher;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using GluonCheckIncomingChanges = PlasticGui.Gluon.WorkspaceWindow.CheckIncomingChanges;
using GluonGui;
using GluonNewIncomingChangesUpdater = PlasticGui.Gluon.WorkspaceWindow.NewIncomingChangesUpdater;
using PlasticAssetModificationProcessor = Unity.PlasticSCM.Editor.AssetUtils.Processor.AssetModificationProcessor;
using PlasticGui;
using PlasticGui.WorkspaceWindow.NotificationBar;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Avatar;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.StatusBar;
using Unity.PlasticSCM.Editor.Views.CreateWorkspace;
using Unity.PlasticSCM.Editor.Views.Welcome;
using Unity.PlasticSCM.Editor.WebApi;

namespace Unity.PlasticSCM.Editor
{
    internal class PlasticWindow : EditorWindow,
        PlasticGui.WorkspaceWindow.CheckIncomingChanges.IAutoRefreshIncomingChangesView,
        GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView,
        CreateWorkspaceView.ICreateWorkspaceListener
    {
        internal WorkspaceWindow WorkspaceWindowForTesting { get { return mWorkspaceWindow; } }
        internal ViewSwitcher ViewSwitcherForTesting { get { return mViewSwitcher; } }
        internal CmConnection CmConnectionForTesting { get { return CmConnection.Get(); } }

        internal bool ShowWelcomeViewForTesting;

        internal void SetupWindowTitle(PlasticNotification.Status status)
        {
            Texture windowIcon = PlasticNotification.GetIcon(status);

            // The titleContent icon does not update unless we also update the title text
            // Temporarily doing it by adding space characters
            string title = UnityConstants.PLASTIC_WINDOW_TITLE;
            title += String.Concat(Enumerable.Repeat(" ", (int)status));

            titleContent = new GUIContent(title, windowIcon);
        }

        internal void DisableCollabIfEnabledWhenLoaded()
        {
            mDisableCollabIfEnabledWhenLoaded = true;
        }

        void PlasticGui.WorkspaceWindow.CheckIncomingChanges.IAutoRefreshIncomingChangesView.IfVisible()
        {
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView.IfVisible()
        {
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void CreateWorkspaceView.ICreateWorkspaceListener.OnWorkspaceCreated(
            WorkspaceInfo wkInfo, bool isGluonMode)
        {
            mWkInfo = wkInfo;
            mIsGluonMode = isGluonMode;
            mWelcomeView = null;

            PlasticPlugin.Enable();

            if (mIsGluonMode)
                ConfigurePartialWorkspace.AsFullyChecked(mWkInfo);

            InitializePlastic();
            Repaint();
        }

        internal void RefreshWorkspaceUI()
        {
            InitializePlastic();
            Repaint();

            OnFocus();
        }

        void OnEnable()
        {
            wantsMouseMove = true;

            if (mException != null)
                return;

            minSize = new Vector2(
                UnityConstants.PLASTIC_WINDOW_MIN_SIZE_WIDTH,
                UnityConstants.PLASTIC_WINDOW_MIN_SIZE_HEIGHT);

            SetupWindowTitle(PlasticNotification.Status.None);

            RegisterApplicationFocusHandlers(this);

            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return;

            PlasticPlugin.Enable();
            InitializePlastic();
        }

        void OnDisable()
        {
            // We need to disable FSWatcher because otherwise it hangs
            // when you move the window between monitors with different scale
            MonoFileSystemWatcher.IsEnabled = false;

            if (mException != null)
                return;

            if (mWkInfo == null)
            {
                ClosePlasticWindow(this);
                return;
            }

            mViewSwitcher.OnDisable();

            ClosePlasticWindow(this);
        }

        void OnDestroy()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            if (!mWorkspaceWindow.IsOperationInProgress())
                return;

            bool bCloseWindow = GuiMessage.ShowQuestion(
                PlasticLocalization.GetString(PlasticLocalization.Name.OperationRunning),
                PlasticLocalization.GetString(PlasticLocalization.Name.ConfirmClosingRunningOperation),
                PlasticLocalization.GetString(PlasticLocalization.Name.YesButton));

            if (bCloseWindow)
                return;

            mForceToOpen = true;
            ShowPlasticWindow(this);
        }

        void OnFocus()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return;

            // We don't want to auto-refresh the views when the window
            // is focused due to a right mouse button click because
            // if there is no internet connection a dialog appears and
            // it prevents being able to open the context menu in order
            // to close the Plastic SCM window
            if (Mouse.IsRightMouseButtonPressed(Event.current))
                return;

            mViewSwitcher.AutoRefreshPendingChangesView();
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void OnGUI()
        {
            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
            {
                DoNotConnectedArea();
                return;
            }

            if (mException != null)
            {
                DoExceptionErrorArea();
                return;
            }

            try
            {
                // IMPORTANT: disable collab (if needed)
                // must be executed before the next if statement
                // where we check if collab is enabled
                if (mDisableCollabIfEnabledWhenLoaded)
                {
                    mDisableCollabIfEnabledWhenLoaded = false;
                    DisableCollabIfEnabled(ProjectPath.FromApplicationDataPath(
                        ApplicationDataPath.Get()));
                }

                if (CollabPlugin.IsEnabled())
                {
                    // execute Close() once after all inspectors update
                    // to avoid our window to be drawn in back color
                    EditorApplication.delayCall = Close;
                    return;
                }

                bool isPlasticExeAvailable = IsExeAvailable.ForMode(mIsGluonMode);
                bool clientNeedsConfiguration = UnityConfigurationChecker.NeedsConfiguration() || ShowWelcomeViewForTesting;

                var welcomeView = GetWelcomeView();

                if (clientNeedsConfiguration && welcomeView.autoLoginState == AutoLogin.State.Off)
                {
                    welcomeView.autoLoginState = AutoLogin.State.Started;
                }

                if (welcomeView.autoLoginState == AutoLogin.State.OrganizationChoosed)
                {
                    OnEnable();
                    welcomeView.autoLoginState = AutoLogin.State.InitializingPlastic;
                }

                if (NeedsToDisplayWelcomeView(clientNeedsConfiguration, mWkInfo))
                {
                    welcomeView.OnGUI(clientNeedsConfiguration);
                    return;
                }

                //TODO: Codice - beta: hide the switcher until the update dialog is implemented
                //DrawGuiModeSwitcher.ForMode(
                //    isGluonMode, plasticClient, changesTreeView, editorWindow);

                DoTabToolbar(
                    isPlasticExeAvailable,
                    mWkInfo,
                    mViewSwitcher,
                    mShowDownloadPlasticExeWindow,
                    mProcessExecutor,
                    mIsGluonMode);

                mViewSwitcher.TabViewGUI();

                if (mWorkspaceWindow.IsOperationInProgress())
                    DrawProgressForOperations.For(
                        mWorkspaceWindow, mWorkspaceWindow.Progress,
                        position.width);

                mStatusBar.OnGUI(
                    mWkInfo,
                    mWorkspaceWindow,
                    mViewSwitcher,
                    mViewSwitcher,
                    mIncomingChangesNotifier,
                    mIsGluonMode);
            }
            catch (Exception ex)
            {
                if (IsExitGUIException(ex))
                    throw;

                GUI.enabled = true;

                if (IsIMGUIPaintException(ex))
                {
                    ExceptionsHandler.LogException("PlasticWindow", ex);
                    return;
                }

                mException = ex;

                DoExceptionErrorArea();

                ExceptionsHandler.HandleException("OnGUI", ex);
            }
        }

        void Update()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            try
            {
                double currentUpdateTime = EditorApplication.timeSinceStartup;
                double elapsedSeconds = currentUpdateTime - mLastUpdateTime;

                mViewSwitcher.Update();
                mWorkspaceWindow.OnParentUpdated(elapsedSeconds);

                if (mWelcomeView != null)
                    mWelcomeView.Update();

                mLastUpdateTime = currentUpdateTime;
            }
            catch (Exception ex)
            {
                mException = ex;

                ExceptionsHandler.HandleException("Update", ex);
            }
        }

        internal void InitializePlastic()
        {
            if (mForceToOpen)
            {
                mForceToOpen = false;
                return;
            }

            try
            {
                if (UnityConfigurationChecker.NeedsConfiguration())
                    return;

                mWkInfo = FindWorkspace.InfoForApplicationPath(
                    ApplicationDataPath.Get(), PlasticGui.Plastic.API);

                if (mWkInfo == null)
                    return;

                PlasticPlugin.EnableForWorkspace();

                SetupCloudProjectIdIfNeeded(mWkInfo, PlasticGui.Plastic.API);

                DisableVCSIfEnabled(mWkInfo.ClientPath);

                mIsGluonMode = PlasticGui.Plastic.API.IsGluonWorkspace(mWkInfo);

                ViewHost viewHost = new ViewHost();

                mStatusBar = new StatusBar();

                mViewSwitcher = new ViewSwitcher(
                    mWkInfo,
                    viewHost,
                    mIsGluonMode,
                    PlasticPlugin.AssetStatusCache,
                    mShowDownloadPlasticExeWindow,
                    mProcessExecutor,
                    PlasticPlugin.WorkspaceOperationsMonitor,
                    mStatusBar,
                    this);

                InitializeNewIncomingChanges(mWkInfo, mIsGluonMode, mViewSwitcher);

                mCooldownAutoRefreshPendingChangesAction = new CooldownWindowDelayer(
                    mViewSwitcher.AutoRefreshPendingChangesView,
                    UnityConstants.AUTO_REFRESH_PENDING_CHANGES_DELAYED_INTERVAL);

                mWorkspaceWindow = new WorkspaceWindow(
                    mWkInfo,
                    viewHost,
                    mViewSwitcher,
                    mViewSwitcher,
                    mDeveloperNewIncomingChangesUpdater,
                    this);

                mViewSwitcher.SetWorkspaceWindow(mWorkspaceWindow);
                mViewSwitcher.ShowInitialView();

                PlasticPlugin.WorkspaceOperationsMonitor.RegisterWindow(
                    mWorkspaceWindow,
                    viewHost,
                    mDeveloperNewIncomingChangesUpdater);

                UnityStyles.Initialize(Repaint);

                AssetMenuItems.BuildOperations(
                    mWkInfo,
                    mWorkspaceWindow,
                    mViewSwitcher,
                    mViewSwitcher,
                    viewHost,
                    mDeveloperNewIncomingChangesUpdater,
                    PlasticPlugin.AssetStatusCache,
                    mViewSwitcher,
                    mViewSwitcher,
                    mShowDownloadPlasticExeWindow,
                    this,
                    mIsGluonMode);

                DrawInspectorOperations.BuildOperations(
                    mWkInfo,
                    mWorkspaceWindow,
                    mViewSwitcher,
                    mViewSwitcher,
                    viewHost,
                    mDeveloperNewIncomingChangesUpdater,
                    PlasticPlugin.AssetStatusCache,
                    mViewSwitcher,
                    mViewSwitcher,
                    mShowDownloadPlasticExeWindow,
                    this,
                    mIsGluonMode);

                mLastUpdateTime = EditorApplication.timeSinceStartup;

                mViewSwitcher.ShowBranchesViewIfNeeded();

                if (!EditionToken.IsCloudEdition())
                    return;

                InitializeNotificationBarUpdater(
                    mWkInfo, mStatusBar.NotificationBar);
            }
            catch (Exception ex)
            {
                mException = ex;

                ExceptionsHandler.HandleException("InitializePlastic", ex);
            }
        }

        void InitializeNewIncomingChanges(
            WorkspaceInfo wkInfo,
            bool bIsGluonMode,
            ViewSwitcher viewSwitcher)
        {
            if (bIsGluonMode)
            {
                Gluon.IncomingChangesNotifier gluonNotifier =
                    new Gluon.IncomingChangesNotifier(this);
                mGluonNewIncomingChangesUpdater =
                    NewIncomingChanges.BuildUpdaterForGluon(
                        wkInfo, viewSwitcher, gluonNotifier, this, gluonNotifier,
                        new GluonCheckIncomingChanges.CalculateIncomingChanges());
                mIncomingChangesNotifier = gluonNotifier;
                return;
            }

            Developer.IncomingChangesNotifier developerNotifier =
                new Developer.IncomingChangesNotifier(this);
            mDeveloperNewIncomingChangesUpdater =
                NewIncomingChanges.BuildUpdaterForDeveloper(
                    wkInfo, viewSwitcher, developerNotifier,
                    this, developerNotifier);
            mIncomingChangesNotifier = developerNotifier;
        }

        void InitializeNotificationBarUpdater(
            WorkspaceInfo wkInfo,
            INotificationBar notificationBar)
        {
            mNotificationBarUpdater = new NotificationBarUpdater(
                notificationBar,
                PlasticGui.Plastic.WebRestAPI,
                new UnityPlasticTimerBuilder(),
                new NotificationBarUpdater.NotificationBarConfig(),
                ScreenResolution.Get());
            mNotificationBarUpdater.Start();
            mNotificationBarUpdater.SetWorkspace(wkInfo);
        }

        void OnApplicationActivated()
        {
            if (mException != null)
                return;

            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return;

            Reload.IfWorkspaceConfigChanged(
                PlasticGui.Plastic.API, mWkInfo, mIsGluonMode,
                ExecuteFullReload);

            if (mWkInfo == null)
                return;

            ((IWorkspaceWindow)mWorkspaceWindow).UpdateTitle();

            NewIncomingChanges.LaunchUpdater(
                mDeveloperNewIncomingChangesUpdater,
                mGluonNewIncomingChangesUpdater);

            // When Unity Editor window is activated it writes some files to its Temp folder.
            // This causes the fswatcher to process those events.
            // We need to wait until the fswatcher finishes processing the events,
            // otherwise the NewChangesInWk method will return TRUE, causing
            // the pending changes view to unwanted auto-refresh.
            // So, we need to delay the auto-refresh call in order
            // to give the fswatcher enough time to process the events.
            // Note that the OnFocus event is not affected by this issue.
            mCooldownAutoRefreshPendingChangesAction.Ping();

            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void OnApplicationDeactivated()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            if (!PlasticPlugin.ConnectionMonitor.IsConnected)
                return;

            NewIncomingChanges.StopUpdater(
                mDeveloperNewIncomingChangesUpdater,
                mGluonNewIncomingChangesUpdater);
        }

        void ExecuteFullReload()
        {
            mException = null;

            DisposeNewIncomingChanges(this);

            DisposeNotificationBarUpdater(this);

            InitializePlastic();
        }

        void DoNotConnectedArea()
        {
            string labelText = PlasticLocalization.GetString(
                PlasticLocalization.Name.NotConnectedTryingToReconnect);

            string buttonText = PlasticLocalization.GetString(
                PlasticLocalization.Name.TryNowButton);

            GUI.enabled = !PlasticPlugin.ConnectionMonitor.IsTryingReconnection;

            DrawActionHelpBox.For(
                Images.GetInfoDialogIcon(), labelText, buttonText,
                PlasticPlugin.ConnectionMonitor.CheckConnection);

            GUI.enabled = true;
        }

        void DoExceptionErrorArea()
        {
            string labelText = PlasticLocalization.GetString(
                PlasticLocalization.Name.UnexpectedError);

            string buttonText = PlasticLocalization.GetString(
                PlasticLocalization.Name.ReloadButton);

            DrawActionHelpBox.For(
                Images.GetErrorDialogIcon(), labelText, buttonText,
                ExecuteFullReload);
        }

        internal WelcomeView GetWelcomeView()
        {
            if (mWelcomeView != null)
                return mWelcomeView;

            mWelcomeView = new WelcomeView(
                this,
                this,
                PlasticGui.Plastic.API,
                PlasticGui.Plastic.WebRestAPI,
                CmConnection.Get());

            return mWelcomeView;
        }

        static void DoSearchField(ViewSwitcher viewSwitcher)
        {
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.PendingChanges))
            {
                viewSwitcher.PendingChangesTab.DrawSearchFieldForPendingChangesTab();
                return;
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Changesets))
            {
                viewSwitcher.ChangesetsTab.DrawSearchFieldForChangesetsTab();
                return;
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.History))
            {
                viewSwitcher.HistoryTab.DrawSearchFieldForHistoryTab();
                return;
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Branches))
            {
                viewSwitcher.BranchesTab.DrawSearchFieldForBranchesTab();
                return;
            }
        }

        static void DoTabToolbar(
            bool isPlasticExeAvailable,
            WorkspaceInfo workspaceInfo,
            ViewSwitcher viewSwitcher,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            LaunchTool.IProcessExecutor processExecutor,
            bool isGluonMode)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            viewSwitcher.TabButtonsGUI();

            GUILayout.FlexibleSpace();

            DoSearchField(viewSwitcher);

            DoLaunchButtons(
                isPlasticExeAvailable,
                workspaceInfo,
                viewSwitcher,
                showDownloadPlasticExeWindow,
                processExecutor,
                isGluonMode);

            EditorGUILayout.EndHorizontal();
        }

        static void OpenBranchListViewAndSendEvent(
            WorkspaceInfo wkInfo,
            ViewSwitcher viewSwitcher)
        {
            viewSwitcher.ShowBranchesView();
            TrackFeatureUseEvent.For(
               PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
               TrackFeatureUseEvent.Features.OpenBranchesView);
        }

        static void ShowBranchesContextMenu(
            WorkspaceInfo wkInfo,
            ViewSwitcher viewSwitcher,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            LaunchTool.IProcessExecutor processExecutor,
            bool isGluonMode)
        {
            GenericMenu menu = new GenericMenu();

            string branchesListView = PlasticLocalization.GetString(
                PlasticLocalization.Name.Branches);

            menu.AddItem(
               new GUIContent(branchesListView),
               false,
               () => OpenBranchListViewAndSendEvent(wkInfo, viewSwitcher));

            string branchExplorer = PlasticLocalization.GetString(
            PlasticLocalization.Name.BranchExplorerMenu);

            menu.AddItem(
              new GUIContent(branchExplorer),
              false,
              () => LaunchTool.OpenBranchExplorer(
                showDownloadPlasticExeWindow,
                processExecutor,
                wkInfo, 
                isGluonMode));

            menu.ShowAsContext();
        }

        static void ShowSettingsContextMenu(
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            LaunchTool.IProcessExecutor processExecutor,
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            GenericMenu menu = new GenericMenu();

            string openToolText = isGluonMode ?
                PlasticLocalization.GetString(PlasticLocalization.Name.OpenInGluon) :
                PlasticLocalization.GetString(PlasticLocalization.Name.OpenInDesktopApp);

            menu.AddItem(
                new GUIContent(openToolText),
                false,
                () => LaunchTool.OpenGUIForMode(
                    showDownloadPlasticExeWindow,
                    processExecutor,
                    wkInfo,
                    isGluonMode));

            if (EditionToken.IsCloudEdition())
            {
                menu.AddItem(new GUIContent(
                    PlasticLocalization.GetString(
                       PlasticLocalization.Name.InviteMembers)),
                false,
               InviteMemberButton_clicked,
               PlasticGui.Plastic.API.GetRepositorySpec(wkInfo));
            }

            menu.AddSeparator("");

            menu.AddItem(
                new GUIContent(
                    PlasticLocalization.GetString(
                PlasticLocalization.Name.Options)),
                false,
                () => SettingsService.OpenProjectSettings(UnityConstants.PROJECT_SETTINGS_TAB_PATH));

            // If the user has the simplified UI key of type .txt in the Assets folder
            // TODO: Remove when Simplified UI is complete
            if (AssetDatabase.FindAssets("simplifieduikey t:textasset", new[] { "Assets" }).Any())
                menu.AddItem(new GUIContent("Try Simplified UI"),
                    false,
                    TrySimplifiedUIButton_Clicked,
                    null);

            //TODO: Localization
            menu.AddItem(
                new GUIContent(PlasticAssetModificationProcessor.ForceCheckout ?
                PlasticLocalization.GetString(PlasticLocalization.Name.DisableForcedCheckout) :
                PlasticLocalization.GetString(PlasticLocalization.Name.EnableForcedCheckout)),
                false,
                ForceCheckout_Clicked,
                null);

            menu.ShowAsContext();
        }

        static void DoLaunchButtons(
            bool isPlasticExeAvailable,
            WorkspaceInfo wkInfo,
            ViewSwitcher viewSwitcher,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            LaunchTool.IProcessExecutor processExecutor,
            bool isGluonMode)
        {
            //TODO: Codice - beta: hide the diff button until the behavior is implemented
            /*GUILayout.Button(PlasticLocalization.GetString(
                PlasticLocalization.Name.DiffWindowMenuItemDiff),
                EditorStyles.toolbarButton,
                GUILayout.Width(UnityConstants.REGULAR_BUTTON_WIDTH));*/

            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Changesets))
            {
                viewSwitcher.ChangesetsTab.DrawDateFilter();
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Branches))
            {
                viewSwitcher.BranchesTab.DrawDateFilter();
            }

            Texture refreshIcon = Images.GetRefreshIcon();
            string refreshIconTooltip = PlasticLocalization.GetString(
                PlasticLocalization.Name.RefreshButton);

            if (DrawLaunchButton(refreshIcon, refreshIconTooltip))
            {
                viewSwitcher.RefreshSelectedView();
            }

            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.PendingChanges))
            {
                Texture2D icon = Images.GetUndoIcon();
                string tooltip = PlasticLocalization.GetString(
                    PlasticLocalization.Name.UndoSelectedChanges);

                if (DrawLaunchButton(icon, tooltip))
                {
                    TrackFeatureUseEvent.For(
                        PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                        TrackFeatureUseEvent.Features.UndoIconButton);

                    viewSwitcher.PendingChangesTab.UndoForMode(wkInfo, isGluonMode);
                }
            }

            if (isGluonMode)
            {
                string label = PlasticLocalization.GetString(PlasticLocalization.Name.ConfigureGluon);
                if (DrawActionButton.For(label))
                    LaunchTool.OpenWorkspaceConfiguration(
                        showDownloadPlasticExeWindow,
                        processExecutor,
                        wkInfo, 
                        isGluonMode);
            }
            else
            {
                Texture2D icon = Images.GetBranchIcon();
                string tooltip = PlasticLocalization.GetString(PlasticLocalization.Name.Branches);
                if (DrawLaunchButton(icon, tooltip))
                {
                    ShowBranchesContextMenu(
                        wkInfo,
                        viewSwitcher,
                        showDownloadPlasticExeWindow,
                        processExecutor,
                        isGluonMode);
                }
            }

            //TODO: Add settings button tooltip localization
            if (DrawLaunchButton(Images.GetSettingsIcon(), string.Empty))
            {
                ShowSettingsContextMenu(
                    showDownloadPlasticExeWindow,
                    processExecutor,
                    wkInfo,
                    isGluonMode);
            }
        }

        static bool DrawLaunchButton(Texture icon, string tooltip)
        {
            return GUILayout.Button(
                new GUIContent(icon, tooltip),
                EditorStyles.toolbarButton, 
                GUILayout.Width(26));
        }

        static void InviteMemberButton_clicked(object obj)
        {
            RepositorySpec repSpec = (RepositorySpec)obj; 

            string organizationName = ServerOrganizationParser.
                GetOrganizationFromServer(repSpec.Server);

            CurrentUserAdminCheckResponse response = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(50);
            waiter.Execute(
                /*threadOperationDelegate*/
                delegate
                {
                    ServerProfile serverProfile = CmConnection.Get().
                        GetProfileManager().GetProfileForServer(repSpec.Server);

                    string authToken = serverProfile != null ?
                        CmConnection.Get().
                            BuildWebApiTokenForCloudEditionForUser(
                                serverProfile.Server, 
                                serverProfile.GetSEIDWorkingMode(), 
                                serverProfile.SecurityConfig):
                        CmConnection.Get().
                            BuildWebApiTokenForCloudEditionForUser(
                                repSpec.Server, 
                                ClientConfig.Get().GetSEIDWorkingMode(), 
                                ClientConfig.Get().GetSecurityConfig());

                    if (string.IsNullOrEmpty(authToken))
                        authToken = CmConnection.Get().
                            BuildWebApiTokenForCloudEditionDefaultUser();

                    if (string.IsNullOrEmpty(authToken))
                    {
                        return;
                    }

                    response = WebRestApiClient.PlasticScm.IsUserAdmin(
                        organizationName,
                        authToken);
                },
                /*afterOperationDelegate*/
                delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.LogException(
                            "IsUserAdmin",
                            waiter.Exception);

                        OpenCloudDashboardUsersGroupsUrl(organizationName);
                        return;
                    }

                    if (response == null)
                    {
                        mLog.DebugFormat(
                            "Error checking if the user is the organization admin for {0}",
                            organizationName);

                        OpenCloudDashboardUsersGroupsUrl(organizationName);
                        return;
                    }

                   if (response.Error != null)
                    {
                        mLog.DebugFormat(
                          "Error checking if the user is the organization admin: {0}",
                          string.Format("Unable to get IsUserAdminResponse: {0} [code {1}]",
                              response.Error.Message,
                              response.Error.ErrorCode));

                        OpenCloudDashboardUsersGroupsUrl(organizationName);
                        return;
                    }
                  
                    if (response.IsCurrentUserAdmin)
                    {
                        OpenCloudDashboardUsersGroupsUrl(response.OrganizationName);
                        return;
        }

                    GuiMessage.ShowInformation(
                        PlasticLocalization.GetString(PlasticLocalization.Name.InviteMembersTitle),
                        PlasticLocalization.GetString(PlasticLocalization.Name.InviteMembersMessage));
                });
        }

        static void OpenCloudDashboardUsersGroupsUrl(string organization)
        {
            Application.OpenURL("https://www.plasticscm.com/dashboard/cloud/" +
                organization +
                "/users-and-groups");
        }

        static void TrySimplifiedUIButton_Clicked(object obj)
        {
            PlasticSCMWindow.ShowWindow();
        }

        static void ForceCheckout_Clicked(object obj)
        {
            PlasticAssetModificationProcessor.SetForceCheckoutOption(
                !PlasticAssetModificationProcessor.ForceCheckout);
        }

        static void SetupCloudProjectIdIfNeeded(
            WorkspaceInfo wkInfo,
            IPlasticAPI plasticApi)
        {
            if (SetupCloudProjectId.HasCloudProjectId())
                return;

            SetupCloudProjectId.ForWorkspace(wkInfo, plasticApi);

            mLog.DebugFormat("Setup CloudProjectId on Project: {0}",
                wkInfo.ClientPath);
        }

        static void DisableVCSIfEnabled(string projectPath)
        {
            if (!VCSPlugin.IsEnabled())
                return;

            VCSPlugin.Disable();

            mLog.DebugFormat("Disabled VCS Plugin on Project: {0}",
                projectPath);
        }

        static void DisposeNewIncomingChanges(PlasticWindow window)
        {
            NewIncomingChanges.DisposeUpdater(
                window.mDeveloperNewIncomingChangesUpdater,
                window.mGluonNewIncomingChangesUpdater);

            window.mDeveloperNewIncomingChangesUpdater = null;
            window.mGluonNewIncomingChangesUpdater = null;
        }

        static void DisposeNotificationBarUpdater(PlasticWindow window)
        {
            if (window.mNotificationBarUpdater == null)
                return;

            window.mNotificationBarUpdater.Dispose();
            window.mNotificationBarUpdater = null;
        }

        static void RegisterApplicationFocusHandlers(PlasticWindow window)
        {
            EditorWindowFocus.OnApplicationActivated += window.OnApplicationActivated;
            EditorWindowFocus.OnApplicationDeactivated += window.OnApplicationDeactivated;
        }

        static void UnRegisterApplicationFocusHandlers(PlasticWindow window)
        {
            EditorWindowFocus.OnApplicationActivated -= window.OnApplicationActivated;
            EditorWindowFocus.OnApplicationDeactivated -= window.OnApplicationDeactivated;
        }

        static bool IsExitGUIException(Exception ex)
        {
            return ex is ExitGUIException;
        }

        static bool IsIMGUIPaintException(Exception ex)
        {
            if (!(ex is ArgumentException))
                return false;

            return ex.Message.StartsWith("Getting control") &&
                   ex.Message.Contains("controls when doing repaint");
        }

        static void ClosePlasticWindow(PlasticWindow window)
        {
            UnRegisterApplicationFocusHandlers(window);

            if (PlasticPlugin.WorkspaceOperationsMonitor != null)
                PlasticPlugin.WorkspaceOperationsMonitor.UnRegisterWindow();

            DisposeNewIncomingChanges(window);

            DisposeNotificationBarUpdater(window);

            AvatarImages.Dispose();
        }

        static void ShowPlasticWindow(PlasticWindow window)
        {
            EditorWindow dockWindow = FindEditorWindow.ToDock<PlasticWindow>();

            PlasticWindow newPlasticWindow = InstantiateFrom(window);

            if (DockEditorWindow.IsAvailable())
                DockEditorWindow.To(dockWindow, newPlasticWindow);

            newPlasticWindow.Show();

            newPlasticWindow.Focus();
        }

        static bool NeedsToDisplayWelcomeView(
            bool clientNeedsConfiguration,
            WorkspaceInfo wkInfo)
        {
            if (clientNeedsConfiguration)
                return true;

            if (wkInfo == null)
                return true;

            return false;
        }

        static void DisableCollabIfEnabled(string projectPath)
        {
            if (!CollabPlugin.IsEnabled())
                return;

            CollabPlugin.Disable();

            mLog.DebugFormat("Disabled Collab Plugin on Project: {0}",
                projectPath);
        }

        static PlasticWindow InstantiateFrom(PlasticWindow window)
        {
            PlasticWindow result = Instantiate(window);
            result.mWkInfo = window.mWkInfo;
            result.mWorkspaceWindow = window.mWorkspaceWindow;
            result.mViewSwitcher = window.mViewSwitcher;
            result.mCooldownAutoRefreshPendingChangesAction = window.mCooldownAutoRefreshPendingChangesAction;
            result.mDeveloperNewIncomingChangesUpdater = window.mDeveloperNewIncomingChangesUpdater;
            result.mGluonNewIncomingChangesUpdater = window.mGluonNewIncomingChangesUpdater;
            result.mException = window.mException;
            result.mLastUpdateTime = window.mLastUpdateTime;
            result.mIsGluonMode = window.mIsGluonMode;
            result.mIncomingChangesNotifier = window.mIncomingChangesNotifier;
            result.mStatusBar = window.mStatusBar;
            result.mWelcomeView = window.mWelcomeView;
            result.mNotificationBarUpdater = window.mNotificationBarUpdater;
            return result;
        }

        internal PlasticProjectSettingsProvider.IAutoRefreshView GetPendingChangesView()
        {
            return mViewSwitcher != null ? mViewSwitcher.PendingChangesTab : null;
        }

        static class Reload
        {
            internal static void IfWorkspaceConfigChanged(
                IPlasticAPI plasticApi,
                WorkspaceInfo lastWkInfo,
                bool lastIsGluonMode,
                Action reloadAction)
            {
                string applicationPath = ApplicationDataPath.Get();

                bool isGluonMode = false;
                WorkspaceInfo wkInfo = null;

                IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                    /*threadOperationDelegate*/ delegate
                    {
                        wkInfo = FindWorkspace.
                            InfoForApplicationPath(applicationPath, plasticApi);

                        if (wkInfo != null)
                            isGluonMode = plasticApi.IsGluonWorkspace(wkInfo);
                    },
                    /*afterOperationDelegate*/ delegate
                    {
                        if (waiter.Exception != null)
                            return;

                        if (!IsWorkspaceConfigChanged(
                                lastWkInfo, wkInfo,
                                lastIsGluonMode, isGluonMode))
                            return;

                        reloadAction();
                    });
            }

            static bool IsWorkspaceConfigChanged(
                WorkspaceInfo lastWkInfo,
                WorkspaceInfo currentWkInfo,
                bool lastIsGluonMode,
                bool currentIsGluonMode)
            {
                if (lastIsGluonMode != currentIsGluonMode)
                    return true;

                if (lastWkInfo == null || currentWkInfo == null)
                    return true;

                return !lastWkInfo.Equals(currentWkInfo);
            }
        }

        [SerializeField]
        bool mForceToOpen;

        [NonSerialized]
        WorkspaceInfo mWkInfo;

        Exception mException;

        internal IIncomingChangesNotifier mIncomingChangesNotifier;

        double mLastUpdateTime = 0f;

        CooldownWindowDelayer mCooldownAutoRefreshPendingChangesAction;
        internal ViewSwitcher mViewSwitcher;
        WelcomeView mWelcomeView;

        StatusBar mStatusBar;
        NotificationBarUpdater mNotificationBarUpdater;

        PlasticGui.WorkspaceWindow.NewIncomingChangesUpdater mDeveloperNewIncomingChangesUpdater;
        GluonNewIncomingChangesUpdater mGluonNewIncomingChangesUpdater;

        WorkspaceWindow mWorkspaceWindow;

        bool mIsGluonMode;
        bool mDisableCollabIfEnabledWhenLoaded;

        LaunchTool.IShowDownloadPlasticExeWindow mShowDownloadPlasticExeWindow =
            new LaunchTool.ShowDownloadPlasticExeWindow();
        LaunchTool.IProcessExecutor mProcessExecutor =
            new LaunchTool.ProcessExecutor();

        static readonly ILog mLog = LogManager.GetLogger("PlasticWindow");
    }
}