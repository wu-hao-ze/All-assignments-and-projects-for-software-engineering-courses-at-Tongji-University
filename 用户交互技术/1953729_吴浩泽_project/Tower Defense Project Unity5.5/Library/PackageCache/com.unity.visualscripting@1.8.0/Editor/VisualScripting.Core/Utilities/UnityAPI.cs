using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class UnityAPI
    {
        internal static void Initialize()
        {
            UnityThread.thread = Thread.CurrentThread;
            UnityThread.editorAsync = Async;
            while (UnityThread.pendingQueue.TryDequeue(out var action))
                queue.Enqueue(action);

            EditorApplicationUtility.onModeChange += () => queue = new ConcurrentQueue<Action>();
            EditorApplication.update += ProcessDelegates;
        }

        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(3);

        private static ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        public static void ProcessDelegates()
        {
            while (queue.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public static void Async(Action action)
        {
            if (UnityThread.allowsAPI)
            {
                action();
                return;
            }

            queue.Enqueue(action);
        }

        public static void Await(Action action)
        {
            Await(action, defaultTimeout);
        }

        public static void AwaitForever(Action action)
        {
            Await(action, null);
        }

        private static void Await(Action action, TimeSpan? timeout)
        {
            if (UnityThread.allowsAPI)
            {
                action();
                return;
            }

            var are = new AutoResetEvent(false);
            Exception exception = null;

            queue.Enqueue(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    are.Set();
                }
            });

            if (timeout.HasValue)
            {
                if (!are.WaitOne(timeout.Value))
                {
                    throw new TimeoutException("Time-out exceeded on Unity API thread action delegate. Potential deadlock.");
                }
            }
            else
            {
                are.WaitOne();
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public static T Await<T>(Func<T> func)
        {
            return Await(func, defaultTimeout);
        }

        public static T AwaitForever<T>(Func<T> func)
        {
            return Await(func, null);
        }

        public static T Await<T>(Func<T> func, TimeSpan? timeout)
        {
            if (UnityThread.allowsAPI)
            {
                return func();
            }

            var are = new AutoResetEvent(false);
            Exception exception = null;

            // Define as object for boxing
            object result = default(T);

            queue.Enqueue(() =>
            {
                try
                {
                    result = func();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    are.Set();
                }
            });

            if (timeout.HasValue)
            {
                if (!are.WaitOne(timeout.Value))
                {
                    throw new TimeoutException("Time-out exceeded on Unity API thread function delegate. Potential deadlock.");
                }
            }
            else
            {
                are.WaitOne();
            }

            if (exception != null)
            {
                throw exception;
            }

            return (T)result;
        }
    }
}
