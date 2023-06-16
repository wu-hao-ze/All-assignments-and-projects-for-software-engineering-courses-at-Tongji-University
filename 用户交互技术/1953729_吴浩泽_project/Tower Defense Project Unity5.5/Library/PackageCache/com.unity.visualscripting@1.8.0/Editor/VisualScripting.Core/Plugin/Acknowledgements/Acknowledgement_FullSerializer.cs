namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_FullSerializer : PluginAcknowledgement
    {
        public Acknowledgement_FullSerializer(Plugin plugin) : base(plugin) { }

        public override string title => "Full Serializer";
        public override string author => "Jacob Dufault";
        public override int? copyrightYear => 2017;
        public override string url => "https://www.fatcow.com/free-icons";
        public override string licenseName => "MIT";
        public override string licenseText => CommonLicenses.MIT;
    }
}
