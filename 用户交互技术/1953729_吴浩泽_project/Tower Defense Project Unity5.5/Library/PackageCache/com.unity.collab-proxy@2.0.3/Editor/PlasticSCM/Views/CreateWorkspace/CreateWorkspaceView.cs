using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.Client.Commands.CheckIn;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.Client.GameUI.Checkin;
using PlasticGui;
using PlasticGui.Configuration.CloudEdition;
using PlasticGui.Help.Conditions;
using PlasticGui.WorkspaceWindow.Home.Repositories;
using PlasticGui.WorkspaceWindow.Home.Workspaces;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace
{
    internal class CreateWorkspaceView :
        IPlasticDialogCloser,
        IWorkspacesRefreshableView
    {
        internal interface ICreateWorkspaceListener
        {
            void OnWorkspaceCreated(WorkspaceInfo wkInfo, bool isGluonMode);
        }

        internal CreateWorkspaceView(
            PlasticWindow parentWindow,
            ICreateWorkspaceListener listener,
            IPlasticAPI plasticApi,
            IPlasticWebRestApi plasticWebRestApi,
            string workspacePath)
        {
            mParentWindow = parentWindow;
            mCreateWorkspaceListener = listener;
            mWorkspacePath = workspacePath;
            mPlasticWebRestApi = plasticWebRestApi;

            mProgressControls = new ProgressControlsForViews();
            mWorkspaceOperations = new WorkspaceOperations(this, mProgressControls, null);
            mCreateWorkspaceState = CreateWorkspaceViewState.BuildForProjectDefaults();

            Initialize(plasticApi, plasticWebRestApi);
        }

        internal void Update()
        {
            mProgressControls.UpdateProgress(mParentWindow);
        }

        internal void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                mProgressControls.ProgressData.CopyInto(
                    mCreateWorkspaceState.ProgressData);
            }

            string repositoryName = mCreateWorkspaceState.RepositoryName;

            DrawCreateWorkspace.ForState(
                CreateRepository,
                ValidateAndCreateWorkspace,
                mParentWindow,
                mPlasticWebRestApi,
                mDefaultServer,
                ref mCreateWorkspaceState);

            if (repositoryName == mCreateWorkspaceState.RepositoryName)
                return;

            OnRepositoryChanged(
                mDialogUserAssistant,
                mCreateWorkspaceState,
                mWorkspacePath);
        }

        void Initialize(IPlasticAPI plasticApi, IPlasticWebRestApi plasticWebRestApi)
        {
            ((IProgressControls)mProgressControls).ShowProgress(string.Empty);

            WorkspaceInfo[] allWorkspaces = null;
            IList allRepositories = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    mDefaultServer = GetDefaultServer.ToCreateWorkspace(plasticWebRestApi);

                    allWorkspaces = plasticApi.GetAllWorkspacesArray();

                    allRepositories = plasticApi.GetAllRepositories(
                        mDefaultServer,
                        true);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)mProgressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(mProgressControls, waiter.Exception);
                        return;
                    }

                    string serverSpecPart = string.Format("@{0}", mDefaultServer);

                    mCreateWorkspaceState.RepositoryName = ValidRepositoryName.Get(
                        string.Format("{0}{1}",
                            mCreateWorkspaceState.RepositoryName,
                            serverSpecPart),
                        allRepositories);

                    mCreateWorkspaceState.WorkspaceName =
                        mCreateWorkspaceState.RepositoryName.Replace(
                            serverSpecPart,
                            string.Empty);

                    mDialogUserAssistant = new CreateWorkspaceDialogUserAssistant(
                        mWorkspacePath,
                        allWorkspaces);

                    OnRepositoryChanged(
                        mDialogUserAssistant,
                        mCreateWorkspaceState,
                        mWorkspacePath);
                });
        }

        static void OnRepositoryChanged(
            CreateWorkspaceDialogUserAssistant dialogUserAssistant,
            CreateWorkspaceViewState createWorkspaceState,
            string workspacePath)
        {
            if (dialogUserAssistant == null)
                return;

            dialogUserAssistant.RepositoryChanged(
                createWorkspaceState.RepositoryName,
                createWorkspaceState.WorkspaceName,
                workspacePath);

            createWorkspaceState.WorkspaceName =
                dialogUserAssistant.GetProposedWorkspaceName();
        }

        void CreateRepository(RepositoryCreationData data)
        {
            if (!data.Result)
                return;

            ((IProgressControls)mProgressControls).ShowProgress(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.CreatingRepository,
                    data.RepName));

            RepositoryInfo createdRepository = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    createdRepository = PlasticGui.Plastic.API.CreateRepository(
                        data.ServerName, data.RepName);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)mProgressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(mProgressControls, waiter.Exception);
                        return;
                    }

                    if (createdRepository == null)
                        return;

                    mCreateWorkspaceState.RepositoryName =
                        createdRepository.GetRepSpec().ToString();
                });
        }

        void ValidateAndCreateWorkspace(
            CreateWorkspaceViewState state)
        {
            mWkCreationData = BuildCreationDataFromState(
                state, mWorkspacePath);

            WorkspaceCreationValidation.AsyncValidation(
                mWkCreationData, this, mProgressControls);
            // validation calls IPlasticDialogCloser.CloseDialog()
            // when the validation is ok
        }

        void IPlasticDialogCloser.CloseDialog()
        {
            ((IProgressControls)mProgressControls).ShowProgress(string.Empty);

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    RepositorySpec repSpec = new SpecGenerator().GenRepositorySpec(
                        false, mWkCreationData.Repository);

                    bool repositoryExist = PlasticGui.Plastic.API.CheckRepositoryExists(
                        repSpec.Server, repSpec.Name);

                    if (!repositoryExist)
                        PlasticGui.Plastic.API.CreateRepository(repSpec.Server, repSpec.Name);
                },
                /*afterOperationDelegate*/ delegate
                {
                    ((IProgressControls)mProgressControls).HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(mProgressControls, waiter.Exception);
                        return;
                    }

                    mWkCreationData.Result = true;
                    mWorkspaceOperations.CreateWorkspace(mWkCreationData);

                    // the operation calls IWorkspacesRefreshableView.RefreshAndSelect
                    // when the workspace is created
                });
        }

        void IWorkspacesRefreshableView.RefreshAndSelect(WorkspaceInfo wkInfo)
        {
            PerformInitialCheckin.IfRepositoryIsEmpty(
                wkInfo,
                mWkCreationData.Repository,
                mWkCreationData.IsGluonWorkspace,
                PlasticGui.Plastic.API,
                mProgressControls,
                mCreateWorkspaceListener,
                mParentWindow);
        }

        static WorkspaceCreationData BuildCreationDataFromState(
            CreateWorkspaceViewState state,
            string workspacePath)
        {
            return new WorkspaceCreationData(
                state.WorkspaceName,
                workspacePath,
                state.RepositoryName,
                state.WorkspaceMode == CreateWorkspaceViewState.WorkspaceModes.Gluon,
                false);
        }

        static void DisplayException(
            IProgressControls progressControls,
            Exception ex)
        {
            ExceptionsHandler.LogException(
                "CreateWorkspaceView", ex);

            progressControls.ShowError(
                ExceptionsHandler.GetCorrectExceptionMessage(ex));
        }

        static class PerformInitialCheckin
        {
            internal static void IfRepositoryIsEmpty(
                WorkspaceInfo wkInfo,
                string repository,
                bool isGluonWorkspace,
                IPlasticAPI plasticApi,
                IProgressControls progressControls,
                ICreateWorkspaceListener createWorkspaceListener,
                PlasticWindow plasticWindow)
            {
                RepositoryInfo repInfo = null;
                bool isEmptyRepository = false;

                progressControls.ShowProgress(string.Empty);

                IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    RepositorySpec repSpec = new SpecGenerator().
                        GenRepositorySpec(false, repository);

                    repInfo = plasticApi.GetRepositoryInfo(repSpec);

                    isEmptyRepository = IsEmptyRepositoryCondition.
                        Evaluate(wkInfo, repSpec, plasticApi);
                },
                /*afterOperationDelegate*/ delegate
                {
                    progressControls.HideProgress();

                    if (waiter.Exception != null)
                    {
                        DisplayException(progressControls, waiter.Exception);
                        return;
                    }

                    if (!SetupCloudProjectId.HasCloudProjectId())
                        SetupCloudProjectId.ForRepository(repInfo);

                    if (!isEmptyRepository)
                    {
                        plasticWindow.RefreshWorkspaceUI();
                        return;
                    }

                    CheckinPackagesAndProjectSettingsFolders(
                        wkInfo, isGluonWorkspace, plasticApi,
                        progressControls, createWorkspaceListener);
                });
            }

            static void CheckinPackagesAndProjectSettingsFolders(
                WorkspaceInfo wkInfo,
                bool isGluonWorkspace,
                IPlasticAPI plasticApi,
                IProgressControls progressControls,
                ICreateWorkspaceListener createWorkspaceListener)
            {
                List<string> paths = new List<string> {
                    Path.Combine(wkInfo.ClientPath, "Packages"),
                    Path.Combine(wkInfo.ClientPath, "ProjectSettings")
                };

                string comment = PlasticLocalization.GetString(
                    PlasticLocalization.Name.UnityInitialCheckinComment);

                progressControls.ShowProgress(PlasticLocalization.GetString(
                    PlasticLocalization.Name.UnityInitialCheckinProgress));

                IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    PerformAdd(paths, plasticApi);

                    PerformCheckinForMode(wkInfo, paths, comment, isGluonWorkspace);
                },
                /*afterOperationDelegate*/ delegate
                {
                    progressControls.HideProgress();

                    if (waiter.Exception != null &&
                        !IsMergeNeededException(waiter.Exception))
                    {
                        DisplayException(progressControls, waiter.Exception);
                        return;
                    }

                    createWorkspaceListener.OnWorkspaceCreated(wkInfo, isGluonWorkspace);
                });
            }

            static void PerformAdd(
                List<string> paths,
                IPlasticAPI plasticApi)
            {
                AddOptions options = new AddOptions();
                options.AddPrivateParents = true;
                options.CheckoutParent = true;
                options.Recurse = true;
                options.SearchForPrivatePaths = true;
                options.SkipIgnored = true;

                IList checkouts;
                plasticApi.Add(paths.ToArray(), options, out checkouts);
            }

            static void PerformCheckinForMode(
                WorkspaceInfo wkInfo,
                List<string> paths,
                string comment,
                bool isGluonWorkspace)
            {
                if (isGluonWorkspace)
                {
                    new BaseCommandsImpl().PartialCheckin(wkInfo, paths, comment);
                    return;
                }

                CheckinParams ciParams = new CheckinParams();
                ciParams.paths = paths.ToArray();
                ciParams.comment = comment;
                ciParams.time = DateTime.MinValue;
                ciParams.flags = CheckinFlags.Recurse | CheckinFlags.ProcessSymlinks;

                new BaseCommandsImpl().CheckIn(ciParams);
            }

            static bool IsMergeNeededException(Exception exception)
            {
                if (exception == null)
                    return false;

                // Check the check-in exception for gluon
                if (exception is CheckinConflictsException)
                    return true;

                // Check the check-in exceptions for plastic
                return exception is CmClientMergeNeededException;
            }
        }

        class GetDefaultServer
        {
            internal static string ToCreateWorkspace(IPlasticWebRestApi plasticWebRestApi)
            {
                string clientConfServer = PlasticGui.Plastic.ConfigAPI.GetClientConfServer();

                if (!EditionToken.IsCloudEdition())
                    return clientConfServer;

                string cloudServer = PlasticGuiConfig.Get().
                    Configuration.DefaultCloudServer;

                if (!string.IsNullOrEmpty(cloudServer))
                    return cloudServer;

                CloudEditionCreds.Data config =
                    CloudEditionCreds.GetFromClientConf();

                cloudServer = GetFirstCloudServer.
                    GetCloudServer(plasticWebRestApi, config.Email, config.Password);

                if (string.IsNullOrEmpty(cloudServer))
                    return clientConfServer;

                SaveCloudServer.ToPlasticGuiConfig(cloudServer);

                return cloudServer;
            }
        }

        WorkspaceCreationData mWkCreationData;
        CreateWorkspaceViewState mCreateWorkspaceState;

        CreateWorkspaceDialogUserAssistant mDialogUserAssistant;

        string mDefaultServer;

        readonly WorkspaceOperations mWorkspaceOperations;
        readonly ProgressControlsForViews mProgressControls;
        readonly string mWorkspacePath;
        readonly ICreateWorkspaceListener mCreateWorkspaceListener;
        readonly PlasticWindow mParentWindow;
        readonly IPlasticWebRestApi mPlasticWebRestApi;
    }
}
