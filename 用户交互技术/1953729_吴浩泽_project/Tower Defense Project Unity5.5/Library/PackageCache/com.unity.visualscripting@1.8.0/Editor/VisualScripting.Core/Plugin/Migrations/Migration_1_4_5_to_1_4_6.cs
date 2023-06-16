namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Migration_1_4_5_to_1_4_6 : BoltCoreMigration
    {
        public Migration_1_4_5_to_1_4_6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.5";
        public override SemanticVersion to => "1.4.6";

        public override void Run()
        {
            // Typo: should have been InputLegacyModule
            // AddDefaultAssemblyOption("UnityEngine.LegacyInputModule");
            // RequireAction("Please rebuild your unit options (Tools > Bolt > Build Unit Options).");
        }
    }

    [Plugin(BoltCore.ID)]
    internal class Migration_1_4_6_to_1_4_6f3 : BoltCoreMigration
    {
        public Migration_1_4_6_to_1_4_6f3(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.6";
        public override SemanticVersion to => "1.4.6f3";

        public override void Run()
        {
            AddDefaultAssemblyOption("UnityEngine.InputLegacyModule");

            RequireAction("Please rebuild your unit options (Tools > Bolt > Build Unit Options).");
        }
    }
}
