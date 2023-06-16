#pragma warning disable 618

namespace Unity.VisualScripting
{
    [Descriptor(typeof(VariableUnit))]
    public class VariableUnitDescriptor<TVariableUnit> : UnitDescriptor<TVariableUnit> where TVariableUnit : VariableUnit
    {
        public VariableUnitDescriptor(TVariableUnit unit) : base(unit) { }

        protected bool hasDefaultName => !string.IsNullOrEmpty(unit.defaultName);

        protected override string DefinedSummary()
        {
            var summary = base.DefinedSummary();

            if (hasDefaultName)
            {
                summary += $" (\"{unit.defaultName}\")";
            }

            return summary;
        }
    }
}
