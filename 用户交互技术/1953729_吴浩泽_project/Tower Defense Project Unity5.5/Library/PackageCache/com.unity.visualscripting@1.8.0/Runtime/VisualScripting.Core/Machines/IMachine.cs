using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IMachine : IGraphRoot, IGraphNester, IAotStubbable
    {
        IGraphData graphData { get; set; }

        GameObject threadSafeGameObject { get; }
    }
}
