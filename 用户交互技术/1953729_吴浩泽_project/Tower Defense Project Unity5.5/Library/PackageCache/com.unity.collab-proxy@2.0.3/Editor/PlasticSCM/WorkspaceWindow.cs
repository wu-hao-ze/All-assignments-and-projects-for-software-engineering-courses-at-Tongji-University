using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using Codice.Client.BaseCommands;
using Codice.Client.Commands.CheckIn;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.CM.Common.Replication;
using GluonGui.WorkspaceWindow.Views;
using GluonGui;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Topbar;
using PlasticGui.WorkspaceWindow.Replication;
using PlasticGui.WorkspaceWindow.Update;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.Developer.UpdateReport;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

using IGluonUpdateReport = PlasticGui.Gluon.IUpdateReport;
using IGluonWorkspaceStatusChangeListener = PlasticGui.Gluon.IWorkspaceStatusChangeListener;
using GluonUpdateReportDialog = Unity.PlasticSCM.Editor.Gluon.UpdateReport.UpdateReportDialog;

namespace Unity.PlasticSCM.Editor
{
    internal class WorkspaceWindow :
        IWorkspaceWindow,
        IRefreshView,
        IUpdateReport,
        IGluonUpdateReport,
        IGluonWorkspaceStatusChangeListener
    {
        internal void SetUpdateNotifierForTesting(UpdateNotifier updateNotifier)
        {
            mUpdateNotifierForTesting = updateNotifier;
        }
        internal WorkspaceStatusString.Data WorkspaceStatus { get; private set; }
        internal OperationProgressData Progress { get { return mOperationProgressData; } }

        internal Gluon.ProgressOperationHandler GluonProgressOperationHandler
        {
            get { return mGluonProgressOperationHandler; }
        }

        internal WorkspaceWindow(
            WorkspaceInfo wkInfo,
            ViewHost viewHost,
            ViewSwitcher switcher,
            IMergeViewLauncher mergeViewLauncher,
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            EditorWindow parentWindow)
        {
            mWkInfo = wkInfo;
            mViewHost = viewHost;
            mSwitcher = switcher;
            mMergeViewLauncher = mergeViewLauncher;
            mDeveloperNewIncomingChangesUpdater = developerNewIncomingChangesUpdater;
            mPlasticWindow = parentWindow;
            mGuiMessage = new UnityPlasticGuiMessage();

            mDeveloperProgressOperationHandler = new Developer.ProgressOperationHandler(mWkInfo, this);
            mGluonProgressOperationHandler = new Gluon.ProgressOperationHandler(this);
            mOperationProgressData = new OperationProgressData();

            ((IWorkspaceWindow)this).UpdateTitle();
        }

        internal bool IsOperationInProgress()
        {
            return mDeveloperProgressOperationHandler.IsOperationInProgress()
                || mGluonProgressOperationHandler.IsOperationInProgress();
        }

        internal void CancelCurrentOperation()
        {
            if (mDeveloperProgressOperationHandler.IsOperationInProgress())
            {
                mDeveloperProgressOperationHandler.CancelCheckinProgress();
                return;
            }

            if (mGluonProgressOperationHandler.IsOperationInProgress())
            {
                mGluonProgressOperationHandler.CancelUpdateProgress();
                return;
            }
        }

        internal void OnParentUpdated(double elapsedSeconds)
        {
            if (IsOperationInProgress() || mRequestedRepaint)
            {
                if (mDeveloperProgressOperationHandler.IsOperationInProgress())
                    mDeveloperProgressOperationHandler.Update(elapsedSeconds);

                mPlasticWindow.Repaint();

                mRequestedRepaint = false;
            }
        }

        internal void RequestRepaint()
        {
            mRequestedRepaint = true;
        }

        internal void UpdateWorkspace()
        {
            UpdateWorkspaceOperation update = new UpdateWorkspaceOperation(
                mWkInfo, this, mSwitcher, mMergeViewLauncher, this,
                mDeveloperNewIncomingChangesUpdater,
                null);

            update.Run(
                UpdateWorkspaceOperation.UpdateType.UpdateToLatest,
                RefreshAsset.UnityAssetDatabase,
                null);
        }

        void IWorkspaceWindow.RefreshView(ViewType viewType)
        {
            mSwitcher.RefreshView(viewType);
        }

        void IWorkspaceWindow.RefreshWorkingObjectViews(
            ViewType viewType,
            WorkingObjectInfo workingObjectInfo)
        {
            mSwitcher.RefreshWorkingObjectInfoForSelectedView(
                viewType,
                workingObjectInfo);
        }

        void IWorkspaceWindow.UpdateTitle()
        {
            UpdateWorkspaceTitle();
        }

        bool IWorkspaceWindow.CheckOperationInProgress()
        {
            return mDeveloperProgressOperationHandler.CheckOperationInProgress();
        }

        void IWorkspaceWindow.ShowUpdateProgress(string title, UpdateNotifier notifier)
        {
            mDeveloperProgressOperationHandler.ShowUpdateProgress(title, mUpdateNotifierForTesting ?? notifier);
        }

        void IWorkspaceWindow.EndUpdateProgress()
        {
            mDeveloperProgressOperationHandler.EndUpdateProgress();
        }

        void IWorkspaceWindow.ShowCheckinProgress()
        {
            mDeveloperProgressOperationHandler.ShowCheckinProgress();
        }

        void IWorkspaceWindow.EndCheckinProgress()
        {
            mDeveloperProgressOperationHandler.EndCheckinProgress();
        }

        void IWorkspaceWindow.RefreshCheckinProgress(
            CheckinStatus checkinStatus,
            BuildProgressSpeedAndRemainingTime.ProgressData progressData)
        {
            mDeveloperProgressOperationHandler.
                RefreshCheckinProgress(checkinStatus, progressData);
        }

        bool IWorkspaceWindow.HasCheckinCancelled()
        {
            return mDeveloperProgressOperationHandler.HasCheckinCancelled();
        }

        void IWorkspaceWindow.ShowReplicationProgress(IReplicationOperation replicationOperation)
        {
            throw new NotImplementedException();
        }

        void IWorkspaceWindow.RefreshReplicationProgress(BranchReplicationData replicationData, ReplicationStatus replicationStatus, int current, int total)
        {
            throw new NotImplementedException();
        }

        void IWorkspaceWindow.EndReplicationProgress(ReplicationStatus replicationStatus)
        {
            throw new NotImplementedException();
        }

        void IWorkspaceWindow.ShowProgress()
        {
            mDeveloperProgressOperationHandler.ShowProgress();
        }

        void IWorkspaceWindow.ShowProgress(IProgressOperation progressOperation)
        {
            throw new NotImplementedException();
        }

        void IWorkspaceWindow.RefreshProgress(ProgressData progressData)
        {
            mDeveloperProgressOperationHandler.RefreshProgress(progressData);
        }

        void IWorkspaceWindow.EndProgress()
        {
            mDeveloperProgressOperationHandler.EndProgress();
        }

        EncryptionConfigurationDialogData IWorkspaceWindow.RequestEncryptionPassword(string server)
        {
            return EncryptionConfigurationDialog.RequestEncryptionPassword(server, mPlasticWindow);
        }

        void IRefreshView.ForType(ViewType viewType)
        {
            mSwitcher.RefreshView(viewType);
        }

        void IUpdateReport.Show(WorkspaceInfo wkInfo, IList reportLines)
        {
            UpdateReportDialog.ShowReportDialog(
                wkInfo,
                reportLines,
                mPlasticWindow);
        }

        void IGluonUpdateReport.AppendReport(string updateReport)
        {
            throw new NotImplementedException();
        }

        void IGluonWorkspaceStatusChangeListener.OnWorkspaceStatusChanged()
        {
            UpdateWorkspaceTitle();
        }

        void UpdateWorkspaceTitle()
        {
            WorkspaceStatusString.Data status = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    status = WorkspaceStatusString.GetSelectorData(mWkInfo);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                        return;

                    WorkspaceStatus = status;

                    RequestRepaint();
                });
        }

        bool mRequestedRepaint;

        UpdateNotifier mUpdateNotifierForTesting;
        IProgressControls mProgressControls;

        readonly OperationProgressData mOperationProgressData;
        readonly Developer.ProgressOperationHandler mDeveloperProgressOperationHandler;
        readonly Gluon.ProgressOperationHandler mGluonProgressOperationHandler;
        readonly GuiMessage.IGuiMessage mGuiMessage;
        readonly EditorWindow mPlasticWindow;
        readonly NewIncomingChangesUpdater mDeveloperNewIncomingChangesUpdater;
        readonly IMergeViewLauncher mMergeViewLauncher;
        readonly ViewSwitcher mSwitcher;
        readonly ViewHost mViewHost;
        readonly WorkspaceInfo mWkInfo;

        internal void RegisterPendingChangesProgressControls(
            ProgressControlsForViews progressControls)
        {
            mProgressControls = progressControls;
        }

        internal void UpdateWorkspaceForMode(
            bool isGluonMode,
            WorkspaceWindow workspaceWindow)
        {
            if (isGluonMode)
            {
                PartialUpdateWorkspace();
                return;
            }

            UpdateWorkspace();
        }

        UpdateReportResult IGluonUpdateReport.ShowUpdateReport(
            WorkspaceInfo wkInfo, List<ErrorMessage> errors)
        {
            return GluonUpdateReportDialog.ShowUpdateReport(
                wkInfo, errors, mPlasticWindow);
        }

        void PartialUpdateWorkspace()
        {
            mProgressControls.ShowProgress(PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdatingWorkspace));

            ((IUpdateProgress)mGluonProgressOperationHandler).ShowCancelableProgress();

            OutOfDateUpdater outOfDateUpdater = new OutOfDateUpdater(mWkInfo, null);

            BuildProgressSpeedAndRemainingTime.ProgressData progressData =
                new BuildProgressSpeedAndRemainingTime.ProgressData(DateTime.Now);

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    outOfDateUpdater.Execute();
                },
                /*afterOperationDelegate*/ delegate
                {
                    mProgressControls.HideProgress();

                    ((IUpdateProgress)mGluonProgressOperationHandler).EndProgress();

                    mViewHost.RefreshView(ViewType.CheckinView);
                    mViewHost.RefreshView(ViewType.IncomingChangesView);

                    RefreshAsset.UnityAssetDatabase();

                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    ShowUpdateReportDialog(
                        mWkInfo, mViewHost, outOfDateUpdater.Progress, mProgressControls,
                        mGuiMessage, mGluonProgressOperationHandler, this);
                },
                /*timerTickDelegate*/ delegate
                {
                    UpdateProgress progress = outOfDateUpdater.Progress;

                    if (progress == null)
                        return;

                    if (progress.IsCanceled)
                    {
                        mProgressControls.ShowNotification(
                            PlasticLocalization.GetString(PlasticLocalization.Name.Canceling));
                    }

                    ((IUpdateProgress)mGluonProgressOperationHandler).RefreshProgress(
                        progress,
                        UpdateProgressDataCalculator.CalculateProgressForWorkspaceUpdate(
                            mWkInfo.ClientPath, progress, progressData));
                });
        }

        static void ShowUpdateReportDialog(
            WorkspaceInfo wkInfo,
            ViewHost viewHost,
            UpdateProgress progress,
            IProgressControls progressControls,
            GuiMessage.IGuiMessage guiMessage,
            IUpdateProgress updateProgress,
            IGluonUpdateReport updateReport)
        {
            if (progress.ErrorMessages.Count == 0)
                return;

            UpdateReportResult updateReportResult =
                updateReport.ShowUpdateReport(wkInfo, progress.ErrorMessages);

            if (!updateReportResult.IsUpdateForcedRequested())
                return;

            UpdateForcedOperation updateForced = new UpdateForcedOperation(
                wkInfo, viewHost, progress, progressControls,
                guiMessage, updateProgress, updateReport);

            updateForced.UpdateForced(
                updateReportResult.UpdateForcedPaths,
                updateReportResult.UnaffectedErrors);
        }
    }
}
