using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IGraphWithVariables : IGraph
    {
        VariableDeclarations variables { get; }

        IEnumerable<string> GetDynamicVariableNames(VariableKind kind, GraphReference reference);
    }
}
