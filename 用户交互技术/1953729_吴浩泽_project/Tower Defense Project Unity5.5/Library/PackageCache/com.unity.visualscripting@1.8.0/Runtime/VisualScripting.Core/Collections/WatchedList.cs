using System;
using System.Collections.ObjectModel;

namespace Unity.VisualScripting
{
    public class WatchedList<T> : Collection<T>, INotifyCollectionChanged<T>
    {
        public event Action<T> ItemAdded;

        public event Action<T> ItemRemoved;

        public event Action CollectionChanged;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            ItemAdded?.Invoke(item);
            CollectionChanged?.Invoke();
        }

        protected override void RemoveItem(int index)
        {
            if (index < Count)
            {
                var item = this[index];
                base.RemoveItem(index);
                ItemRemoved?.Invoke(item);
                CollectionChanged?.Invoke();
            }
        }

        protected override void ClearItems()
        {
            while (Count > 0)
            {
                RemoveItem(0);
            }
        }
    }
}
