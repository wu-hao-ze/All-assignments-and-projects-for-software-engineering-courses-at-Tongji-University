using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class GraphsExceptionUtility
    {
        // Note: Checking hasDebugData here instead of enableDebug,
        // because we always want exceptions to register, even if
        // background debug is disabled.

        private const string handledKey = "Bolt.Core.Handled";

        public static Exception GetException(this IGraphElementWithDebugData element, GraphPointer pointer)
        {
            if (!pointer.hasDebugData)
            {
                return null;
            }

            var debugData = pointer.GetElementDebugData<IGraphElementDebugData>(element);

            return debugData.runtimeException;
        }

        public static void SetException(this IGraphElementWithDebugData element, GraphPointer pointer, Exception ex)
        {
            if (!pointer.hasDebugData)
            {
                return;
            }

            var debugData = pointer.GetElementDebugData<IGraphElementDebugData>(element);

            debugData.runtimeException = ex;
        }

        public static void HandleException(this IGraphElementWithDebugData element, GraphPointer pointer, Exception ex)
        {
            Ensure.That(nameof(ex)).IsNotNull(ex);

            if (pointer == null)
            {
                Debug.LogError("Caught exception with null graph pointer (flow was likely disposed):\n" + ex);
                return;
            }

            var reference = pointer.AsReference();

            if (!ex.HandledIn(reference))
            {
                element.SetException(pointer, ex);
            }

            while (reference.isChild)
            {
                var parentElement = reference.parentElement;
                reference = reference.ParentReference(true);

                if (parentElement is IGraphElementWithDebugData debuggableParentElement)
                {
                    if (!ex.HandledIn(reference))
                    {
                        debuggableParentElement.SetException(reference, ex);
                    }
                }
            }
        }

        private static bool HandledIn(this Exception ex, GraphReference reference)
        {
            Ensure.That(nameof(ex)).IsNotNull(ex);

            if (!ex.Data.Contains(handledKey))
            {
                ex.Data.Add(handledKey, new HashSet<GraphReference>());
            }

            var handled = (HashSet<GraphReference>)ex.Data[handledKey];

            if (handled.Contains(reference))
            {
                return true;
            }
            else
            {
                handled.Add(reference);
                return false;
            }
        }
    }
}
