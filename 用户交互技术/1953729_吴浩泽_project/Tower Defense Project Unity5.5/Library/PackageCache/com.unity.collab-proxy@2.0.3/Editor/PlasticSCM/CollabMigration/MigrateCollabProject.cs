using System;
using System.IO;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.CM.WorkspaceServer;
using Codice.LogWrapper;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.PlasticSCM.Editor.ProjectDownloader;

namespace Unity.PlasticSCM.Editor.CollabMigration
{
    public static class MigrateCollabProject 
    {
        internal static void Initialize()
        {
            if (SessionState.GetInt(
                    IS_PROJECT_MIGRATED_ALREADY_CALCULATED_KEY,
                    MIGRATED_NOT_CALCULATED) == MIGRATED_NOTHING_TO_DO)
                return;

            EditorApplication.update += RunOnceWhenAccessTokenAndProjectIdAreInitialized;
        }

        internal static void RunOnceWhenAccessTokenAndProjectIdAreInitialized()
        {
            if (string.IsNullOrEmpty(CloudProjectSettings.accessToken))
                return;

            if (!SetupCloudProjectId.HasCloudProjectId())
                return;

            if (!SessionState.GetBool(
                   CloudProjectDownloader.IS_PROJECT_DOWNLOADER_ALREADY_EXECUTED_KEY, false))
                return;

            EditorApplication.update -= RunOnceWhenAccessTokenAndProjectIdAreInitialized;

            string projectPath = ProjectPath.FromApplicationDataPath(
                ApplicationDataPath.Get());

            string projectGuid = SetupCloudProjectId.GetCloudProjectId();

            if (!ShouldProjectBeMigrated(projectPath, projectGuid))
            {
                SessionState.SetInt(
                    IS_PROJECT_MIGRATED_ALREADY_CALCULATED_KEY,
                    MIGRATED_NOTHING_TO_DO);
                return;
            }

            Execute(
                CloudProjectSettings.accessToken,
                projectPath,
                projectGuid);
        }

        static bool ShouldProjectBeMigrated(
            string projectPath,
            string projectGuid)
        {
            if (SessionState.GetBool(
                    CloudProjectDownloader.SHOULD_PROJECT_BE_DOWNLOADED_KEY, false))
            {
                return false;
            }

            string collabPath = GetCollabSnapshotFile(
                projectPath, projectGuid);

            if (!File.Exists(collabPath))
            {
                return false;
            }

            if (FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
            {
                return false;
            }
 
            return true;
        }

        static void Execute(
            string unityAccessToken,
            string projectPath,
            string projectGuid)
        {
            string headCommitSha = GetCollabHeadCommitSha(projectPath, projectGuid);

            if (string.IsNullOrEmpty(headCommitSha))
                return;

            PlasticApp.InitializeIfNeeded();

            LaunchMigrationIfProjectIsArchivedAndMigrated(
                unityAccessToken,
                projectPath,
                projectGuid,
                headCommitSha);
        }

        internal static void DeletePlasticDirectoryIfExists(string projectPath)
        {
            WorkspaceInfo wkInfo = new WorkspaceInfo("wk", projectPath);
            string plasticDirectory = WorkspaceConfigFile.GetPlasticWkConfigPath(wkInfo);

            if (!Directory.Exists(plasticDirectory))
                return;

            Directory.Delete(plasticDirectory, true);
        }

        static void LaunchMigrationIfProjectIsArchivedAndMigrated(
            string unityAccessToken,
            string projectPath,
            string projectGuid,
            string headCommitSha)
        {
            IsCollabProjectMigratedResponse isMigratedResponse = null;
            ChangesetFromCollabCommitResponse changesetResponse = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
            /*threadOperationDelegate*/ delegate
            {
                isMigratedResponse = WebRestApiClient.PlasticScm.
                    IsCollabProjectMigrated(unityAccessToken, projectGuid);

                if (isMigratedResponse.Error != null)
                    return;

                if (!isMigratedResponse.IsMigrated)
                    return;

                OrganizationCredentials credentials = new OrganizationCredentials();
                credentials.User = isMigratedResponse.Credentials.Email;
                credentials.Password = isMigratedResponse.Credentials.Token;

                string webLoginAccessToken = WebRestApiClient.CloudServer.WebLogin(
                    isMigratedResponse.WebServerUri,
                    isMigratedResponse.PlasticCloudOrganizationName,
                    credentials);

                changesetResponse = WebRestApiClient.CloudServer.
                    GetChangesetFromCollabCommit(
                        isMigratedResponse.WebServerUri,
                        isMigratedResponse.PlasticCloudOrganizationName,
                        webLoginAccessToken, projectGuid, headCommitSha);
            },
            /*afterOperationDelegate*/ delegate
            {
                if (waiter.Exception != null)
                {
                    ExceptionsHandler.LogException(
                        "IsCollabProjectArchivedAndMigrated",
                        waiter.Exception);
                    return;
                }

                if (isMigratedResponse.Error != null)
                {
                    mLog.ErrorFormat(
                        "Unable to get IsCollabProjectMigratedResponse: {0} [code {1}]",
                        isMigratedResponse.Error.Message,
                        isMigratedResponse.Error.ErrorCode);
                    return;
                }

                if (!isMigratedResponse.IsMigrated)
                {
                    SessionState.SetInt(
                        IS_PROJECT_MIGRATED_ALREADY_CALCULATED_KEY,
                        MIGRATED_NOTHING_TO_DO);
                    return;
                }

                if (changesetResponse.Error != null)
                {
                    mLog.ErrorFormat(
                        "Unable to get ChangesetFromCollabCommitResponse: {0} [code {1}]",
                        changesetResponse.Error.Message,
                        changesetResponse.Error.ErrorCode);
                    return;
                }

                DeletePlasticDirectoryIfExists(projectPath);

                MigrationDialog.Show(
                    null,
                    unityAccessToken,
                    projectPath,
                    isMigratedResponse.Credentials.Email,
                    isMigratedResponse.PlasticCloudOrganizationName,
                    new RepId(
                        changesetResponse.RepId,
                        changesetResponse.RepModuleId),
                    changesetResponse.ChangesetId,
                    changesetResponse.BranchId,
                    AfterWorkspaceMigrated);
            });
        }

        static void AfterWorkspaceMigrated()
        {
            SessionState.SetInt(
                IS_PROJECT_MIGRATED_ALREADY_CALCULATED_KEY,
                MIGRATED_NOTHING_TO_DO);

            CollabPlugin.Disable();

            mLog.DebugFormat(
                "Disabled Collab Plugin after the migration for Project: {0}",
                ProjectPath.FromApplicationDataPath(ApplicationDataPath.Get()));
        }

        static string GetCollabHeadCommitSha(
            string projectPath,
            string projectGuid)
        {
            string collabPath = GetCollabSnapshotFile(
                projectPath, projectGuid);

            if (!File.Exists(collabPath))
                return null;

            string text = File.ReadAllText(collabPath);

            string[] chunks = text.Split(
                new string[] { "currRevisionID" },
                StringSplitOptions.None);

            string current = chunks[1].Substring(3, 40);

            if (!current.Contains("none"))
                return current;

            chunks = text.Split(
                new string[] { "headRevisionID" },
                StringSplitOptions.None);

            return chunks[1].Substring(3, 40);
        }

        static string GetCollabSnapshotFile(
            string projectPath,
            string projectGuid)
        {
            return projectPath + "/Library/Collab/CollabSnapshot_" + projectGuid + ".txt";
        }

        const string IS_PROJECT_MIGRATED_ALREADY_CALCULATED_KEY =
           "PlasticSCM.MigrateCollabProject.IsAlreadyCalculated";

        const int MIGRATED_NOT_CALCULATED = 0;
        const int MIGRATED_NOTHING_TO_DO = 1;

        static readonly ILog mLog = LogManager.GetLogger("MigrateCollabProject");
    }
}