using System.Collections.Generic;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class SortOrderComparer<T> : IComparer<T>
    {
        internal SortOrderComparer(IComparer<T> comparer, bool isAscending)
        {
            mComparer = comparer;
            mIsAscending = isAscending;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            int result = mComparer.Compare(x, y);
            return mIsAscending ? result : -result;
        }

        bool mIsAscending;
        IComparer<T> mComparer;
    }
}
