using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public static class ArrayPool<T>
    {
        private static readonly object @lock = new object();
        private static readonly Dictionary<int, Stack<T[]>> free = new Dictionary<int, Stack<T[]>>();
        private static readonly HashSet<T[]> busy = new HashSet<T[]>();

        public static T[] New(int length)
        {
            lock (@lock)
            {
                if (!free.ContainsKey(length))
                {
                    free.Add(length, new Stack<T[]>());
                }

                if (free[length].Count == 0)
                {
                    free[length].Push(new T[length]);
                }

                var array = free[length].Pop();

                busy.Add(array);

                return array;
            }
        }

        public static void Free(T[] array)
        {
            lock (@lock)
            {
                if (!busy.Contains(array))
                {
                    throw new ArgumentException("The array to free is not in use by the pool.", nameof(array));
                }

                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = default(T);
                }

                busy.Remove(array);

                free[array.Length].Push(array);
            }
        }
    }

    public static class XArrayPool
    {
        public static T[] ToArrayPooled<T>(this IEnumerable<T> source)
        {
            var array = ArrayPool<T>.New(source.Count());

            var i = 0;

            foreach (var item in source)
            {
                array[i++] = item;
            }

            return array;
        }

        public static void Free<T>(this T[] array)
        {
            ArrayPool<T>.Free(array);
        }
    }
}
