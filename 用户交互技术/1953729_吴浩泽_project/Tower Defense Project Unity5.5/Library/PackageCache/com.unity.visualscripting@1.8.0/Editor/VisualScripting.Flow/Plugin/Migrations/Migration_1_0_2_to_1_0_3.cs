namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Migration_1_0_2_to_1_0_3 : PluginMigration
    {
        public Migration_1_0_2_to_1_0_3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.0.2";
        public override SemanticVersion to => "1.0.3";

        public override void Run()
        {
            RequireAction("Run the new unit options wizard from: \nTools > Bolt > Unit Options Wizard..." +
                "\n\nYou will need to run it every time you change your codebase. " +
                "To skip the wizard and keep the same settings, use: \nTools > Bolt > Update Unit Options");
        }
    }
}
