using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    [Analyser(typeof(FlowStateTransition))]
    public class FlowStateTransitionAnalyser : NesterStateTransitionAnalyser<FlowStateTransition>
    {
        public FlowStateTransitionAnalyser(GraphReference reference, FlowStateTransition transition) : base(reference, transition) { }

        protected override bool IsTraversed()
        {
            var graph = transition.nest.graph;

            if (graph == null)
            {
                return false;
            }

            using (var recursion = Recursion.New(1))
            {
                foreach (var trigger in graph.GetUnitsRecursive(recursion).OfType<TriggerStateTransition>())
                {
                    if (trigger.Analysis<UnitAnalysis>(context).isEntered)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            var graph = transition.nest.graph;

            if (graph == null)
            {
                yield break;
            }

            using (var recursion = Recursion.New(1))
            {
                if (!graph.GetUnitsRecursive(recursion).OfType<IEventUnit>().Any())
                {
                    yield return Warning.Caution("Transition graph is missing an event.");
                }
            }

            using (var recursion = Recursion.New(1))
            {
                if (!graph.GetUnitsRecursive(recursion).OfType<TriggerStateTransition>().Any())
                {
                    yield return Warning.Caution("Transition graph is missing a trigger unit.");
                }
            }
        }
    }
}
