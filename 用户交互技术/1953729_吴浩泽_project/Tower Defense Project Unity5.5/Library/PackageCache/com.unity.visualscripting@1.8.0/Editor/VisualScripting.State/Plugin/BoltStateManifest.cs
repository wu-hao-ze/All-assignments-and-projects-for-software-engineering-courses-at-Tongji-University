namespace Unity.VisualScripting
{
    [Plugin(BoltState.ID)]
    public sealed class BoltStateManifest : PluginManifest
    {
        private BoltStateManifest(BoltState plugin) : base(plugin) { }

        public override string name => "Visual Scripting State";
        public override string author => "";
        public override string description => "State-machine based visual scripting.";
        public override SemanticVersion version => PackageVersionUtility.version;
    }
}
