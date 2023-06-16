namespace Unity.VisualScripting
{
    [Inspector(typeof(IUnit))]
    public class UnitInspector : ReflectedInspector
    {
        public UnitInspector(Metadata metadata) : base(metadata) { }
    }
}
