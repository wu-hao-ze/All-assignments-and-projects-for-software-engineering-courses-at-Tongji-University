using System.Collections.Generic;
using System.Linq;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    internal class TreeViewItemIds<C, I>
    {
        internal void Clear()
        {
            mCacheByCategories.Clear();
            mCacheByInfo.Clear();
        }

        internal List<int> GetCategoryIds()
        {
            return new List<int>(mCacheByCategories.Values);
        }

        internal List<KeyValuePair<C, int>> GetCategoryItems()
        {
            return mCacheByCategories.ToList();
        }

        internal List<KeyValuePair<I, int>> GetInfoItems()
        {
            return mCacheByInfo.ToList();
        }

        internal bool TryGetCategoryItemId(C category, out int itemId)
        {
            return mCacheByCategories.TryGetValue(category, out itemId);
        }

        internal bool TryGetInfoItemId(I info, out int itemId)
        {
            return mCacheByInfo.TryGetValue(info, out itemId);
        }

        internal int AddCategoryItem(C category)
        {
            int itemId = GetNextItemId();

            mCacheByCategories.Add(category, itemId);

            return itemId;
        }

        internal int AddInfoItem(I info)
        {
            int itemId = GetNextItemId();

            mCacheByInfo.Add(info, itemId);

            return itemId;
        }

        int GetNextItemId()
        {
            return mCacheByCategories.Count
                + mCacheByInfo.Count
                + 1;
        }

        Dictionary<C, int> mCacheByCategories = new Dictionary<C, int>();
        Dictionary<I, int> mCacheByInfo = new Dictionary<I, int>();
    }
}
