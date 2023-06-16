using System.Collections.Generic;

using Codice.CM.Common;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.History
{
    internal static class HistorySelection
    {
        internal static void SelectRevisions(
            HistoryListView listView,
            List<RepObjectInfo> revisionsToSelect)
        {
            if (revisionsToSelect == null || revisionsToSelect.Count == 0)
            {
                TableViewOperations.SelectFirstRow(listView);
                return;
            }

            listView.SelectRepObjectInfos(revisionsToSelect);

            if (listView.HasSelection())
                return;

            TableViewOperations.SelectFirstRow(listView);
        }

        internal static List<RepObjectInfo> GetSelectedRepObjectInfos(
            HistoryListView listView)
        {
            return listView.GetSelectedRepObjectInfos();
        }

        internal static List<HistoryRevision> GetSelectedHistoryRevisions(
            HistoryListView listView)
        {
            return listView.GetSelectedHistoryRevisions();
        }

        internal static HistoryRevision GetSelectedHistoryRevision(
            HistoryListView listView)
        {
            List<HistoryRevision> revisions =
                listView.GetSelectedHistoryRevisions();

            if (revisions.Count == 0)
                return null;

            return revisions[0];
        }
    }
}
