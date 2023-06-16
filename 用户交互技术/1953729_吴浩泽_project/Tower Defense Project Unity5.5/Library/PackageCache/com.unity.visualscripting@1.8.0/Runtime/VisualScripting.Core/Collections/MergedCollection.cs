using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class MergedCollection<T> : IMergedCollection<T>
    {
        public MergedCollection()
        {
            collections = new Dictionary<Type, ICollection<T>>();
        }

        private readonly Dictionary<Type, ICollection<T>> collections;

        public int Count
        {
            get
            {
                int count = 0;

                foreach (var collection in collections.Values)
                {
                    count += collection.Count;
                }

                return count;
            }
        }

        public bool IsReadOnly => false;

        public void Include<TI>(ICollection<TI> collection) where TI : T
        {
            collections.Add(typeof(TI), new VariantCollection<T, TI>(collection));
        }

        public bool Includes<TI>() where TI : T
        {
            return Includes(typeof(TI));
        }

        public bool Includes(Type implementationType)
        {
            return GetCollectionForType(implementationType, false) != null;
        }

        public ICollection<TI> ForType<TI>() where TI : T
        {
            return ((VariantCollection<T, TI>)GetCollectionForType(typeof(TI))).implementation;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var collection in collections.Values)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }
            }
        }

        private ICollection<T> GetCollectionForItem(T item)
        {
            Ensure.That(nameof(item)).IsNotNull(item);

            return GetCollectionForType(item.GetType());
        }

        private ICollection<T> GetCollectionForType(Type type, bool throwOnFail = true)
        {
            if (collections.ContainsKey(type))
            {
                return collections[type];
            }

            foreach (var collectionByType in collections)
            {
                if (collectionByType.Key.IsAssignableFrom(type))
                {
                    return collectionByType.Value;
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

        public bool Contains(T item)
        {
            return GetCollectionForItem(item).Contains(item);
        }

        public virtual void Add(T item)
        {
            GetCollectionForItem(item).Add(item);
        }

        public virtual void Clear()
        {
            foreach (var collection in collections.Values)
            {
                collection.Clear();
            }
        }

        public virtual bool Remove(T item)
        {
            return GetCollectionForItem(item).Remove(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
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
    }
}
