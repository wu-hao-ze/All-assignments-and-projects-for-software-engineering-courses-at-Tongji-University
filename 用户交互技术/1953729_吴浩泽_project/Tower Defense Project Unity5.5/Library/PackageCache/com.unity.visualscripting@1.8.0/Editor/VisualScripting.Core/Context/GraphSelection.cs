using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public sealed class GraphSelection : ISet<IGraphElement>
    {
        public GraphSelection() : base()
        {
            set = new HashSet<IGraphElement>();
        }

        public event Action changed;

        private readonly HashSet<IGraphElement> set;

        public int Count => set.Count;

        public bool IsReadOnly => false;

        private void OnChange()
        {
            changed?.Invoke();
        }

        IEnumerator<IGraphElement> IEnumerable<IGraphElement>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public HashSet<IGraphElement>.Enumerator GetEnumerator()
        {
            return set.GetEnumerator();
        }

        public void Select(IGraphElement item)
        {
            Clear();
            Add(item);
        }

        public void Select(IEnumerable<IGraphElement> items)
        {
            Clear();
            UnionWith(items);
        }

        public bool Contains(IGraphElement item)
        {
            Ensure.That(nameof(item)).IsNotNull(item);

            return set.Contains(item);
        }

        public bool Add(IGraphElement item)
        {
            Ensure.That(nameof(item)).IsNotNull(item);

            if (set.Add(item))
            {
                OnChange();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(IGraphElement item)
        {
            Ensure.That(nameof(item)).IsNotNull(item);

            if (set.Remove(item))
            {
                OnChange();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            if (set.Count == 0)
            {
                return;
            }

            set.Clear();
            OnChange();
        }

        #region Set Logic

        public void ExceptWith(IEnumerable<IGraphElement> other)
        {
            var countBefore = set.Count;

            set.ExceptWith(other);

            if (countBefore != set.Count)
            {
                OnChange();
            }
        }

        public void IntersectWith(IEnumerable<IGraphElement> other)
        {
            var countBefore = set.Count;

            set.IntersectWith(other);

            if (countBefore != set.Count)
            {
                OnChange();
            }
        }

        public void SymmetricExceptWith(IEnumerable<IGraphElement> other)
        {
            var countBefore = set.Count;

            set.SymmetricExceptWith(other);

            if (countBefore != set.Count)
            {
                OnChange();
            }
        }

        public void UnionWith(IEnumerable<IGraphElement> other)
        {
            var countBefore = set.Count;

            set.UnionWith(other);

            if (countBefore != set.Count)
            {
                OnChange();
            }
        }

        public bool IsProperSubsetOf(IEnumerable<IGraphElement> other)
        {
            return set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<IGraphElement> other)
        {
            return set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<IGraphElement> other)
        {
            return set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<IGraphElement> other)
        {
            return set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<IGraphElement> other)
        {
            return set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<IGraphElement> other)
        {
            return set.SetEquals(other);
        }

        public int RemoveWhere(Predicate<IGraphElement> match)
        {
            return set.RemoveWhere(match);
        }

        #endregion

        #region ICollection

        void ICollection<IGraphElement>.Add(IGraphElement item)
        {
            if (set.Add(item))
            {
                OnChange();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(IGraphElement[] array, int arrayIndex)
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

            foreach (var item in this)
            {
                array[i + arrayIndex] = item;
                i++;
            }
        }

        #endregion
    }
}
