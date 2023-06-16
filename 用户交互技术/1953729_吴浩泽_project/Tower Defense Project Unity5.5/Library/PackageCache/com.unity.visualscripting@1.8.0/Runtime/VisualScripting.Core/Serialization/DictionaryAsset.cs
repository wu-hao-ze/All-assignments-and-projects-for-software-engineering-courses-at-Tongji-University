using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    [IncludeInSettings(false)]
    public sealed class DictionaryAsset : LudiqScriptableObject, IDictionary<string, object>
    {
        public object this[string key]
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

        [Serialize]
        public Dictionary<string, object> dictionary { get; private set; } = new Dictionary<string, object>();

        public int Count => dictionary.Count;

        public ICollection<string> Keys => dictionary.Keys;

        public ICollection<object> Values => dictionary.Values;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => ((ICollection<KeyValuePair<string, object>>)dictionary).IsReadOnly;

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            if (dictionary == null)
            {
                dictionary = new Dictionary<string, object>();
            }
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            dictionary.Add(key, value);
        }

        public void Merge(DictionaryAsset other, bool overwriteExisting = true)
        {
            foreach (var key in other.Keys)
            {
                if (overwriteExisting)
                {
                    dictionary[key] = other[key];
                }
                else if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, other[key]);
                }
            }
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)dictionary).Add(item);
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)dictionary).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)dictionary).Remove(item);
        }

        [ContextMenu("Show Data...")]
        protected override void ShowData()
        {
            base.ShowData();
        }
    }
}
