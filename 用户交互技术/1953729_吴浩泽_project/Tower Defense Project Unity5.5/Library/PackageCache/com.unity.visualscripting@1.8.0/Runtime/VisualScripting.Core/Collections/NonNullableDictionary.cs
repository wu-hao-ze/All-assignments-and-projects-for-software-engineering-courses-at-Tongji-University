using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class NonNullableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        public NonNullableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        public NonNullableDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public NonNullableDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public NonNullableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public NonNullableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public NonNullableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        private readonly Dictionary<TKey, TValue> dictionary;

        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }

            set
            {
                dictionary[key] = value;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return ((IDictionary)dictionary)[key];
            }
            set
            {
                ((IDictionary)dictionary)[key] = value;
            }
        }

        public int Count => dictionary.Count;

        public bool IsSynchronized => ((ICollection)dictionary).IsSynchronized;

        public object SyncRoot => ((ICollection)dictionary).SyncRoot;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => dictionary.Keys;

        ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

        ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

        public ICollection<TValue> Values => dictionary.Values;

        public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        public void CopyTo(Array array, int index)
        {
            ((ICollection)dictionary).CopyTo(array, index);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);
        }

        public void Add(TKey key, TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            dictionary.Add(key, value);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)dictionary).Add(key, value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(object key)
        {
            return ((IDictionary)dictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dictionary).GetEnumerator();
        }

        public void Remove(object key)
        {
            ((IDictionary)dictionary).Remove(key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }
}
