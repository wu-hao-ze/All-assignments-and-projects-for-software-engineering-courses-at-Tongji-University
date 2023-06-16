using System;

namespace Unity.VisualScripting
{
    public class WidgetProvider : SingleDecoratorProvider<IGraphItem, IWidget, WidgetAttribute>
    {
        public ICanvas canvas { get; }

        public WidgetProvider(ICanvas canvas)
        {
            Ensure.That(nameof(canvas)).IsNotNull(canvas);

            this.canvas = canvas;
        }

        protected override bool cache => true;

        public override bool IsValid(IGraphItem item)
        {
            return item.graph == canvas.graph;
        }

        protected override IWidget CreateDecorator(Type widgetType, IGraphItem item)
        {
            return (IWidget)widgetType.Instantiate(false, canvas, item);
        }
    }
}
