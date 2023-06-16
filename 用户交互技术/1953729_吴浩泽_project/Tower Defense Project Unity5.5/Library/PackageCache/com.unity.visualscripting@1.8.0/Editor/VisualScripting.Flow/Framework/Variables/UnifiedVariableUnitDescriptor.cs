namespace Unity.VisualScripting
{
    [Descriptor(typeof(UnifiedVariableUnit))]
    public class UnifiedVariableUnitDescriptor<TVariableUnit> : UnitDescriptor<TVariableUnit> where TVariableUnit : UnifiedVariableUnit
    {
        public UnifiedVariableUnitDescriptor(TVariableUnit unit) : base(unit) { }

        protected override EditorTexture DefinedIcon()
        {
            return BoltCore.Icons.VariableKind(unit.kind);
        }
    }
}
