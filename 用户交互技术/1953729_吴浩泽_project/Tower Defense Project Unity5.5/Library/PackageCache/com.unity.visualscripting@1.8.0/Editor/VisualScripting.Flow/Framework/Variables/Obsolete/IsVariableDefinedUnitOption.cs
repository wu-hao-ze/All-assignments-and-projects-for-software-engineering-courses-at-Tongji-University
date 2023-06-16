namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(IsVariableDefinedUnit))]
    public class IsVariableDefinedUnitOption<TVariableUnit> : VariableUnitOption<TVariableUnit> where TVariableUnit : IsVariableDefinedUnit
    {
        public IsVariableDefinedUnitOption() : base() { }

        public IsVariableDefinedUnitOption(TVariableUnit unit) : base(unit) { }

        public override string Kind()
        {
            return base.Kind().TrimStart("Is ").TrimEnd(" Defined");
        }

        protected override string DefaultNameLabel()
        {
            return $"Is {unit.defaultName} Defined";
        }
    }
}
