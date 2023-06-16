namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(SetVariableUnit))]
    public class SetVariableUnitOption<TVariableUnit> : VariableUnitOption<TVariableUnit> where TVariableUnit : SetVariableUnit
    {
        public SetVariableUnitOption() : base() { }

        public SetVariableUnitOption(TVariableUnit unit) : base(unit) { }

        public override string Kind()
        {
            return base.Kind().TrimStart("Set ");
        }

        protected override string DefaultNameLabel()
        {
            return $"Set {unit.defaultName}";
        }
    }
}
