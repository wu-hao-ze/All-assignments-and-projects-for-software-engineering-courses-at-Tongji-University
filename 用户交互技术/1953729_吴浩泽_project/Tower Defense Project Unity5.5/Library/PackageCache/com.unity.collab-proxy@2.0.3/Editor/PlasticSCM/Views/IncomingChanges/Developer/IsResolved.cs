using Codice.Client.BaseCommands.Merge;
using PlasticGui.WorkspaceWindow.Merge;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal static class IsSolved
    {
        internal static bool Conflict(
            MergeChangeInfo changeInfo,
            MergeChangeInfo metaChangeInfo,
            MergeSolvedFileConflicts solvedFileConflicts)
        {
            if (IsDirectoryConflict(changeInfo))
            {
                if (metaChangeInfo == null)
                    return IsDirectoryConflictResolved(changeInfo);

                return IsDirectoryConflictResolved(changeInfo) &&
                       IsDirectoryConflictResolved(metaChangeInfo);
            }

            if (metaChangeInfo == null)
            {
                return IsFileConflictResolved(
                    changeInfo, solvedFileConflicts);
            }

            return IsFileConflictResolved(changeInfo, solvedFileConflicts) && 
                   IsFileConflictResolved(metaChangeInfo, solvedFileConflicts);
        }

        static bool IsFileConflictResolved(
            MergeChangeInfo changeInfo,
            MergeSolvedFileConflicts solvedFileConflicts)
        {
            if (solvedFileConflicts == null)
                return false;

            return solvedFileConflicts.IsResolved(
                changeInfo.GetMount().Id,
                changeInfo.GetRevision().ItemId);
        }

        static bool IsDirectoryConflictResolved(MergeChangeInfo changeInfo)
        {
            return changeInfo.DirectoryConflict.IsResolved();
        }

        static bool IsDirectoryConflict(MergeChangeInfo changeInfo)
        {
            return (changeInfo.DirectoryConflict != null);
        }
    }
}
