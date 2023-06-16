namespace Unity.VisualScripting
{
    public sealed class AutomaticReflectedInspector : ReflectedInspector
    {
        public AutomaticReflectedInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            metadata.instantiate = true;

            base.Initialize();
        }
    }
}
