using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class GraphContextExtensionProvider : MultiDecoratorProvider<IGraphContext, IGraphContextExtension, GraphContextExtensionAttribute>
    {
        static GraphContextExtensionProvider()
        {
            instance = new GraphContextExtensionProvider();
        }

        public static GraphContextExtensionProvider instance { get; }
    }

    public static class XCanvasExtensionProvider
    {
        public static IEnumerable<IGraphContextExtension> Extensions(this IGraphContext context)
        {
            return GraphContextExtensionProvider.instance.GetDecorators(context);
        }
    }
}
