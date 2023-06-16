using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class VariantCollection<TBase, TImplementation> : ICollection<TBase> where TImplementation : TBase
    {
        public VariantCollection(ICollection<TImplementation> implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            this.implementation = implementation;
        }

        public ICollection<TImplementation> implementation { get; private set; }

        public int Count => implementation.Count;

        public bool IsReadOnly => implementation.IsReadOnly;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TBase> GetEnumerator()
        {
            foreach (var i in implementation)
            {
                yield return i;
            }
        }

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
    }
}
