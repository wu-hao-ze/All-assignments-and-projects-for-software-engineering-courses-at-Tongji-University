using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IGraph : IDisposable, IPrewarmable, IAotStubbable, ISerializationDepender
    {
        Vector2 pan { get; set; }

        float zoom { get; set; }

        MergedGraphElementCollection elements { get; }

        string title { get; }

        string summary { get; }

        IGraphData CreateData();

        IGraphDebugData CreateDebugData();

        void Instantiate(GraphReference instance);

        void Uninstantiate(GraphReference instance);
    }
}
