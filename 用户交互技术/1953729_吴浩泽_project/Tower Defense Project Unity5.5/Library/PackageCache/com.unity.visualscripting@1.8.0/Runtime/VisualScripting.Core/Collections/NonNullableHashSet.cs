using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class NonNullableHashSet<T> : ISet<T>
    {
        public NonNullableHashSet()
        {
            set = new HashSet<T>();
        }

        public NonNullableHashSet(IEqualityComparer<T> comparer)
        {
            set = new HashSet<T>(comparer);
        }

        public NonNullableHashSet(IEnumerable<T> collection)
        {
            set = new HashSet<T>(collection);
        }

        public NonNullableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            set = new HashSet<T>(collection, comparer);
        }

        private readonly HashSet<T> set;

        public int Count => set.Count;

        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return set.Add(item);
        }

        public void Clear()
        {
            set.Clear();
        }

        public bool Contains(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            set.ExceptWith(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return set.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return set.Overlaps(other);
        }

        public bool Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return set.Remove(item);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            set.UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            ((ICollection<T>)set).Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)set).GetEnumerator();
        }
    }
}
