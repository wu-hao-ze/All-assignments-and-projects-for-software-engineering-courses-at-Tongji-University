using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public static class ListPool<T>
    {
        private static readonly object @lock = new object();
        private static readonly Stack<List<T>> free = new Stack<List<T>>();
        private static readonly HashSet<List<T>> busy = new HashSet<List<T>>();

        public static List<T> New()
        {
            lock (@lock)
            {
                if (free.Count == 0)
                {
                    free.Push(new List<T>());
                }

                var array = free.Pop();

                busy.Add(array);

                return array;
            }
        }

        public static void Free(List<T> list)
        {
            lock (@lock)
            {
                if (!busy.Contains(list))
                {
                    throw new ArgumentException("The list to free is not in use by the pool.", nameof(list));
                }

                list.Clear();

                busy.Remove(list);

                free.Push(list);
            }
        }
    }

    public static class XListPool
    {
        public static List<T> ToListPooled<T>(this IEnumerable<T> source)
        {
            var list = ListPool<T>.New();

            foreach (var item in source)
            {
                list.Add(item);
            }

            return list;
        }

        public static void Free<T>(this List<T> list)
        {
            ListPool<T>.Free(list);
        }
    }
}
