namespace Unity.VisualScripting
{
    public class CanvasProvider : SingleDecoratorProvider<IGraph, ICanvas, CanvasAttribute>
    {
        static CanvasProvider()
        {
            instance = new CanvasProvider();
        }

        public static CanvasProvider instance { get; }

        protected override bool cache => true;

        public override bool IsValid(IGraph graph)
        {
            return true;
        }
    }

    public static class XCanvasProvider
    {
        public static ICanvas Canvas(this IGraph graph)
        {
            return CanvasProvider.instance.GetDecorator(graph);
        }

        public static TCanvas Canvas<TCanvas>(this IGraph graph) where TCanvas : ICanvas
        {
            return CanvasProvider.instance.GetDecorator<TCanvas>(graph);
        }

        public static IWidget Widget(this ICanvas context, IGraphItem item)
        {
            return context.widgetProvider.GetDecorator(item);
        }

        public static TWidget Widget<TWidget>(this ICanvas context, IGraphItem item) where TWidget : IWidget
        {
            return context.widgetProvider.GetDecorator<TWidget>(item);
        }

        public static IGraphElementWidget Widget(this ICanvas context, IGraphElement element)
        {
            return (IGraphElementWidget)context.widgetProvider.GetDecorator(element);
        }

        public static TWidget Widget<TWidget>(this ICanvas context, IGraphElement element) where TWidget : IGraphElementWidget
        {
            return context.widgetProvider.GetDecorator<TWidget>(element);
        }
    }
}
