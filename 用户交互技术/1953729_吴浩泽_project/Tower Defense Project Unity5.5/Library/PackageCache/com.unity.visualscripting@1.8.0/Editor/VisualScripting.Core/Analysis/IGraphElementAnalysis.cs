using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IGraphElementAnalysis : IAnalysis
    {
        List<Warning> warnings { get; }
    }
}
