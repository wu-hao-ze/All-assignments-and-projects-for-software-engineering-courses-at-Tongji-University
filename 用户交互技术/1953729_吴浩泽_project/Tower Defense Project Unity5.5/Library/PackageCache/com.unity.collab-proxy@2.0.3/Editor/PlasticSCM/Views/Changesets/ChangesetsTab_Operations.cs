using UnityEditor;

using Codice.CM.Common;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal partial class ChangesetsTab
    {
        void SwitchToChangesetForMode(bool isGluonMode)
        {
            if (isGluonMode)
            {
                SwitchToChangesetForGluon();
                return;
            }

            SwitchToChangesetForDeveloper();
        }

        void SwitchToChangesetForDeveloper()
        {
            mChangesetOperations.SwitchToChangeset(
                ChangesetsSelection.GetSelectedRepository(mChangesetsListView),
                ChangesetsSelection.GetSelectedChangeset(mChangesetsListView),
                RefreshAsset.BeforeLongAssetOperation,
                RefreshAsset.AfterLongAssetOperation);
        }

        void SwitchToChangesetForGluon()
        {
            ChangesetExtendedInfo csetInfo = ChangesetsSelection.GetSelectedChangeset(mChangesetsListView);

            SwitchToUIOperation.SwitchToChangeset(
                mWkInfo,
                csetInfo.BranchName,
                csetInfo.Id,
                mViewHost,
                new UnityPlasticGuiMessage(),
                mProgressControls,
                mWorkspaceWindow.GluonProgressOperationHandler,
                mGluonUpdateReport,
                mWorkspaceWindow,
                RefreshAsset.BeforeLongAssetOperation,
                RefreshAsset.AfterLongAssetOperation);
        }
    }
}