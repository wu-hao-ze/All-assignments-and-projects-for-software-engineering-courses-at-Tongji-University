using System;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.BaseCommands.Sync;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using CodiceApp.EventTracking;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.PlasticSCM.Editor.Configuration;

namespace Unity.PlasticSCM.Editor.CollabMigration
{
    internal class MigrationDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 710, 260);
            }
        }

        protected override string GetTitle()
        {
            return "Upgrade your Collaborate project to Unity Version Control";
        }

        //TODO: localize the strings
        protected override void OnModalGUI()
        {
            GUILayout.BeginHorizontal();

            DoIconArea();

            GUILayout.Space(20);

            DoContentArea();

            GUILayout.EndHorizontal();

            DoButtonsArea();

            mProgressControls.UpdateDeterminateProgress(this);
        }

        internal static bool Show(
            EditorWindow parentWindow,
            string unityAccessToken,
            string projectPath,
            string user,
            string organizationName,
            RepId repId,
            long changesetId,
            long branchId,
            Action afterWorkspaceMigratedAction)
        {
            MigrationDialog dialog = Create(
                unityAccessToken,
                projectPath,
                user,
                organizationName,
                repId,
                changesetId,
                branchId,
                afterWorkspaceMigratedAction,
                new ProgressControlsForMigration());

            return dialog.RunModal(parentWindow) == ResponseType.Ok;
        }

        void DoIconArea()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(30);

            Rect iconRect = GUILayoutUtility.GetRect(
                GUIContent.none, EditorStyles.label,
                GUILayout.Width(60), GUILayout.Height(60));

            GUI.DrawTexture(
                iconRect,
                Images.GetPlasticIcon(),
                ScaleMode.ScaleToFit);

            GUILayout.EndVertical();
        }

        void DoContentArea()
        {
            GUILayout.BeginVertical();

            Title("Upgrade your Collaborate project to Unity Version Control");

            GUILayout.Space(20);

            Paragraph("Your Unity project has been upgraded (from Collaborate) to Unity Version Control free" +
                " of charge by your administrator. Your local workspace will now be converted to a" +
                " Unity Version Control workspace in just a few minutes. Select “Migrate” to start the conversion process.");

            DrawProgressForMigration.For(
               mProgressControls.ProgressData);

            GUILayout.Space(40);

            GUILayout.EndVertical();
        }

        //TODO: localize the strings
        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoOkButton();
                    DoCloseButton();
                    return;
                }

                DoCloseButton();
                DoOkButton();
            }
        }

        void DoOkButton()
        {
            if (mIsMigrationCompleted)
            {
                DoOpenPlasticButton();
                return;
            }

            DoMigrateButton();
        }

        void DoCloseButton()
        {
            GUI.enabled = !mProgressControls.ProgressData.IsOperationRunning;

            if (NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CloseButton)))
            {
                if (mIsMigrationCompleted) 
                    TrackFeatureUseEvent.For(
                        PlasticGui.Plastic.API.GetRepositorySpec(mWorkspaceInfo),
                        TrackFeatureUseEvent.Features.CloseDialogAfterWorkspaceMigration);
                else 
                    TrackFeatureUseEvent.For(
                        GetEventCloudOrganizationInfo(),
                        TrackFeatureUseEvent.Features.DoNotMigrateWorkspace);

                CloseButtonAction();
            }

            GUI.enabled = true;
        }

        void DoOpenPlasticButton()
        {
            if (!NormalButton("Open Unity Version Control"))
                return;

            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(mWorkspaceInfo),
                TrackFeatureUseEvent.Features.OpenPlasticAfterWorkspaceMigration);

            ((IPlasticDialogCloser)this).CloseDialog();
            ShowWindow.Plastic();
        }

        EventCloudOrganizationInfo GetEventCloudOrganizationInfo()
        {
            return new EventCloudOrganizationInfo()
            {
                Name = mOrganizationName,
                ServerType = EventCloudOrganizationInfo.GetServerType(true),
                User = mUser
            };
        }

        void DoMigrateButton()
        {
            GUI.enabled = !mProgressControls.ProgressData.IsOperationRunning;

            if (NormalButton("Migrate"))
            {
                if (EditorUtility.DisplayDialog(
                        "Collab migration to Unity Version Control",
                        "Are you sure to start the migration process?",
                        PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                        PlasticLocalization.GetString(PlasticLocalization.Name.NoButton)))
                {
                    TrackFeatureUseEvent.For(
                        GetEventCloudOrganizationInfo(),
                        TrackFeatureUseEvent.Features.MigrateWorkspace);

                    LaunchMigration(
                        mUnityAccessToken, mProjectPath,
                        mOrganizationName, mRepId,
                        mChangesetId, mBranchId,
                        mAfterWorkspaceMigratedAction,
                        mProgressControls);
                }
                else
                {
                    TrackFeatureUseEvent.For(
                        GetEventCloudOrganizationInfo(),
                        TrackFeatureUseEvent.Features.DoNotMigrateWorkspace);
                }
            }

            GUI.enabled = true;
        }

        static void UpdateProgress(string wkPath,
            CreateWorkspaceFromCollab.Progress progress,
            ProgressControlsForMigration progressControls,
            BuildProgressSpeedAndRemainingTime.ProgressData progressData)
        {
            string header = MigrationProgressRender.FixNotificationPath(
                wkPath, progress.CurrentFile);

            string message = MigrationProgressRender.GetProgressString(
                progress,
                progressData,
                DateTime.Now,
                0.05,
                "Calculating...",
                "Converted {0} of {1}bytes ({2} of 1 file){4}",
                "Converted {0} of {1}bytes ({2} of {3} files {4})",
                "remaining");

            float percent = GetProgressBarPercent.ForTransfer(
                progress.ProcessedSize, progress.TotalSize) / 100f;

            progressControls.ShowProgress(header, message, percent);
        }

        void LaunchMigration(
            string unityAccessToken,
            string projectPath,
            string organizationName,
            RepId repId,
            long changesetId,
            long branchId,
            Action afterWorkspaceMigratedAction,
            ProgressControlsForMigration progressControls)
        {
            string serverName = string.Format(
                "{0}@cloud", organizationName);

            TokenExchangeResponse tokenExchangeResponse = null;
            mWorkspaceInfo = null;

            CreateWorkspaceFromCollab.Progress progress = new CreateWorkspaceFromCollab.Progress();

            BuildProgressSpeedAndRemainingTime.ProgressData progressData =
                new BuildProgressSpeedAndRemainingTime.ProgressData(DateTime.Now);

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
            /*threadOperationDelegate*/
            delegate
            {
                tokenExchangeResponse = AutoConfig.PlasticCredentials(
                    unityAccessToken,
                    serverName,
                    projectPath);

                if (tokenExchangeResponse.Error != null)
                {
                    return;
                }

                RepositoryInfo repInfo = new BaseCommandsImpl().
                    GetRepositoryInfo(repId, serverName);

                if (repInfo == null)
                {
                    return;
                }

                repInfo.SetExplicitServer(serverName);

                mWorkspaceInfo = CreateWorkspaceFromCollab.Create(
                    projectPath, repInfo.Name, repInfo,
                    changesetId, branchId,
                    progress);
            },
            /*afterOperationDelegate*/
            delegate
            {
                progressControls.HideProgress();

                if (waiter.Exception != null)
                {
                    DisplayException(progressControls, waiter.Exception);
                    TrackWorkspaceMigrationFinishedFailureEvent(mWorkspaceInfo);
                    return;
                }

                if (tokenExchangeResponse.Error != null)
                {
                    mLog.ErrorFormat(
                        "Unable to get TokenExchangeResponse: {0} [code {1}]",
                        tokenExchangeResponse.Error.Message,
                        tokenExchangeResponse.Error.ErrorCode);
                }

                if (tokenExchangeResponse.Error != null ||
                    mWorkspaceInfo == null)
                {
                    progressControls.ShowError(
                        "Failed to convert your workspace to Unity Version Control");
                    TrackWorkspaceMigrationFinishedFailureEvent(mWorkspaceInfo);
                    return;
                }

                progressControls.ShowSuccess(
                    "Your workspace has been successfully converted to Unity Version Control");

                mIsMigrationCompleted = true;

                TrackFeatureUseEvent.For(
                    PlasticGui.Plastic.API.GetRepositorySpec(mWorkspaceInfo),
                    TrackFeatureUseEvent.Features.WorkspaceMigrationFinishedSuccess);

                afterWorkspaceMigratedAction();
            },
            /*timerTickDelegate*/
            delegate
            {
                UpdateProgress(projectPath, progress, progressControls, progressData);
            });
        }

        void TrackWorkspaceMigrationFinishedFailureEvent(WorkspaceInfo wkInfo)
        {
            if (wkInfo == null)
            {
                TrackFeatureUseEvent.For(
                    GetEventCloudOrganizationInfo(),
                    TrackFeatureUseEvent.Features.WorkspaceMigrationFinishedFailure);
                return;
            }
            
            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                TrackFeatureUseEvent.Features.WorkspaceMigrationFinishedFailure);
        }
                 
        static void DisplayException(
            ProgressControlsForMigration progressControls,
            Exception ex)
        {
            ExceptionsHandler.LogException(
                "MigrationDialog", ex);

            progressControls.ShowError(
                ExceptionsHandler.GetCorrectExceptionMessage(ex));
        }

        static MigrationDialog Create(
            string unityAccessToken,
            string projectPath,
            string user,
            string organizationName,
            RepId repId,
            long changesetId,
            long branchId,
            Action afterWorkspaceMigratedAction,
            ProgressControlsForMigration progressControls)
        {
            var instance = CreateInstance<MigrationDialog>();
            instance.IsResizable = false;
            instance.mUnityAccessToken = unityAccessToken;
            instance.mProjectPath = projectPath;
            instance.mUser = user;
            instance.mOrganizationName = organizationName;
            instance.mRepId = repId;
            instance.mChangesetId = changesetId;
            instance.mBranchId = branchId;
            instance.mAfterWorkspaceMigratedAction = afterWorkspaceMigratedAction;
            instance.mProgressControls = progressControls;
            instance.mEscapeKeyAction = instance.CloseButtonAction;
            return instance;
        }

        bool mIsMigrationCompleted;

        ProgressControlsForMigration mProgressControls;
        Action mAfterWorkspaceMigratedAction;
        long mChangesetId;
        long mBranchId;
        RepId mRepId;
        string mOrganizationName;
        string mUser;
        string mProjectPath;
        string mUnityAccessToken;
        WorkspaceInfo mWorkspaceInfo;

        static readonly ILog mLog = LogManager.GetLogger("MigrationDialog");
    }
}
