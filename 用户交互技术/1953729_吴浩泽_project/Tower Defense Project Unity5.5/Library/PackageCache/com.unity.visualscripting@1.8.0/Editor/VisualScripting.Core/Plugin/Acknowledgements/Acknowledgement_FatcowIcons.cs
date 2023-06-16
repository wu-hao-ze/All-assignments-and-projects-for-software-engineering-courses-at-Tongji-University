namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_FatcowIcons : PluginAcknowledgement
    {
        public Acknowledgement_FatcowIcons(Plugin plugin) : base(plugin) { }

        public override string title => "FatCow Icons";
        public override string author => "FatCow Web Hosting";
        public override int? copyrightYear => 2017;
        public override string url => "https://www.fatcow.com/free-icons";
        public override string licenseName => "Creative Commons Attribution 3.0";
        public override string licenseText => CommonLicenses.CCA3;
    }
}
