namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_SQLite : PluginAcknowledgement
    {
        public Acknowledgement_SQLite(Plugin plugin) : base(plugin) { }

        public override string title => "SQLite .NET";
        public override string author => "Roberto Huertas";
        public override int? copyrightYear => 2014;
        public override string url => "https://github.com/codecoding/SQLite4Unity3d";
        public override string licenseName => "MIT";
        public override string licenseText => CommonLicenses.MIT;
    }
}
