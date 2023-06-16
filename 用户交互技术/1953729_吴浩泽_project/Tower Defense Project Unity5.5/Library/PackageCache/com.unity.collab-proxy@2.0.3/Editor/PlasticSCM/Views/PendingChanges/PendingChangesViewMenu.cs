using System.Collections.Generic;

using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Open;
using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Changelists;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class PendingChangesViewMenu
    {
        internal interface IGetSelectedNodes
        {
            List<IPlasticTreeNode> GetSelectedNodes();
        }

        internal PendingChangesViewMenu(
            WorkspaceInfo wkInfo,
            IPendingChangesMenuOperations pendingChangesViewOperations,
            IFilesFilterPatternsMenuOperations filterMenuOperations,
            IOpenMenuOperations openMenuOperations,
            PendingChangesViewPendingChangeMenu.IMetaMenuOperations metaMenuOperations,
            IChangelistMenuOperations changelistMenuOperations,
            IGetSelectedNodes getSelectedNodes,
            bool isGluonMode)
        {
            mWkInfo = wkInfo;
            mPendingChangesViewOperations = pendingChangesViewOperations;
            mFilterMenuOperations = filterMenuOperations;
            mOpenMenuOperations = openMenuOperations;
            mMetaMenuOperations = metaMenuOperations;
            mChangelistMenuOperations = changelistMenuOperations;
            mGetSelectedNodes = getSelectedNodes;
            mIsGluonMode = isGluonMode;
        }

        internal void Popup()
        {
            List<IPlasticTreeNode> selectedNodes = mGetSelectedNodes.GetSelectedNodes();

            if (AreAllChangelists(selectedNodes))
            {
                GetChangelistMenu().Popup();
                return;
            }

            if (AreAllPendingChanges(selectedNodes))
            {
                GetPendingChangeMenu().Popup();
                return;
            }
        }

        internal bool ProcessKeyActionIfNeeded(Event e)
        {
            List<IPlasticTreeNode> selectedNodes = mGetSelectedNodes.GetSelectedNodes();

            if (AreAllChangelists(selectedNodes))
            {
                return GetChangelistMenu().ProcessKeyActionIfNeeded(e);
            }

            if (AreAllPendingChanges(selectedNodes))
            {
                return GetPendingChangeMenu().ProcessKeyActionIfNeeded(e);
            }

            return false;
        }

        PendingChangesViewPendingChangeMenu GetPendingChangeMenu()
        {
            if (mPendingChangeMenu == null)
            {
                mPendingChangeMenu = new PendingChangesViewPendingChangeMenu(
                    mWkInfo,
                    mPendingChangesViewOperations,
                    mChangelistMenuOperations,
                    mOpenMenuOperations,
                    mMetaMenuOperations,
                    mFilterMenuOperations);
            }

            return mPendingChangeMenu;
        }

        ChangelistMenu GetChangelistMenu()
        {
            if (mChangelistMenu == null)
                mChangelistMenu = new ChangelistMenu(
                    mChangelistMenuOperations,
                    mIsGluonMode);

            return mChangelistMenu;
        }

        static bool AreAllChangelists(List<IPlasticTreeNode> selectedNodes)
        {
            foreach (IPlasticTreeNode node in selectedNodes)
            {
                if (!(node is ChangelistNode))
                    return false;
            }
            return true;
        }

        static bool AreAllPendingChanges(List<IPlasticTreeNode> selectedNodes)
        {
            foreach (IPlasticTreeNode node in selectedNodes)
            {
                if (!(node is PendingChangeInfo))
                    return false;
            }
            return true;
        }

        PendingChangesViewPendingChangeMenu mPendingChangeMenu;
        ChangelistMenu mChangelistMenu;

        readonly WorkspaceInfo mWkInfo;
        readonly IPendingChangesMenuOperations mPendingChangesViewOperations;
        readonly IFilesFilterPatternsMenuOperations mFilterMenuOperations;
        readonly IOpenMenuOperations mOpenMenuOperations;
        readonly PendingChangesViewPendingChangeMenu.IMetaMenuOperations mMetaMenuOperations;
        readonly IChangelistMenuOperations mChangelistMenuOperations;
        readonly IGetSelectedNodes mGetSelectedNodes;
        readonly bool mIsGluonMode;
    }
}
