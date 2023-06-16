using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public static class HashSetPool<T>
    {
        private static readonly object @lock = new object();
        private static readonly Stack<HashSet<T>> free = new Stack<HashSet<T>>();
        private static readonly HashSet<HashSet<T>> busy = new HashSet<HashSet<T>>();

        public static HashSet<T> New()
        {
            lock (@lock)
            {
                if (free.Count == 0)
                {
                    free.Push(new HashSet<T>());
                }

                var hashSet = free.Pop();

                busy.Add(hashSet);

                return hashSet;
            }
        }

        public static void Free(HashSet<T> hashSet)
        {
            lock (@lock)
            {
                if (!busy.Remove(hashSet))
                {
                    throw new ArgumentException("The hash set to free is not in use by the pool.", nameof(hashSet));
                }

                hashSet.Clear();

                free.Push(hashSet);
            }
        }
    }

    public static class XHashSetPool
    {
        public static HashSet<T> ToHashSetPooled<T>(this IEnumerable<T> source)
        {
            var hashSet = HashSetPool<T>.New();

            foreach (var item in source)
            {
                hashSet.Add(item);
            }

            return hashSet;
        }

        public static void Free<T>(this HashSet<T> hashSet)
        {
            HashSetPool<T>.Free(hashSet);
        }
    }
}
