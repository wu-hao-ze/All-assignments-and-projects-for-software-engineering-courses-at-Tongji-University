using Codice.Client.Commands.WkTree;
using Codice.CM.Common;

namespace Codice
{
    internal static class CheckWorkspaceTreeNodeStatus
    {
        internal static bool IsPrivate(WorkspaceTreeNode node)
        {
            return node == null;
        }

        internal static bool IsCheckedOut(WorkspaceTreeNode node)
        {
            if (node == null)
                return false;

            return node.RevInfo.CheckedOut;
        }

        internal static bool IsAdded(WorkspaceTreeNode node)
        {
            if (node == null)
                return false;

            return node.RevInfo.CheckedOut &&
                   node.RevInfo.ParentId == -1;
        }

        internal static bool IsDirectory(WorkspaceTreeNode node)
        {
            return node.RevInfo.Type == EnumRevisionType.enDirectory;
        }
    }
}
