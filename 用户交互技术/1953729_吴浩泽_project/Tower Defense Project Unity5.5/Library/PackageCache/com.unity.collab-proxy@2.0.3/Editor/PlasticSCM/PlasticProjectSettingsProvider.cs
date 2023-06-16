using System;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Commands;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.Utils;
using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    class PlasticProjectSettingsProvider : SettingsProvider
    {
        public PlasticProjectSettingsProvider(
            string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope)
        {
            label = UnityConstants.PROJECT_SETTINGS_TAB_TITLE;
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (CollabPlugin.IsEnabled())
                return null;

            if (!FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
                return null;

            PlasticApp.InitializeIfNeeded();

            return new PlasticProjectSettingsProvider(
                UnityConstants.PROJECT_SETTINGS_TAB_PATH, SettingsScope.Project)
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
        }

        public override void OnActivate(
            string searchContext,
            VisualElement rootElement)
        {
            IAutoRefreshView autoRefreshView = GetPendingChangesView();

            if (autoRefreshView != null)
                autoRefreshView.DisableAutoRefresh();

            mIsPluginEnabled = PlasticPluginIsEnabledPreference.IsEnabled();

            mWkInfo = FindWorkspace.InfoForApplicationPath(
                ApplicationDataPath.Get(), PlasticGui.Plastic.API);

            CheckFsWatcher(mWkInfo);

            mInitialOptions = new PendingChangesOptions();
            mInitialOptions.LoadPendingChangesOptions();

            SetOptions(mInitialOptions);
        }

        public override void OnDeactivate()
        {
            if (mInitialOptions == null)
                return;

            bool isDialogueDirty = false;
            try
            {
                PendingChangesOptions currentOptions = GetOptions();
                isDialogueDirty = IsDirty(currentOptions);
                if (!isDialogueDirty)
                    return;

                currentOptions.SavePreferences();
            }
            finally
            {
                IAutoRefreshView autoRefreshView = GetPendingChangesView();

                if (autoRefreshView != null)
                {
                    autoRefreshView.EnableAutoRefresh();

                    if (isDialogueDirty)
                        autoRefreshView.ForceRefresh();
                }
            }
        }

        public override void OnGUI(string searchContext)
        {
            DrawSettingsSection(
                DoIsEnabledSetting);

            if (!mIsPluginEnabled)
                return;

            DrawSplitter.ForWidth(UnityConstants.SETTINGS_GUI_WIDTH);

            DrawSettingsSection(
                DoPendingChangesSettings);
        }

        void DoIsEnabledSetting()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string message = PlasticLocalization.GetString(
                    mIsPluginEnabled ?
                        PlasticLocalization.Name.UnityVCSIsEnabled :
                        PlasticLocalization.Name.UnityVCSIsDisabled);

                GUILayout.Label(
                    message,
                    EditorStyles.boldLabel,
                    GUILayout.Height(20));

                EditorGUILayout.Space(8);

                DoIsEnabledButton();

                GUILayout.FlexibleSpace();
            }
        }

        void DoIsEnabledButton()
        {
            if (!GUILayout.Button(PlasticLocalization.GetString(
                    mIsPluginEnabled ?
                        PlasticLocalization.Name.DisableButton :
                        PlasticLocalization.Name.EnableButton),
                    UnityStyles.ProjectSettings.ToggleOn))
            {
                return;
            }

            if (!mIsPluginEnabled)
            {
                mIsPluginEnabled = true;

                TrackFeatureUseEvent.For(
                    PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                    TrackFeatureUseEvent.Features.UnityPackage.EnableManually);

                PlasticPlugin.Enable();
                PlasticPluginIsEnabledPreference.Enable();

                return;
            }

            if (mIsPluginEnabled)
            {
                mIsPluginEnabled = false;

                TrackFeatureUseEvent.For(
                    PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                    TrackFeatureUseEvent.Features.UnityPackage.DisableManually);

                PlasticPlugin.ConnectionMonitor.Stop();
                PlasticPluginIsEnabledPreference.Disable();
                CloseWindowIfOpened.Plastic();
                PlasticPlugin.Disable();
                return;
            }
        }

        void DoPendingChangesSettings()
        {
            DoGeneralSettings();

            DoFileChangeSettings();

            DoFileVisibililySettings();

            DoFileDetectionSetings();

            EditorGUILayout.Space(10);

            DoFsWatcherMessage(mFSWatcherEnabled);
        }

        void DoGeneralSettings()
        {
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsGeneral),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowCheckouts = EditorGUILayout.Toggle(Styles.ShowCheckouts, mShowCheckouts);
            mAutoRefresh = EditorGUILayout.Toggle(Styles.AutoRefresh, mAutoRefresh);
        }

        void DoFileChangeSettings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsFileChange),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowChangedFiles = EditorGUILayout.Toggle(Styles.ShowChangedFiles, mShowChangedFiles);
            mCheckFileContent = EditorGUILayout.Toggle(Styles.CheckFileContent, mCheckFileContent);
        }

        void DoFileVisibililySettings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsFileVisibility),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mUseChangeLists = EditorGUILayout.Toggle(Styles.UseChangeLists, mUseChangeLists);
            mShowPrivateFields = EditorGUILayout.Toggle(Styles.ShowPrivateFields, mShowPrivateFields);
            mShowIgnoredFiles = EditorGUILayout.Toggle(Styles.ShowIgnoredFields, mShowIgnoredFiles);
            mShowHiddenFiles = EditorGUILayout.Toggle(Styles.ShowHiddenFields, mShowHiddenFiles);
            mShowDeletedFiles = EditorGUILayout.Toggle(Styles.ShowDeletedFilesDirs, mShowDeletedFiles);
        }

        void DoFileDetectionSetings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsMoveAndRename),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowMovedFiles = EditorGUILayout.Toggle(Styles.ShowMovedFiles, mShowMovedFiles);
            mMatchBinarySameExtension = EditorGUILayout.Toggle(Styles.MatchBinarySameExtension, mMatchBinarySameExtension);
            mMatchTextSameExtension = EditorGUILayout.Toggle(Styles.MatchTextSameExtension, mMatchTextSameExtension);
            mSimilarityPercent = EditorGUILayout.IntSlider(Styles.SimilarityPercent, mSimilarityPercent, 0, 100);
        }

        void DoFsWatcherMessage(bool isEnabled)
        {
            GUIContent message = new GUIContent(
                isEnabled ?
                    GetFsWatcherEnabledMessage() :
                    GetFsWatcherDisabledMessage(),
                isEnabled ?
                    Images.GetInfoIcon() :
                    Images.GetWarnIcon());

            GUILayout.Label(message, UnityStyles.Dialog.Toggle, GUILayout.Height(32));
            GUILayout.Space(-10);

            string formattedExplanation = isEnabled ?
                GetFsWatcherEnabledExplanation() :
                GetFsWatcherDisabledExplanation();

            string helpLink = GetHelpLink();

            DrawTextBlockWithEndLink.For(
                helpLink, formattedExplanation, UnityStyles.Paragraph);
        }

        void CheckFsWatcher(WorkspaceInfo wkInfo)
        {
            bool isFileSystemWatcherEnabled = false;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                    /*threadOperationDelegate*/
                    delegate
                    {
                        isFileSystemWatcherEnabled =
                            IsFileSystemWatcherEnabled(wkInfo);
                    },
                    /*afterOperationDelegate*/
                    delegate
                    {
                        if (waiter.Exception != null)
                            return;

                        mFSWatcherEnabled = isFileSystemWatcherEnabled;
                    });
        }

        void SetOptions(PendingChangesOptions options)
        {
            mShowCheckouts = IsEnabled(
                WorkspaceStatusOptions.FindCheckouts, options.WorkspaceStatusOptions);
            mAutoRefresh = options.AutoRefresh;

            mShowChangedFiles = IsEnabled(
                WorkspaceStatusOptions.FindChanged, options.WorkspaceStatusOptions);
            mCheckFileContent = options.CheckFileContentForChanged;
            
            mUseChangeLists = options.UseChangeLists;

            mShowPrivateFields = IsEnabled(
                WorkspaceStatusOptions.FindPrivates, options.WorkspaceStatusOptions);
            mShowIgnoredFiles = IsEnabled(
                WorkspaceStatusOptions.ShowIgnored, options.WorkspaceStatusOptions);
            mShowHiddenFiles = IsEnabled(
                WorkspaceStatusOptions.ShowHiddenChanges, options.WorkspaceStatusOptions);
            mShowDeletedFiles = IsEnabled(
                WorkspaceStatusOptions.FindLocallyDeleted, options.WorkspaceStatusOptions);

            mShowMovedFiles = IsEnabled(
                WorkspaceStatusOptions.CalculateLocalMoves, options.WorkspaceStatusOptions);
            mMatchBinarySameExtension =
                options.MovedMatchingOptions.bBinMatchingOnlySameExtension;
            mMatchTextSameExtension =
                options.MovedMatchingOptions.bTxtMatchingOnlySameExtension;
            mSimilarityPercent = (int)((1 - options.MovedMatchingOptions.AllowedChangesPerUnit) * 100f);
        }

        PendingChangesOptions GetOptions()
        {
            WorkspaceStatusOptions resultWkStatusOptions =
                WorkspaceStatusOptions.None;

            if (mShowCheckouts)
            {
                resultWkStatusOptions |= WorkspaceStatusOptions.FindCheckouts;
                resultWkStatusOptions |= WorkspaceStatusOptions.FindReplaced;
                resultWkStatusOptions |= WorkspaceStatusOptions.FindCopied;
            }

            if (mShowChangedFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.FindChanged;
            if (mShowPrivateFields)
                resultWkStatusOptions |= WorkspaceStatusOptions.FindPrivates;
            if (mShowIgnoredFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.ShowIgnored;
            if (mShowHiddenFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.ShowHiddenChanges;
            if (mShowDeletedFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.FindLocallyDeleted;
            if (mShowMovedFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.CalculateLocalMoves;

            MovedMatchingOptions matchingOptions = new MovedMatchingOptions();
            matchingOptions.AllowedChangesPerUnit =
                (100 - mSimilarityPercent) / 100f;
            matchingOptions.bBinMatchingOnlySameExtension =
                mMatchBinarySameExtension;
            matchingOptions.bTxtMatchingOnlySameExtension =
                mMatchTextSameExtension;

            return new PendingChangesOptions(
                resultWkStatusOptions,
                matchingOptions,
                mUseChangeLists,
                mAutoRefresh,
                false,
                mCheckFileContent,
                false);
        }

        bool IsDirty(PendingChangesOptions currentOptions)
        {
            return !mInitialOptions.AreSameOptions(currentOptions);
        }

        static void DrawSettingsSection(Action drawSettings)
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;

            try
            {
                EditorGUIUtility.labelWidth = UnityConstants.SETTINGS_GUI_WIDTH;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);

                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Space(10);

                        drawSettings();

                        GUILayout.Space(10);
                    }

                    GUILayout.Space(10);
                }
            }
            finally
            {
                EditorGUIUtility.labelWidth = originalLabelWidth;
            }
        }

        static IAutoRefreshView GetPendingChangesView()
        {
            if (!EditorWindow.HasOpenInstances<PlasticWindow>())
                return null;

            PlasticWindow window = EditorWindow.
                GetWindow<PlasticWindow>(null, false);

            return window.GetPendingChangesView();
        }

        static string GetFsWatcherEnabledMessage()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherEnabled);

            return PlasticLocalization.GetString(
                PlasticLocalization.Name.PendingChangesINotifyEnabled);
        }

        static string GetFsWatcherDisabledMessage()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherDisabled);

            return PlasticLocalization.GetString(
                PlasticLocalization.Name.PendingChangesINotifyDisabled);
        }

        static string GetFsWatcherEnabledExplanation()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherEnabledExplanationUnityVCS);

            return PlasticLocalization.GetString(
            PlasticLocalization.Name.PendingChangesINotifyEnabledExplanation);
        }

        static string GetFsWatcherDisabledExplanation()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
            {
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherDisabledExplanationUnityVCS)
                    .Replace("[[HELP_URL|{0}]]", "{0}");
            }

            return PlasticLocalization.GetString(
                PlasticLocalization.Name.PendingChangesINotifyDisabledExplanation);
        }

        static string GetHelpLink()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return FS_WATCHER_HELP_URL;

            return INOTIFY_HELP_URL;
        }

        static bool IsFileSystemWatcherEnabled(
            WorkspaceInfo wkInfo)
        {
            return WorkspaceWatcherFsNodeReadersCache.Get().
                IsWatcherEnabled(wkInfo);
        }

        static bool IsEnabled(
            WorkspaceStatusOptions option,
            WorkspaceStatusOptions options)
        {
            return (options & option) == option;
        }

        internal interface IAutoRefreshView
        {
            void DisableAutoRefresh();
            void EnableAutoRefresh();
            void ForceRefresh();
        }

        class Styles
        {
            internal static GUIContent ShowCheckouts =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowCheckouts),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowCheckoutsExplanation));
            internal static GUIContent AutoRefresh =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesAutoRefresh),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesAutoRefreshExplanation));
            internal static GUIContent ShowChangedFiles =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindChanged),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindChangedExplanation));
            internal static GUIContent CheckFileContent =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesCheckFileContent),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesCheckFileContentExplanation));
            internal static GUIContent UseChangeLists =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesGroupInChangeLists), 
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesGroupInChangeListsExplanation));
            internal static GUIContent ShowPrivateFields =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowPrivateFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowPrivateFilesExplanation));
            internal static GUIContent ShowIgnoredFields =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowIgnoredFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowIgnoredFilesExplanation));
            internal static GUIContent ShowHiddenFields =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowHiddenFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowHiddenFilesExplanation));
            internal static GUIContent ShowDeletedFilesDirs =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowDeletedFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowDeletedFilesExplanation));
            internal static GUIContent ShowMovedFiles =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindMovedFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindMovedFilesExplanation));
            internal static GUIContent MatchBinarySameExtension =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchBinarySameExtension),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchBinarySameExtensionExplanation));
            internal static GUIContent MatchTextSameExtension =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchTextSameExtension),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchTextSameExtensionExplanation));
            internal static GUIContent SimilarityPercent =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesSimilarityPercentage),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesSimilarityPercentageExplanation));
        }

        bool mIsPluginEnabled;

        WorkspaceInfo mWkInfo;
        PendingChangesOptions mInitialOptions;

        bool mShowCheckouts;
        bool mAutoRefresh;
        bool mFSWatcherEnabled;

        bool mShowChangedFiles;
        bool mCheckFileContent;

        bool mUseChangeLists;
        bool mShowPrivateFields;
        bool mShowIgnoredFiles;
        bool mShowHiddenFiles;
        bool mShowDeletedFiles;

        bool mShowMovedFiles;
        bool mMatchBinarySameExtension;
        bool mMatchTextSameExtension;
        int mSimilarityPercent;

        const string FS_WATCHER_HELP_URL = "https://plasticscm.com/download/help/support";
        const string INOTIFY_HELP_URL = "https://plasticscm.com/download/help/inotify";
    }
}
