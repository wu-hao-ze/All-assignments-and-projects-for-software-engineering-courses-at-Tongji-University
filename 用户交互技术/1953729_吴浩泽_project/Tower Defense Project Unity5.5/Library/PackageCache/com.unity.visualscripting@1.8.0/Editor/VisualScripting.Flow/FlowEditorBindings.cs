namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class FlowEditorBindings
    {
        static FlowEditorBindings()
        {
            Flow.isInspectedBinding = IsInspected;
        }

        private static bool IsInspected(GraphPointer pointer)
        {
            Ensure.That(nameof(pointer)).IsNotNull(pointer);

            foreach (var graphWindow in GraphWindow.tabsNoAlloc)
            {
                if (graphWindow.reference?.InstanceEquals(pointer) ?? false)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
