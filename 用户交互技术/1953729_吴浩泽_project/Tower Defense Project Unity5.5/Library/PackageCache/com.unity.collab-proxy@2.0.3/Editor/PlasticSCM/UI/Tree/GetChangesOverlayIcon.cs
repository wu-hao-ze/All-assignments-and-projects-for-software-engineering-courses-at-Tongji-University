using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.ThemeImages;
using PlasticGui.WorkspaceWindow.Merge;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.AssetsOverlays;

using GluonIncomingChangeInfo = PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges.IncomingChangeInfo;
using GluonIncomingChangeCategory = PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges.IncomingChangeCategory;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    static class GetChangesOverlayIcon
    {
        internal static Texture ForPlasticIncomingChange(
            MergeChangeInfo incomingChange,
            bool isSolvedConflict)
        {
            if (incomingChange.CategoryType == MergeChangesCategory.Type.FileConflicts ||
                incomingChange.CategoryType == MergeChangesCategory.Type.DirectoryConflicts)
                return ForConflict(isSolvedConflict);

            if (incomingChange.IsXLink())
                return ForXLink();

            if (incomingChange.CategoryType == MergeChangesCategory.Type.Deleted)
                return ForDeletedOnServer();

            if (incomingChange.CategoryType == MergeChangesCategory.Type.Changed)
                return ForOutOfDate();

            if (incomingChange.CategoryType == MergeChangesCategory.Type.Added)
                return ForAdded();

            return null;
        }

        internal static Texture ForGluonIncomingChange(
            GluonIncomingChangeInfo incomingChange,
            bool isSolvedConflict)
        {
            if (incomingChange.CategoryType == GluonIncomingChangeCategory.Type.Conflicted)
                return ForConflict(isSolvedConflict);

            if (incomingChange.IsXLink())
                return ForXLink();

            if (incomingChange.CategoryType == GluonIncomingChangeCategory.Type.Deleted)
                return ForDeletedOnServer();

            if (incomingChange.CategoryType == GluonIncomingChangeCategory.Type.Changed)
                return ForOutOfDate();

            if (incomingChange.CategoryType == GluonIncomingChangeCategory.Type.Added)
                return ForAdded();

            return null;
        }

        internal static Texture ForPendingChange(
            ChangeInfo changeInfo,
            bool isConflict)
        {
            if (isConflict)
                return ForConflicted();

            ItemIconImageType type = ChangeInfoView.
                GetIconImageType(changeInfo);

            if (ChangeTypesOperator.AreAllSet(
                    changeInfo.ChangeTypes, ChangeTypes.Added))
                return ForAdded();

            if (type.HasFlag(ItemIconImageType.Ignored))
                return ForIgnored();

            if (type.HasFlag(ItemIconImageType.Deleted))
                return ForDeleted();

            if (type.HasFlag(ItemIconImageType.CheckedOut))
                return ForCheckedOut();

            if (type.HasFlag(ItemIconImageType.Private))
                return ForPrivated();

            return null;
        }

        static Texture ForConflict(bool isResolved)
        {
            if (isResolved)
                return ForConflictResolved();

            return ForConflicted();
        }

        static Texture ForXLink()
        {
            return Images.GetImage(Images.Name.XLink);
        }

        static Texture ForIgnored()
        {
            return Images.GetIgnoredOverlayIcon();
        }

        static Texture ForPrivated()
        {
            return Images.GetPrivatedOverlayIcon();
        }

        static Texture ForAdded()
        {
            return Images.GetAddedOverlayIcon();
        }

        static Texture ForDeleted()
        {
            return Images.GetDeletedLocalOverlayIcon();
        }

        static Texture ForCheckedOut()
        {
            return Images.GetCheckedOutOverlayIcon();
        }

        static Texture ForDeletedOnServer()
        {
            return Images.GetDeletedRemoteOverlayIcon();
        }

        static Texture ForOutOfDate()
        {
            return Images.GetOutOfSyncOverlayIcon();
        }

        static Texture ForConflicted()
        {
            return Images.GetConflictedOverlayIcon();
        }

        static Texture ForConflictResolved()
        {
            return Images.GetConflictResolvedOverlayIcon();
        }
    }
}
