using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public abstract class StateTransitionAnalyser<TStateTransition> : Analyser<TStateTransition, StateTransitionAnalysis>
        where TStateTransition : IStateTransition
    {
        protected StateTransitionAnalyser(GraphReference reference, TStateTransition target) : base(reference, target) { }

        public TStateTransition transition => target;

        [Assigns]
        protected virtual bool IsTraversed()
        {
            return true;
        }

        [Assigns]
        protected virtual IEnumerable<Warning> Warnings()
        {
            if (!IsTraversed())
            {
                yield return Warning.Info("Transition is never traversed.");
            }
        }
    }
}
