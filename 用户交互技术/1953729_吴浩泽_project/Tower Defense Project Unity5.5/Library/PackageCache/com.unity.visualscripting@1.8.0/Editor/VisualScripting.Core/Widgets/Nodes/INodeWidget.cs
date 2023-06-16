using UnityEngine;

namespace Unity.VisualScripting
{
    public interface INodeWidget : IGraphElementWidget
    {
        Rect outerPosition { get; }
        Rect edgePosition { get; }
        Rect innerPosition { get; }
    }
}
