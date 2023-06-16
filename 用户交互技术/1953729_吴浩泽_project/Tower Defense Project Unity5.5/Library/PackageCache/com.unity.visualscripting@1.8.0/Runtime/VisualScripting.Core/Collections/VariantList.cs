using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class VariantList<TBase, TImplementation> : IList<TBase> where TImplementation : TBase
    {
        public VariantList(IList<TImplementation> implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            this.implementation = implementation;
        }

        public TBase this[int index]
        {
            get
            {
                return implementation[index];
            }
            set
            {
                if (!(value is TImplementation))
                {
                    throw new NotSupportedException();
                }

                implementation[index] = (TImplementation)value;
            }
        }

        public IList<TImplementation> implementation { get; private set; }

        public int Count => implementation.Count;

        public bool IsReadOnly => implementation.IsReadOnly;

        public void Add(TBase item)
        {
            if (!(item is TImplementation))
            {
                throw new NotSupportedException();
            }

            implementation.Add((TImplementation)item);
        }

        public void Clear()
        {
            implementation.Clear();
        }

        public bool Contains(TBase item)
        {
            if (!(item is TImplementation))
            {
                throw new NotSupportedException();
            }

            return implementation.Contains((TImplementation)item);
        }

        public bool Remove(TBase item)
        {
            if (!(item is TImplementation))
            {
                throw new NotSupportedException();
            }

            return implementation.Remove((TImplementation)item);
        }

        public void CopyTo(TBase[] array, int arrayIndex)
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

            var implementationArray = new TImplementation[Count];
            implementation.CopyTo(implementationArray, 0);

            for (var i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = implementationArray[i];
            }
        }

        public int IndexOf(TBase item)
        {
            if (!(item is TImplementation))
            {
                throw new NotSupportedException();
            }

            return implementation.IndexOf((TImplementation)item);
        }

        public void Insert(int index, TBase item)
        {
            if (!(item is TImplementation))
            {
                throw new NotSupportedException();
            }

            implementation.Insert(index, (TImplementation)item);
        }

        public void RemoveAt(int index)
        {
            implementation.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TBase> IEnumerable<TBase>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public NoAllocEnumerator<TBase> GetEnumerator()
        {
            return new NoAllocEnumerator<TBase>(this);
        }
    }
}
