using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class MergedKeyedCollection<TKey, TItem> : IMergedCollection<TItem>
    {
        public MergedKeyedCollection() : base()
        {
            collections = new Dictionary<Type, IKeyedCollection<TKey, TItem>>();
            collectionsLookup = new Dictionary<Type, IKeyedCollection<TKey, TItem>>();
        }

        protected readonly Dictionary<Type, IKeyedCollection<TKey, TItem>> collections;

        // Used for performance optimization when finding the right collection for a type
        protected readonly Dictionary<Type, IKeyedCollection<TKey, TItem>> collectionsLookup;

        public TItem this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                foreach (var collectionByType in collections)
                {
                    if (collectionByType.Value.Contains(key))
                    {
                        return collectionByType.Value[key];
                    }
                }

                throw new KeyNotFoundException();
            }
        }

        public int Count
        {
            get
            {
                int count = 0;

                foreach (var collectionByType in collections)
                {
                    count += collectionByType.Value.Count;
                }

                return count;
            }
        }

        public bool IsReadOnly => false;

        public bool Includes<TSubItem>() where TSubItem : TItem
        {
            return Includes(typeof(TSubItem));
        }

        public bool Includes(Type elementType)
        {
            return GetCollectionForType(elementType, false) != null;
        }

        public IKeyedCollection<TKey, TSubItem> ForType<TSubItem>() where TSubItem : TItem
        {
            return ((VariantKeyedCollection<TItem, TSubItem, TKey>)GetCollectionForType(typeof(TSubItem))).implementation;
        }

        public virtual void Include<TSubItem>(IKeyedCollection<TKey, TSubItem> collection) where TSubItem : TItem
        {
            var type = typeof(TSubItem);
            var variantCollection = new VariantKeyedCollection<TItem, TSubItem, TKey>(collection);
            collections.Add(type, variantCollection);
            collectionsLookup.Add(type, variantCollection);
        }

        protected IKeyedCollection<TKey, TItem> GetCollectionForItem(TItem item)
        {
            Ensure.That(nameof(item)).IsNotNull(item);

            return GetCollectionForType(item.GetType());
        }

        protected IKeyedCollection<TKey, TItem> GetCollectionForType(Type type, bool throwOnFail = true)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            if (collectionsLookup.TryGetValue(type, out var collection))
            {
                return collection;
            }

            foreach (var collectionByType in collections)
            {
                if (collectionByType.Key.IsAssignableFrom(type))
                {
                    collection = collectionByType.Value;
                    collectionsLookup.Add(type, collection);
                    return collection;
                }
            }

            if (throwOnFail)
            {
                throw new InvalidOperationException($"No sub-collection available for type '{type}'.");
            }
            else
            {
                return null;
            }
        }

        protected IKeyedCollection<TKey, TItem> GetCollectionForKey(TKey key, bool throwOnFail = true)
        {
            // Optim: avoid boxing here.
            // Ensure.That(nameof(key)).IsNotNull(key);

            foreach (var collectionsByType in collections)
            {
                if (collectionsByType.Value.Contains(key))
                {
                    return collectionsByType.Value;
                }
            }

            if (throwOnFail)
            {
                throw new InvalidOperationException($"No sub-collection available for key '{key}'.");
            }
            else
            {
                return null;
            }
        }

        public bool TryGetValue(TKey key, out TItem value)
        {
            var collection = GetCollectionForKey(key, false);

            value = default(TItem);

            return collection != null && collection.TryGetValue(key, out value);
        }

        public virtual void Add(TItem item)
        {
            GetCollectionForItem(item).Add(item);
        }

        public void Clear()
        {
            foreach (var collection in collections.Values)
            {
                collection.Clear();
            }
        }

        public bool Contains(TItem item)
        {
            return GetCollectionForItem(item).Contains(item);
        }

        public bool Remove(TItem item)
        {
            return GetCollectionForItem(item).Remove(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException();
            }

            var i = 0;

            foreach (var collection in collections.Values)
            {
                collection.CopyTo(array, arrayIndex + i);
                i += collection.Count;
            }
        }

        public bool Contains(TKey key)
        {
            return GetCollectionForKey(key, false) != null;
        }

        public bool Remove(TKey key)
        {
            return GetCollectionForKey(key).Remove(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<TItem>
        {
            private Dictionary<Type, IKeyedCollection<TKey, TItem>>.Enumerator collectionsEnumerator;
            private TItem currentItem;
            private IKeyedCollection<TKey, TItem> currentCollection;
            private int indexInCurrentCollection;
            private bool exceeded;

            public Enumerator(MergedKeyedCollection<TKey, TItem> merged) : this()
            {
                collectionsEnumerator = merged.collections.GetEnumerator();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                // We just started, so we're not in a collection yet
                if (currentCollection == null)
                {
                    // Try to find the first collection
                    if (collectionsEnumerator.MoveNext())
                    {
                        // There is at least a collection, start with this one
                        currentCollection = collectionsEnumerator.Current.Value;

                        if (currentCollection == null)
                        {
                            throw new InvalidOperationException("Merged sub collection is null.");
                        }
                    }
                    else
                    {
                        // There is no collection at all, stop
                        currentItem = default(TItem);
                        exceeded = true;
                        return false;
                    }
                }

                // Check if we're within the current collection
                if (indexInCurrentCollection < currentCollection.Count)
                {
                    // We are, return this element and move to the next
                    currentItem = currentCollection[indexInCurrentCollection];
                    indexInCurrentCollection++;
                    return true;
                }

                // We're beyond the current collection, but there may be more,
                // and because there may be many empty collections, we need to check
                // them all until we find an element, not just the next one
                while (collectionsEnumerator.MoveNext())
                {
                    currentCollection = collectionsEnumerator.Current.Value;
                    indexInCurrentCollection = 0;

                    if (currentCollection == null)
                    {
                        throw new InvalidOperationException("Merged sub collection is null.");
                    }

                    if (indexInCurrentCollection < currentCollection.Count)
                    {
                        currentItem = currentCollection[indexInCurrentCollection];
                        indexInCurrentCollection++;
                        return true;
                    }
                }

                // We're beyond all collections, stop
                currentItem = default(TItem);
                exceeded = true;
                return false;
            }

            public TItem Current => currentItem;

            Object IEnumerator.Current
            {
                get
                {
                    if (exceeded)
                    {
                        throw new InvalidOperationException();
                    }

                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
