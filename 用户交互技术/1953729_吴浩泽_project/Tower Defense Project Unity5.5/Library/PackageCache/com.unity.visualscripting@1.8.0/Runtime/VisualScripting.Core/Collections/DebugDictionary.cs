using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public class DebugDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                Debug($"Set: {key} => {value}");
                dictionary[key] = value;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        public string label { get; set; } = "Dictionary";

        public bool debug { get; set; } = false;

        public int Count => dictionary.Count;

        object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)dictionary).IsSynchronized;

        ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

        bool IDictionary.IsReadOnly => ((IDictionary)dictionary).IsReadOnly;

        bool IDictionary.IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

        public ICollection<TKey> Keys => dictionary.Keys;

        ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

        public ICollection<TValue> Values => dictionary.Values;

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)dictionary).CopyTo(array, index);
        }

        private void Debug(string message)
        {
            if (!debug)
            {
                return;
            }

            if (!string.IsNullOrEmpty(label))
            {
                message = $"[{label}] {message}";
            }

            UnityEngine.Debug.Log(message + "\n");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        public void Clear()
        {
            Debug("Clear");
            dictionary.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dictionary).GetEnumerator();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            Debug($"Add: {key} => {value}");
            dictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            Debug($"Remove: {key}");
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }
    }
}
