using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.VisualScripting
{
    public static class EditorLinqUtility
    {
        public static IEnumerable<T> Cancellable<T>(this IEnumerable<T> source, CancellationToken cancellation)
        {
            foreach (var item in source)
            {
                yield return item;
                cancellation.ThrowIfCancellationRequested();
            }
        }

        public static IEnumerable<T> Cancellable<T>(this IEnumerable<T> source, CancellationToken cancellation, Action cancel)
        {
            Ensure.That(nameof(cancel)).IsNotNull(cancel);

            foreach (var item in source)
            {
                yield return item;

                if (cancellation.IsCancellationRequested)
                {
                    cancel();
                }
            }
        }
    }
}
