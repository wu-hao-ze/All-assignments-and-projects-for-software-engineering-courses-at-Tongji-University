using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class FuzzyOptionTreeExtensionProvider : MultiDecoratorProvider<IFuzzyOptionTree, IFuzzyOptionTree, FuzzyOptionTreeExtensionAttribute>
    {
        static FuzzyOptionTreeExtensionProvider()
        {
            instance = new FuzzyOptionTreeExtensionProvider();
        }

        public static FuzzyOptionTreeExtensionProvider instance { get; }
    }

    public static class XFuzzyOptionTreeExtensionProvider
    {
        public static IEnumerable<IFuzzyOptionTree> Extensions(this IFuzzyOptionTree optionTree)
        {
            return FuzzyOptionTreeExtensionProvider.instance.GetDecorators(optionTree);
        }
    }
}
