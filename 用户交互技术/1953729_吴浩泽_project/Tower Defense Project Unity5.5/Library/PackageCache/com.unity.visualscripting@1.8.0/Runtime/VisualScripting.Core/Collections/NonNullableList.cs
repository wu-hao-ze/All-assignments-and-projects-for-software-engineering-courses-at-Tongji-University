using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class NonNullableList<T> : IList<T>, IList
    {
        public NonNullableList()
        {
            list = new List<T>();
        }

        public NonNullableList(int capacity)
        {
            list = new List<T>(capacity);
        }

        public NonNullableList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
        }

        private readonly List<T> list;

        public T this[int index]
        {
            get
            {
                return list[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                list[index] = value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)list)[index];
            }
            set
            {
                ((IList)list)[index] = value;
            }
        }

        public int Count => list.Count;

        public bool IsSynchronized => ((ICollection)list).IsSynchronized;

        public object SyncRoot => ((ICollection)list).SyncRoot;

        public bool IsReadOnly => false;

        public bool IsFixedSize => ((IList)list).IsFixedSize;

        public void CopyTo(Array array, int index)
        {
            ((ICollection)list).CopyTo(array, index);
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            list.Add(item);
        }

        public int Add(object value)
        {
            return ((IList)list).Add(value);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(object value)
        {
            return ((IList)list).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)list).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)list).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList)list).Remove(value);
        }

        public bool Contains(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }
    }
}
