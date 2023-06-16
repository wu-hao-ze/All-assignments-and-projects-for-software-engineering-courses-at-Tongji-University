#pragma warning disable 618

namespace Unity.VisualScripting
{
    public interface IVariableUnitOption
    {
        string Kind();
    }

    public abstract class VariableUnitOption<TVariableUnit> : UnitOption<TVariableUnit>, IVariableUnitOption where TVariableUnit : VariableUnit
    {
        protected VariableUnitOption() : base() { }

        protected VariableUnitOption(TVariableUnit unit) : base(unit) { }

        protected bool hasDefaultName => !string.IsNullOrEmpty(unit.defaultName);

        protected override string FavoriteKey()
        {
            return $"{unit.GetType().FullName}${unit.defaultName}";
        }

        private string DimmedKind()
        {
            return LudiqGUIUtility.DimString($" ({Kind()})");
        }

        public virtual string Kind()
        {
            return unit.GetType().HumanName();
        }

        protected virtual string DefaultNameLabel()
        {
            return unit.defaultName;
        }

        protected override string Label(bool human)
        {
            if (hasDefaultName)
            {
                return DefaultNameLabel() + DimmedKind();
            }
            else
            {
                return base.Label(human);
            }
        }

        protected override string Haystack(bool human)
        {
            if (hasDefaultName)
            {
                return DefaultNameLabel();
            }

            return base.Haystack(human);
        }

        public override string SearchResultLabel(string query)
        {
            if (hasDefaultName)
            {
                return base.SearchResultLabel(query) + DimmedKind();
            }
            else
            {
                return base.SearchResultLabel(query);
            }
        }
    }
}
