namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_2_0_to_1_2_2 : PluginMigration
    {
        public Migration_1_2_0_to_1_2_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.2.0";
        public override SemanticVersion to => "1.2.2";

        public override void Run()
        {
            UnitBase.Rebuild();
        }
    }
}
