namespace Unity.VisualScripting
{
    [Plugin(BoltProduct.ID)]
    internal class Acknowledgement_NCalc : PluginAcknowledgement
    {
        public Acknowledgement_NCalc(Plugin plugin) : base(plugin) { }

        public override string title => "NCalc";
        public override string author => "Sébastien Ros";
        public override string url => "https://ncalc.codeplex.com/";
        public override string licenseName => "MIT";
        public override string licenseText => CommonLicenses.MIT;
    }
}
