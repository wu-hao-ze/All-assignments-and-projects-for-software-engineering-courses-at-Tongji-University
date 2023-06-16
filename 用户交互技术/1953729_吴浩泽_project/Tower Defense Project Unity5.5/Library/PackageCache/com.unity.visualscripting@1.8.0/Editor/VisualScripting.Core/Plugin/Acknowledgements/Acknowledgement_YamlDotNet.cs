namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_YamlDotNet : PluginAcknowledgement
    {
        public Acknowledgement_YamlDotNet(Plugin plugin) : base(plugin) { }

        public override string title => "YamlDotNet";
        public override string author => "Antoine Aubry";
        public override string url => "http://aaubry.net/pages/yamldotnet.html";
        public override string licenseName => "MIT";
        public override string licenseText => CommonLicenses.MIT;
    }
}
