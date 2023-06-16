using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Analyser(typeof(INesterState))]
    public class NesterStateAnalyser<TNesterState> : StateAnalyser<TNesterState>
        where TNesterState : class, INesterState
    {
        public NesterStateAnalyser(GraphReference reference, TNesterState state) : base(reference, state) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (state.childGraph == null)
            {
                yield return Warning.Caution("Missing nested graph.");
            }

            if (state.nest.hasBackgroundEmbed)
            {
                yield return Warning.Caution("Background embed graph detected.");
            }
        }
    }
}
