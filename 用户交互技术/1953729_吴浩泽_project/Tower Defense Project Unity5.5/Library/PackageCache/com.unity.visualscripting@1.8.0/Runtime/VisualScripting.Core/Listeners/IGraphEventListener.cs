namespace Unity.VisualScripting
{
    public interface IGraphEventListener
    {
        void StartListening(GraphStack stack);

        void StopListening(GraphStack stack);

        bool IsListening(GraphPointer pointer);
    }

    public static class XGraphEventListener
    {
        public static void StartListening(this IGraphEventListener listener, GraphReference reference)
        {
            using (var stack = reference.ToStackPooled())
            {
                listener.StartListening(stack);
            }
        }

        public static void StopListening(this IGraphEventListener listener, GraphReference reference)
        {
            using (var stack = reference.ToStackPooled())
            {
                listener.StopListening(stack);
            }
        }

        public static bool IsHierarchyListening(GraphReference reference)
        {
            using (var stack = reference.ToStackPooled())
            {
                // Check if any parent of the graph is not listening
                while (stack.isChild)
                {
                    var parent = stack.parent;

                    // Exit the parent first, as the IsListening method expects to be at the level of the parent
                    stack.ExitParentElement();

                    if (parent is IGraphEventListener listener && !listener.IsListening(stack))
                    {
                        return false;
                    }
                }

                // Check if the root graph is not listening
                if (stack.graph is IGraphEventListener graphListener && !graphListener.IsListening(stack))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
