using System;
using System.Threading;

using UnityEditor;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui;
using PlasticGui.WebApi;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Update;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.PlasticSCM.Editor.Configuration;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    internal class DownloadRepositoryOperation
    {
        internal void DownloadRepositoryToPathIfNeeded(
            string cloudRepository,
            string cloudOrganization,
            string projectPath,
            string unityAccessToken)
        {
            RefreshAsset.BeforeLongAssetOperation();

            try
            {
                BuildProgressSpeedAndRemainingTime.ProgressData progressData =
                    new BuildProgressSpeedAndRemainingTime.ProgressData(DateTime.Now);

                ThreadPool.QueueUserWorkItem(
                    DownloadRepository,
                    new DownloadRepositoryParameters()
                    {
                        CloudOrganization = cloudOrganization,
                        CloudRepository = cloudRepository,
                        ProjectPath = projectPath,
                        AccessToken = unityAccessToken
                    });

                while (!mOperationFinished)
                {
                    if (mDisplayProgress)
                    {
                        DisplayProgress(
                            mUpdateNotifier.GetUpdateStatus(),
                            progressData,
                            cloudRepository);
                    }

                    Thread.Sleep(150);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();

                RefreshAsset.AfterLongAssetOperation();

                if (!mOperationFailed)
                {
                    PlasticPlugin.Enable();
                    ShowWindow.PlasticAfterDownloadingProject();
                }
            }
        }

        void DownloadRepository(object state)
        {
            DownloadRepositoryParameters parameters = (DownloadRepositoryParameters)state;

            try
            {
                if (FindWorkspace.HasWorkspace(parameters.ProjectPath))
                {
                    // each domain reload, the package is reloaded.
                    // way need to check if we already downloaded it
                    return;
                }

                mDisplayProgress = true;

                IPlasticWebRestApi restApi = new PlasticWebRestApi();
                string defaultCloudAlias = restApi.GetDefaultCloudAlias();

                RepositorySpec repSpec = BuildRepSpec(
                    parameters.CloudRepository,
                    parameters.CloudOrganization,
                    defaultCloudAlias);

                TokenExchangeResponse tokenExchangeResponse =
                    AutoConfig.PlasticCredentials(
                        parameters.AccessToken,
                        repSpec.Server,
                        parameters.ProjectPath);

                if (tokenExchangeResponse.Error != null)
                {
                    mOperationFailed = true;

                    UnityEngine.Debug.LogErrorFormat(
                        PlasticLocalization.GetString(PlasticLocalization.Name.ErrorDownloadingCloudProject),
                        string.Format("Unable to get TokenExchangeResponse: {0} [code {1}]",
                            tokenExchangeResponse.Error.Message,
                            tokenExchangeResponse.Error.ErrorCode));
                    return;
                }

                WorkspaceInfo wkInfo = CreateWorkspace(
                    repSpec, parameters.ProjectPath);

                mLog.DebugFormat("Created workspace {0} on {1}",
                    wkInfo.Name,
                    wkInfo.ClientPath);
               
                PlasticGui.Plastic.API.Update(
                    wkInfo.ClientPath,
                    UpdateFlags.None,
                    null,
                    mUpdateNotifier);
            }
            catch (Exception ex)
            {
                LogException(ex);

                UnityEngine.Debug.LogErrorFormat(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ErrorDownloadingCloudProject),
                    ex.Message);

                mOperationFailed = true;
            }
            finally
            {
                mOperationFinished = true;
            }
        }

        static void DisplayProgress(
            UpdateOperationStatus status,
            BuildProgressSpeedAndRemainingTime.ProgressData progressData,
            string cloudRepository)
        {
            string totalProgressMessage = UpdateProgressRender.
                GetProgressString(status, progressData);

            float totalProgressPercent = GetProgressBarPercent.
                ForTransfer(status.UpdatedSize, status.TotalSize) / 100f;

            EditorUtility.DisplayProgressBar(
                string.Format("{0} {1}",
                    PlasticLocalization.GetString(PlasticLocalization.Name.DownloadingProgress),
                    cloudRepository),
                totalProgressMessage, totalProgressPercent);
        }

        static WorkspaceInfo CreateWorkspace(
            RepositorySpec repositorySpec,
            string projectPath)
        {
            CreateWorkspaceDialogUserAssistant assistant = new CreateWorkspaceDialogUserAssistant(
                PlasticGuiConfig.Get().Configuration.DefaultWorkspaceRoot,
                PlasticGui.Plastic.API.GetAllWorkspacesArray());

            assistant.RepositoryChanged(
                repositorySpec.ToString(),
                string.Empty,
                string.Empty);

            return PlasticGui.Plastic.API.CreateWorkspace(
                projectPath,
                assistant.GetProposedWorkspaceName(),
                repositorySpec.ToString());
        }

        static RepositorySpec BuildRepSpec(
            string cloudRepository,
            string cloudOrganization,
            string defaultCloudAlias)
        {
            return new RepositorySpec()
            {
                Name = cloudRepository,
                Server = CloudServer.BuildFullyQualifiedName(
                    cloudOrganization, defaultCloudAlias)
            };
        }

        static void LogException(Exception ex)
        {
            mLog.WarnFormat("Message: {0}", ex.Message);

            mLog.DebugFormat(
                "StackTrace:{0}{1}",
                Environment.NewLine, ex.StackTrace);
        }

        class DownloadRepositoryParameters
        {
            internal string CloudRepository;
            internal string CloudOrganization;
            internal string ProjectPath;
            internal string AccessToken;
        }

        volatile bool mOperationFinished = false;
        volatile bool mOperationFailed = false;
        volatile bool mDisplayProgress;

        UpdateNotifier mUpdateNotifier = new UpdateNotifier();

        static readonly ILog mLog = LogManager.GetLogger("DownloadRepositoryOperation");
    }
}
