namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_3_0_to_1_4_0 : PluginMigration
    {
        public Migration_1_3_0_to_1_4_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.3.0";
        public override SemanticVersion to => "1.4.0";

        public override void Run()
        {
            UnitBase.Rebuild();
        }
    }
}
