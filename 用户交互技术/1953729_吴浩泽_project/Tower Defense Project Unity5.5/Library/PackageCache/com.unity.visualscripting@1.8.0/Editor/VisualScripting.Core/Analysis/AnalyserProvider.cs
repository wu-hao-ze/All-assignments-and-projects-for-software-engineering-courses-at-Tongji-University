using System;

namespace Unity.VisualScripting
{
    public sealed class AnalyserProvider : SingleDecoratorProvider<object, IAnalyser, AnalyserAttribute>
    {
        protected override bool cache => true;

        public GraphReference reference { get; }

        public AnalyserProvider(GraphReference reference)
        {
            this.reference = reference;
        }

        protected override IAnalyser CreateDecorator(Type decoratorType, object decorated)
        {
            return (IAnalyser)decoratorType.Instantiate(true, reference, decorated);
        }

        public override bool IsValid(object analyzed)
        {
            return !analyzed.IsUnityNull();
        }

        public void Analyze(object analyzed)
        {
            GetDecorator(analyzed).isDirty = true;
        }

        public void AnalyzeAll()
        {
            foreach (var analyser in decorators.Values)
            {
                analyser.isDirty = true;
            }
        }
    }

    public static class XAnalyserProvider
    {
        // Analysis are conceptually reference-bound, but practically context-bound,
        // so it's faster to avoid the reference-to-context lookup if we can avoid it.

        public static IAnalyser Analyser(this object target, IGraphContext context)
        {
            return context.analyserProvider.GetDecorator(target);
        }

        public static TAnalyser Analyser<TAnalyser>(this object target, IGraphContext context) where TAnalyser : IAnalyser
        {
            return context.analyserProvider.GetDecorator<TAnalyser>(target);
        }

        public static IAnalysis Analysis(this object target, IGraphContext context)
        {
            var analyser = target.Analyser(context);
            analyser.Validate();
            return analyser.analysis;
        }

        public static TAnalysis Analysis<TAnalysis>(this object target, IGraphContext context) where TAnalysis : IAnalysis
        {
            return (TAnalysis)target.Analysis(context);
        }

        // Shortcuts, but the above are faster because Context doesn't have to be looked up

        public static IAnalyser Analyser(this object target, GraphReference reference)
        {
            return target.Analyser(reference.Context());
        }

        public static TAnalyser Analyser<TAnalyser>(this object target, GraphReference reference) where TAnalyser : IAnalyser
        {
            return target.Analyser<TAnalyser>(reference.Context());
        }

        public static IAnalysis Analysis(this object target, GraphReference reference)
        {
            return target.Analysis(reference.Context());
        }

        public static TAnalysis Analysis<TAnalysis>(this object target, GraphReference reference) where TAnalysis : IAnalysis
        {
            return target.Analysis<TAnalysis>(reference.Context());
        }
    }
}
