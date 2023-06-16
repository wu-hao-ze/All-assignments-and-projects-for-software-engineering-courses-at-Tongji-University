namespace Unity.VisualScripting
{
    public interface IGraphData
    {
        bool TryGetElementData(IGraphElementWithData element, out IGraphElementData data);

        bool TryGetChildGraphData(IGraphParentElement element, out IGraphData data);

        IGraphElementData CreateElementData(IGraphElementWithData element);

        void FreeElementData(IGraphElementWithData element);

        IGraphData CreateChildGraphData(IGraphParentElement element);

        void FreeChildGraphData(IGraphParentElement element);
    }
}
