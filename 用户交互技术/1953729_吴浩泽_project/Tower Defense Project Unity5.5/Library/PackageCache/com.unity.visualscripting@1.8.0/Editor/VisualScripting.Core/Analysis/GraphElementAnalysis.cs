using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class GraphElementAnalysis : IGraphElementAnalysis
    {
        public List<Warning> warnings { get; set; } = new List<Warning>();
    }
}
