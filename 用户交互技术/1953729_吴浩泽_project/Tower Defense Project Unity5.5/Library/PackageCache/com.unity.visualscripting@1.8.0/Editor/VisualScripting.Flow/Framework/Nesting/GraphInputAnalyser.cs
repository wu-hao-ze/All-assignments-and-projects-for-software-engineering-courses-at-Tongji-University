using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Analyser(typeof(GraphInput))]
    public class GraphInputAnalyser : UnitAnalyser<GraphInput>
    {
        public GraphInputAnalyser(GraphReference reference, GraphInput unit) : base(reference, unit) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (unit.graph != null)
            {
                foreach (var definitionWarning in UnitPortDefinitionUtility.Warnings(unit.graph, LinqUtility.Concat<IUnitPortDefinition>(unit.graph.controlInputDefinitions, unit.graph.valueInputDefinitions)))
                {
                    yield return definitionWarning;
                }

                var inputs = unit.graph.units.Where(u => u is GraphInput).ToList();
                if (inputs.Count > 1)
                {
                    var firstInput = inputs[0];
                    if (unit != firstInput)
                    {
                        var graphName = string.IsNullOrEmpty(unit.graph.title) ? nameof(SubgraphUnit) : unit.graph.title;
                        Debug.LogWarning($"Only one Input node can be used and will execute in {graphName}.");
                        yield return Warning.Caution("Only one Input node can be used and will execute.");
                    }
                }
            }
        }
    }
}
