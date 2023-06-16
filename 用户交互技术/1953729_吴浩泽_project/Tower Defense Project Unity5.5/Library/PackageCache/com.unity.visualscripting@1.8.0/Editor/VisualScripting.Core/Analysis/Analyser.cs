namespace Unity.VisualScripting
{
    public abstract class Analyser<TTarget, TAnalysis> : Assigner<TTarget, TAnalysis>, IAnalyser
        where TAnalysis : class, IAnalysis, new()
    {
        protected Analyser(GraphReference reference, TTarget target) : base(target, new TAnalysis())
        {
            Ensure.That(nameof(reference)).IsNotNull(reference);

            this.reference = reference;

            // HACK: It makes more sense to think of analysis as reference-bound,
            // however in practice they are context-bound and therefore it is safe
            // (and more importantly faster) to cache the context once for recursive
            // analyses.
            this.context = reference.Context();
        }

        public TAnalysis analysis => assignee;

        IAnalysis IAnalyser.analysis => analysis;

        protected IGraphContext context { get; }

        public GraphReference reference { get; }
    }
}
