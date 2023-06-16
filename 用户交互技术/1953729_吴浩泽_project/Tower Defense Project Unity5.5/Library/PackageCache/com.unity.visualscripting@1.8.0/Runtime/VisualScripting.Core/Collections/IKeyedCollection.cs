using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IKeyedCollection<TKey, TItem> : ICollection<TItem>
    {
        TItem this[TKey key] { get; }
        TItem this[int index] { get; } // For allocation free enumerators
        bool TryGetValue(TKey key, out TItem value);
        bool Contains(TKey key);
        bool Remove(TKey key);
    }
}
