using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public class GraphDebugDataProvider
    {
        static GraphDebugDataProvider()
        {
            GraphPointer.fetchRootDebugDataBinding = FetchRootDebugData;
        }

        private static IGraphDebugData FetchRootDebugData(IGraphRoot root)
        {
            if (!rootDatas.TryGetValue(root, out var rootData))
            {
                rootData = new GraphDebugData(root.childGraph);
                rootDatas.Add(root, rootData);
            }

            return rootData;
        }

        private static Dictionary<IGraphRoot, IGraphDebugData> rootDatas = new Dictionary<IGraphRoot, IGraphDebugData>();
    }
}
