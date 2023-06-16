namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_DeepCopy : PluginAcknowledgement
    {
        public Acknowledgement_DeepCopy(Plugin plugin) : base(plugin) { }

        public override string title => "Deep Copy";
        public override string author => "Alexey Burtsev";
        public override int? copyrightYear => 2014;
        public override string url => "https://github.com/Burtsev-Alexey/net-object-deep-copy";
        public override string licenseName => "MIT";
        public override string licenseText => CommonLicenses.MIT;
    }
}
