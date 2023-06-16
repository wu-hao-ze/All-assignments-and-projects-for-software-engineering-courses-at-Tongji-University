using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IGraphDebugData
    {
        IGraphElementDebugData GetOrCreateElementData(IGraphElementWithDebugData element);

        IGraphDebugData GetOrCreateChildGraphData(IGraphParentElement element);

        IEnumerable<IGraphElementDebugData> elementsData { get; }
    }
}
