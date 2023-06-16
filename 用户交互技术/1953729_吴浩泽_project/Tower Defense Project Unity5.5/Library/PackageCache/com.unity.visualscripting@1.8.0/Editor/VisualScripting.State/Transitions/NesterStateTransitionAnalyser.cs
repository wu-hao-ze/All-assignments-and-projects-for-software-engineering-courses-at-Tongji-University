using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Analyser(typeof(INesterStateTransition))]
    public class NesterStateTransitionAnalyser<TGraphNesterStateTransition> : StateTransitionAnalyser<TGraphNesterStateTransition>
        where TGraphNesterStateTransition : class, INesterStateTransition
    {
        public NesterStateTransitionAnalyser(GraphReference reference, TGraphNesterStateTransition transition) : base(reference, transition) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (transition.childGraph == null)
            {
                yield return Warning.Caution("Missing transition graph.");
            }

            if (transition.nest.hasBackgroundEmbed)
            {
                yield return Warning.Caution("Background embed graph detected.");
            }
        }
    }
}
