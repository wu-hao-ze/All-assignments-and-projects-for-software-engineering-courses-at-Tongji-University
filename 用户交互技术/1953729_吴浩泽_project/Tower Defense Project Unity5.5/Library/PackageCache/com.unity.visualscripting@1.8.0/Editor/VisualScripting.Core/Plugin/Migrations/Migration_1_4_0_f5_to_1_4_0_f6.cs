namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class Migration_1_4_0_f5_to_1_4_0_f6 : BoltCoreMigration
    {
        public Migration_1_4_0_f5_to_1_4_0_f6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion @from => "1.4.0f5";
        public override SemanticVersion to => "1.4.0f6";

        public override void Run()
        {
            AddDefaultTypeOption(typeof(UnityEngine.Resources));
        }
    }
}
