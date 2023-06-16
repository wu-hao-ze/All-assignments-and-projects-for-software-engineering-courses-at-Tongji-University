using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    [Analyser(typeof(GraphOutput))]
    public class GraphOutputAnalyser : UnitAnalyser<GraphOutput>
    {
        public GraphOutputAnalyser(GraphReference reference, GraphOutput unit) : base(reference, unit) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (unit.graph != null)
            {
                foreach (var definitionWarning in UnitPortDefinitionUtility.Warnings(unit.graph, LinqUtility.Concat<IUnitPortDefinition>(unit.graph.controlOutputDefinitions, unit.graph.valueOutputDefinitions)))
                {
                    yield return definitionWarning;
                }

                if (unit.graph.units.Count(unit => unit is GraphOutput) > 1)
                {
                    yield return Warning.Caution("Multiple output nodes in the same graph. Only one of them will be used.");
                }
            }
        }
    }
}
