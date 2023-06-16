namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    public sealed class BoltFlowManifest : PluginManifest
    {
        private BoltFlowManifest(BoltFlow plugin) : base(plugin) { }

        public override string name => "Visual Scripting Flow";
        public override string author => "";
        public override string description => "Flow-graph based visual scripting.";
        public override SemanticVersion version => PackageVersionUtility.version;
    }
}
