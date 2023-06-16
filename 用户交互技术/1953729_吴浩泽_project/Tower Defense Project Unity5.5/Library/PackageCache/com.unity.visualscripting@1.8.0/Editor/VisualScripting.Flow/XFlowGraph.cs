using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public static class XFlowGraph
    {
        public static IEnumerable<IUnit> GetUnitsRecursive(this FlowGraph flowGraph, Recursion recursion)
        {
            Ensure.That(nameof(flowGraph)).IsNotNull(flowGraph);

            if (!recursion?.TryEnter(flowGraph) ?? false)
            {
                yield break;
            }

            foreach (var unit in flowGraph.units)
            {
                yield return unit;

                var nestedGraph = (unit as SubgraphUnit)?.nest.graph;

                if (nestedGraph != null)
                {
                    foreach (var nestedUnit in GetUnitsRecursive(nestedGraph, recursion))
                    {
                        yield return nestedUnit;
                    }
                }
            }

            recursion?.Exit(flowGraph);
        }
    }
}
