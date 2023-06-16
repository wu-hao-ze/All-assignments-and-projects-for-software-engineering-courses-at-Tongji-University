namespace Unity.VisualScripting
{
    public interface IGraphRoot : IGraphParent
    {
        GraphPointer GetReference();
    }
}
