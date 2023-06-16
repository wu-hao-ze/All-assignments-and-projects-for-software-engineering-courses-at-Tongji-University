namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_1_3_to_1_2_0 : PluginMigration
    {
        public Migration_1_1_3_to_1_2_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.1.3";
        public override SemanticVersion to => "1.2.0";

        public override void Run()
        {
            //UnitBase.Build();
        }
    }
}
