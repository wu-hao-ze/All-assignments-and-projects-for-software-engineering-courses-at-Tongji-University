namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(GetVariableUnit))]
    public class GetVariableUnitOption<TVariableUnit> : VariableUnitOption<TVariableUnit> where TVariableUnit : GetVariableUnit
    {
        public GetVariableUnitOption() : base() { }

        public GetVariableUnitOption(TVariableUnit unit) : base(unit) { }

        public override string Kind()
        {
            return base.Kind().TrimStart("Get ");
        }

        protected override string DefaultNameLabel()
        {
            return $"Get {unit.defaultName}";
        }
    }
}
