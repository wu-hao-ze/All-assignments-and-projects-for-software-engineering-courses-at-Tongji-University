namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Acknowledgement_ReorderableList : PluginAcknowledgement
    {
        public Acknowledgement_ReorderableList(Plugin plugin) : base(plugin) { }

        public override string title => "Reorderable List";
        public override string author => "Rotorz Limited";
        public override string url => "https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity";
        public override string licenseName => "MIT";
        public override string licenseText => CommonLicenses.MIT;
    }
}
