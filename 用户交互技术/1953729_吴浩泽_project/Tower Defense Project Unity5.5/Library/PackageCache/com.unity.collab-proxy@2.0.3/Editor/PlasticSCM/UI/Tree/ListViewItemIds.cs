using System.Collections.Generic;
using System.Linq;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    internal class ListViewItemIds<I>
    {
        internal void Clear()
        {
            mCacheByInfo.Clear();
        }

        internal List<KeyValuePair<I, int>> GetInfoItems()
        {
            return mCacheByInfo.ToList();
        }

        internal bool TryGetInfoItemId(I info, out int itemId)
        {
            return mCacheByInfo.TryGetValue(info, out itemId);
        }

        internal int AddInfoItem(I info)
        {
            int itemId = GetNextItemId();

            mCacheByInfo.Add(info, itemId);

            return itemId;
        }

        int GetNextItemId()
        {
            return mCacheByInfo.Count + 1;
        }

        Dictionary<I, int> mCacheByInfo = new Dictionary<I, int>();
    }
}

