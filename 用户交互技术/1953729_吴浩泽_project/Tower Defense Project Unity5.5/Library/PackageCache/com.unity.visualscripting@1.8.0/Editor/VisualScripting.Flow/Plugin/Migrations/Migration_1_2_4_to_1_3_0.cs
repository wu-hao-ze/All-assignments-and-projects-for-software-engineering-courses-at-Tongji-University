namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_2_4_to_1_3_0 : PluginMigration
    {
        public Migration_1_2_4_to_1_3_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.2.4";
        public override SemanticVersion to => "1.3.0";

        public override void Run()
        {
            ScriptReferenceResolver.Run();

            UnitBase.Rebuild();

            RequireAction("Version 1.3 is a major refactor that changed most of the folder structure. Some manual actions may be required. See: http://bit.do/bolt-1-3");
        }
    }
}
