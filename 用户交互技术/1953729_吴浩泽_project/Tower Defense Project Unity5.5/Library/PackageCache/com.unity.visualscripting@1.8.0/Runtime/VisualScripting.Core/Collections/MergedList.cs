using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    // The advantage of this class is not to provide list accessors
    // for merged lists (which would be confusing), but rather that unlike
    // merged collection, it can provide a zero-allocation enumerator.

    // OPTIM note: Dictionary<,>.Values allocated memory the first time, so avoid it if possible
    public class MergedList<T> : IMergedCollection<T>
    {
        public MergedList()
        {
            lists = new Dictionary<Type, IList<T>>();
        }

        protected readonly Dictionary<Type, IList<T>> lists;

        public int Count
        {
            get
            {
                int count = 0;

                foreach (var listByType in lists)
                {
                    count += listByType.Value.Count;
                }

                return count;
            }
        }

        public bool IsReadOnly => false;

        public virtual void Include<TI>(IList<TI> list) where TI : T
        {
            lists.Add(typeof(TI), new VariantList<T, TI>(list));
        }

        public bool Includes<TI>() where TI : T
        {
            return Includes(typeof(TI));
        }

        public bool Includes(Type elementType)
        {
            return GetListForType(elementType, false) != null;
        }

        public IList<TI> ForType<TI>() where TI : T
        {
            return ((VariantList<T, TI>)GetListForType(typeof(TI))).implementation;
        }

        protected IList<T> GetListForItem(T item)
        {
            Ensure.That(nameof(item)).IsNotNull(item);

            return GetListForType(item.GetType());
        }

        protected IList<T> GetListForType(Type type, bool throwOnFail = true)
        {
            if (lists.ContainsKey(type))
            {
                return lists[type];
            }

            foreach (var listByType in lists)
            {
                if (listByType.Key.IsAssignableFrom(type))
                {
                    return listByType.Value;
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
            return GetListForItem(item).Contains(item);
        }

        public virtual void Add(T item)
        {
            GetListForItem(item).Add(item);
        }

        public virtual void Clear()
        {
            foreach (var listByType in lists)
            {
                listByType.Value.Clear();
            }
        }

        public virtual bool Remove(T item)
        {
            return GetListForItem(item).Remove(item);
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

            foreach (var listByType in lists)
            {
                var list = listByType.Value;
                list.CopyTo(array, arrayIndex + i);
                i += list.Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private Dictionary<Type, IList<T>>.Enumerator listsEnumerator;
            private T currentItem;
            private IList<T> currentList;
            private int indexInCurrentList;
            private bool exceeded;

            public Enumerator(MergedList<T> merged) : this()
            {
                listsEnumerator = merged.lists.GetEnumerator();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                // We just started, so we're not in a list yet
                if (currentList == null)
                {
                    // Try to find the first list
                    if (listsEnumerator.MoveNext())
                    {
                        // There is at least a list, start with this one
                        currentList = listsEnumerator.Current.Value;

                        if (currentList == null)
                        {
                            throw new InvalidOperationException("Merged sub list is null.");
                        }
                    }
                    else
                    {
                        // There is no list at all, stop
                        currentItem = default(T);
                        exceeded = true;
                        return false;
                    }
                }

                // Check if we're within the current list
                if (indexInCurrentList < currentList.Count)
                {
                    // We are, return this element and move to the next
                    currentItem = currentList[indexInCurrentList];
                    indexInCurrentList++;
                    return true;
                }

                // We're beyond the current list, but there may be more,
                // and because there may be many empty lists, we need to check
                // them all until we find an element, not just the next one
                while (listsEnumerator.MoveNext())
                {
                    currentList = listsEnumerator.Current.Value;
                    indexInCurrentList = 0;

                    if (currentList == null)
                    {
                        throw new InvalidOperationException("Merged sub list is null.");
                    }

                    if (indexInCurrentList < currentList.Count)
                    {
                        currentItem = currentList[indexInCurrentList];
                        indexInCurrentList++;
                        return true;
                    }
                }

                // We're beyond all lists, stop
                currentItem = default(T);
                exceeded = true;
                return false;
            }

            public T Current => currentItem;

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
