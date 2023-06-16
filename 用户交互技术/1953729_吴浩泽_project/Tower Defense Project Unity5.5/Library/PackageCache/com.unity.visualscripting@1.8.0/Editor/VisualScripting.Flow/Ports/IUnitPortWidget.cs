using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IUnitPortWidget : IWidget
    {
        IUnitPort port { get; }
        float y { set; }
        Rect handlePosition { get; }
        float GetInnerWidth();
        float GetHeight();
        bool willDisconnect { get; }
    }
}
