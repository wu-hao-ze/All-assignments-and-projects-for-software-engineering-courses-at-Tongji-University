using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public struct NoAllocEnumerator<T> : IEnumerator<T>
    {
        private readonly IList<T> list;
        private int index;
        private T current;
        private bool exceeded;

        public NoAllocEnumerator(IList<T> list) : this()
        {
            this.list = list;
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            if (index < list.Count)
            {
                current = list[index];
                index++;
                return true;
            }
            else
            {
                index = list.Count + 1;
                current = default(T);
                exceeded = true;
                return false;
            }
        }

        public T Current => current;

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
