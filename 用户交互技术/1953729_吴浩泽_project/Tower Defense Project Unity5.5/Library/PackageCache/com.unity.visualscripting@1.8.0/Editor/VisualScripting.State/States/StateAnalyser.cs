using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Analyser(typeof(IState))]
    public class StateAnalyser<TState> : Analyser<TState, StateAnalysis>
        where TState : class, IState
    {
        public StateAnalyser(GraphReference reference, TState target) : base(reference, target) { }

        public TState state => target;

        [Assigns]
        protected virtual bool IsEntered()
        {
            using (var recursion = Recursion.New(1))
            {
                return IsEntered(state, recursion);
            }
        }

        [Assigns]
        protected virtual IEnumerable<Warning> Warnings()
        {
            if (!IsEntered())
            {
                yield return Warning.Info("State is never entered.");
            }
        }

        private bool IsEntered(IState state, Recursion recursion)
        {
            if (state.isStart)
            {
                return true;
            }

            if (!recursion?.TryEnter(state) ?? false)
            {
                return false;
            }

            foreach (var incomingTransition in state.incomingTransitions)
            {
                if (IsEntered(incomingTransition.source, recursion) && incomingTransition.Analysis<StateTransitionAnalysis>(context).isTraversed)
                {
                    recursion?.Exit(state);
                    return true;
                }
            }

            recursion?.Exit(state);

            return false;
        }
    }
}
