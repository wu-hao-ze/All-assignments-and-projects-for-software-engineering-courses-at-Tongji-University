using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class GraphNesterDescriptor
    {
        public static string Title(IGraphNester nester)
        {
            return Title(nester, string.Empty);
        }

        public static string Title(IGraphNester nester, string defaultName)
        {
            var graph = nester.childGraph;

            if (!StringUtility.IsNullOrWhiteSpace(graph?.title))
            {
                return graph?.title;
            }

            if (nester.nest.source == GraphSource.Macro && (UnityObject)nester.nest.macro != null)
            {
                var macroName = ((UnityObject)nester.nest.macro).name;
                return BoltCore.Configuration.humanNaming ? macroName.Prettify() : macroName;
            }

            return !string.IsNullOrEmpty(defaultName) ? defaultName : nester.GetType().HumanName();
        }

        public static string Summary(IGraphNester nester)
        {
            var graph = nester.childGraph;

            if (!StringUtility.IsNullOrWhiteSpace(graph?.summary))
            {
                return graph?.summary;
            }

            return nester.GetType().Summary();
        }
    }
}
