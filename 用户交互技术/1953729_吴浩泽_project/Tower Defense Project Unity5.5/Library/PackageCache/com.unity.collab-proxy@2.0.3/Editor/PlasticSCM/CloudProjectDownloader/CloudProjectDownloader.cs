using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using Codice.LogWrapper;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    internal static class CloudProjectDownloader
    {
        internal const string IS_PROJECT_DOWNLOADER_ALREADY_EXECUTED_KEY =
            "PlasticSCM.ProjectDownloader.IsAlreadyExecuted";

        internal const string SHOULD_PROJECT_BE_DOWNLOADED_KEY =
            "PlasticSCM.ProjectDownloader.ShouldProjectBeDownloaded";

        internal static void Initialize()
        {
            EditorApplication.update += RunOnceWhenAccessTokenIsInitialized;
        }

        static void RunOnceWhenAccessTokenIsInitialized()
        {
            if (string.IsNullOrEmpty(CloudProjectSettings.accessToken))
                return;

            EditorApplication.update -= RunOnceWhenAccessTokenIsInitialized;

            Execute(CloudProjectSettings.accessToken);
        }

        static void Execute(string unityAccessToken)
        {
            if (SessionState.GetBool(
                    IS_PROJECT_DOWNLOADER_ALREADY_EXECUTED_KEY, false))
            {
                return;
            }

            DownloadRepository(unityAccessToken);

            SessionState.SetBool(
                IS_PROJECT_DOWNLOADER_ALREADY_EXECUTED_KEY, true);
        }

        internal static void DownloadRepository(string unityAccessToken)
        {
            DownloadRepository(unityAccessToken, Environment.GetCommandLineArgs());
        }

        internal static void DownloadRepository(string unityAccessToken, string[] commandLineArgs)
        {
            Dictionary<string, string> args = CommandLineArguments.Build(commandLineArgs);

            mLog.DebugFormat(
                "Processing Unity arguments: {0}", string.Join(" ", commandLineArgs));
            
            string projectPath = ParseArguments.ProjectPath(args);
            string cloudRepository = ParseArguments.CloudProject(args);
            string cloudOrganization = ParseArguments.CloudOrganization(args);

            if (string.IsNullOrEmpty(projectPath) ||
                string.IsNullOrEmpty(cloudRepository) ||
                string.IsNullOrEmpty(cloudOrganization))
            {
                return;
            }

            SessionState.SetBool(
                SHOULD_PROJECT_BE_DOWNLOADED_KEY, true);

            PlasticApp.InitializeIfNeeded();

            DownloadRepositoryOperation downloadOperation =
                new DownloadRepositoryOperation();

            downloadOperation.DownloadRepositoryToPathIfNeeded(
                cloudRepository,
                cloudOrganization,
                Path.GetFullPath(projectPath),
                unityAccessToken);
        }

        static readonly ILog mLog = LogManager.GetLogger("ProjectDownloader");
    }
}
