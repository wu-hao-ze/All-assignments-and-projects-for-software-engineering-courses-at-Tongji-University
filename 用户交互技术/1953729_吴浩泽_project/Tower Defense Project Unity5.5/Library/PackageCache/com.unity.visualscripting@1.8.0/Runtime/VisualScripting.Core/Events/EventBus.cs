using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class EventBus
    {
        static EventBus()
        {
            events = new Dictionary<EventHook, HashSet<Delegate>>(new EventHookComparer());
        }

        private static readonly Dictionary<EventHook, HashSet<Delegate>> events;

        public static void Register<TArgs>(EventHook hook, Action<TArgs> handler)
        {
            if (!events.TryGetValue(hook, out var handlers))
            {
                handlers = new HashSet<Delegate>();
                events.Add(hook, handlers);
            }

            handlers.Add(handler);
        }

        public static void Unregister(EventHook hook, Delegate handler)
        {
            if (events.TryGetValue(hook, out var handlers))
            {
                if (handlers.Remove(handler))
                {
                    // Free the key references for GC collection
                    if (handlers.Count == 0)
                    {
                        events.Remove(hook);
                    }
                }
            }
        }

        public static void Trigger<TArgs>(EventHook hook, TArgs args)
        {
            HashSet<Action<TArgs>> handlers = null;

            if (events.TryGetValue(hook, out var potentialHandlers))
            {
                foreach (var potentialHandler in potentialHandlers)
                {
                    if (potentialHandler is Action<TArgs> handler)
                    {
                        if (handlers == null)
                        {
                            handlers = HashSetPool<Action<TArgs>>.New();
                        }

                        handlers.Add(handler);
                    }
                }
            }

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    if (!potentialHandlers.Contains(handler))
                    {
                        continue;
                    }

                    handler.Invoke(args);
                }

                handlers.Free();
            }
        }

        public static void Trigger<TArgs>(string name, GameObject target, TArgs args)
        {
            Trigger(new EventHook(name, target), args);
        }

        public static void Trigger(EventHook hook)
        {
            Trigger(hook, new EmptyEventArgs());
        }

        public static void Trigger(string name, GameObject target)
        {
            Trigger(new EventHook(name, target));
        }
    }
}
