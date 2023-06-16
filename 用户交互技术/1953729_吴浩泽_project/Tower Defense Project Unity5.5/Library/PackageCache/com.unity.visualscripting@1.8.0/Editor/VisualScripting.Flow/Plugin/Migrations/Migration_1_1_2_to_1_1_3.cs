namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_1_2_to_1_1_3 : PluginMigration
    {
        public Migration_1_1_2_to_1_1_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.1.2";
        public override SemanticVersion to => "1.1.3";

        public override void Run()
        {
            //UnitBase.CacheStaticOptions();
        }
    }
}
