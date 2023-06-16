namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_DotNetZip : PluginAcknowledgement
    {
        public Acknowledgement_DotNetZip(Plugin plugin) : base(plugin) { }

        public override string title => "DotNetZip";
        public override string author => "Ionic";
        public override int? copyrightYear => 2017;
        public override string url => "https://dotnetzip.codeplex.com/";
        public override string licenseName => "Microsoft Public License";
        public override string licenseText => CommonLicenses.MSPL;
    }
}
