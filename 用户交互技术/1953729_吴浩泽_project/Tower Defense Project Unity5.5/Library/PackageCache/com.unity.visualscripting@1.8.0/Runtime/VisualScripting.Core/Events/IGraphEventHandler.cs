namespace Unity.VisualScripting
{
    public interface IGraphEventHandler<TArgs>
    {
        EventHook GetHook(GraphReference reference);

        void Trigger(GraphReference reference, TArgs args);

        bool IsListening(GraphPointer pointer);
    }
}
