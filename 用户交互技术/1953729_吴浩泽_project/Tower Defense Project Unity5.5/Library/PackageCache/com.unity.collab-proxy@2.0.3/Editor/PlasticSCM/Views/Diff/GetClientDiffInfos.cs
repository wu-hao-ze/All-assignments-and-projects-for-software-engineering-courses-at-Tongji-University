using System.Collections.Generic;

using Codice.Utils;
using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal static class GetClientDiffInfos
    {
        internal static List<ClientDiffInfo> FromCategories(List<IDiffCategory> categories)
        {
            List<ClientDiffInfo> result = new List<ClientDiffInfo>();

            foreach (ITreeViewNode node in categories)
                AddClientDiffInfos(node, result);

            return result;
        }

        static void AddClientDiffInfos(ITreeViewNode node, List<ClientDiffInfo> result)
        {
            if (node is ClientDiffInfo)
            {
                result.Add((ClientDiffInfo)node);
                return;
            }

            for (int i = 0; i < node.GetChildrenCount(); i++)
                AddClientDiffInfos(node.GetChild(i), result);

        }
    }
}
