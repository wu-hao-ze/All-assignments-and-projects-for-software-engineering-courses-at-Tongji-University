using System;

namespace Unity.VisualScripting
{
    public interface INotifyCollectionChanged<T>
    {
        event Action<T> ItemAdded;

        event Action<T> ItemRemoved;

        event Action CollectionChanged;
    }
}
