using System.IO;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    public class BoltFlowPaths : PluginPaths
    {
        public BoltFlowPaths(Plugin plugin) : base(plugin) { }

        public string unitOptions => Path.Combine(transientGenerated, "UnitOptions.db");
    }
}
