namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_1_1_to_1_1_2 : PluginMigration
    {
        public Migration_1_1_1_to_1_1_2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.1.1";
        public override SemanticVersion to => "1.1.2";

        public override void Run()
        {
            // RequireAction("Update your unit options from:\nTools > Bolt > Update Unit Options");
        }
    }
}
