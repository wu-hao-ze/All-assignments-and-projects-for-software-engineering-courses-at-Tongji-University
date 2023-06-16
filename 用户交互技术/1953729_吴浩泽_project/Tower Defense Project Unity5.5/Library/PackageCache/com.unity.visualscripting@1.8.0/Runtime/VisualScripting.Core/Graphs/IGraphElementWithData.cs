namespace Unity.VisualScripting
{
    public interface IGraphElementWithData : IGraphElement
    {
        IGraphElementData CreateData();
    }
}
