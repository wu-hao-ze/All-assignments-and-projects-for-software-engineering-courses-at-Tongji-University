using System;

namespace Unity.VisualScripting
{
    public static class XEventGraph
    {
        public static void TriggerEventHandler<TArgs>(this GraphReference reference, Func<EventHook, bool> predicate, TArgs args, Func<IGraphParentElement, bool> recurse, bool force)
        {
            Ensure.That(nameof(reference)).IsNotNull(reference);

            foreach (var element in reference.graph.elements)
            {
                if (element is IGraphEventHandler<TArgs> handler && (predicate?.Invoke(handler.GetHook(reference)) ?? true))
                {
                    if (force || handler.IsListening(reference))
                    {
                        handler.Trigger(reference, args);
                    }
                }

                if (element is IGraphParentElement parentElement && recurse(parentElement))
                {
                    reference.ChildReference(parentElement, false, 0)?.TriggerEventHandler(predicate, args, recurse, force);
                }
            }
        }

        public static void TriggerEventHandler<TArgs>(this GraphStack stack, Func<EventHook, bool> predicate, TArgs args, Func<IGraphParentElement, bool> recurse, bool force)
        {
            Ensure.That(nameof(stack)).IsNotNull(stack);

            GraphReference reference = null;

            foreach (var element in stack.graph.elements)
            {
                if (element is IGraphEventHandler<TArgs> handler)
                {
                    if (reference == null)
                    {
                        reference = stack.ToReference();
                    }

                    if (predicate == null || predicate.Invoke(handler.GetHook(reference)))
                    {
                        if (force || handler.IsListening(reference))
                        {
                            handler.Trigger(reference, args);
                        }
                    }
                }

                if (element is IGraphParentElement parentElement && recurse(parentElement))
                {
                    if (stack.TryEnterParentElementUnsafe(parentElement))
                    {
                        stack.TriggerEventHandler(predicate, args, recurse, force);
                        stack.ExitParentElement();
                    }
                }
            }
        }
    }
}
