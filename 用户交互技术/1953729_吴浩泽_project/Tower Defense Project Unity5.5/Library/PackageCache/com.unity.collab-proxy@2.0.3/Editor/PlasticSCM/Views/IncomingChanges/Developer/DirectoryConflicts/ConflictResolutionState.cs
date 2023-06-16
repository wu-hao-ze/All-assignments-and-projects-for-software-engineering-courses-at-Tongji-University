using Codice.Client.BaseCommands.Merge;
using Codice.CM.Common.Merge;
using PlasticGui.WorkspaceWindow.Merge;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer.DirectoryConflicts
{
    internal class ConflictResolutionState
    {
        internal DirectoryConflictResolveActions ResolveAction { get; set; }
        internal string RenameValue { get; set; }
        internal bool IsApplyActionsForNextConflictsChecked { get; set; }

        internal static ConflictResolutionState Build(
            DirectoryConflict directoryConflict,
            DirectoryConflictAction[] conflictActions)
        {
            bool hasRenameOption = DirectoryConflictResolutionInfo.HasRenameOption(
                conflictActions);

            ConflictResolutionState result = new ConflictResolutionState()
            {
                IsApplyActionsForNextConflictsChecked = false,
                ResolveAction = (hasRenameOption) ?
                    DirectoryConflictResolveActions.Rename :
                    DirectoryConflictResolveActions.KeepSource,
            };

            if (!hasRenameOption)
                return result;


            result.RenameValue = DirectoryConflictResolutionInfo.GetProposeNewItemName(
                directoryConflict, "dst");

            return result;
        }

    }
}
