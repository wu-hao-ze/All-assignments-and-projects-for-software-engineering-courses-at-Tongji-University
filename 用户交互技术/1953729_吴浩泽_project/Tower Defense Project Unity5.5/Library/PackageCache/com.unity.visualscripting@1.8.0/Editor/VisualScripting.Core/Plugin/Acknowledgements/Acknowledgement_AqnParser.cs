namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_AqnParser : PluginAcknowledgement
    {
        public Acknowledgement_AqnParser(Plugin plugin) : base(plugin) { }

        public override string title => "AQN Parser";
        public override string author => "Christophe Bertrand";
        public override int? copyrightYear => 2013;
        public override string url => "https://www.codeproject.com/Tips/624300/AssemblyQualifiedName-Parser";
        public override string licenseName => "Microsoft Public License";
        public override string licenseText => CommonLicenses.MSPL;
    }
}
