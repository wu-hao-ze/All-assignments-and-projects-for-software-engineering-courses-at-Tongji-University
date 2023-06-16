using System;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(Expose))]
    public class ExposeOption : UnitOption<Expose>
    {
        public ExposeOption() : base() { }

        public ExposeOption(Expose unit) : base(unit)
        {
            sourceScriptGuids = UnitBase.GetScriptGuids(unit.type).ToHashSet();
        }

        public Type exposedType { get; private set; }

        protected override string FavoriteKey()
        {
            return $"{exposedType.FullName}@expose";
        }

        protected override string Label(bool human)
        {
            return $"Expose {unit.type.SelectedName(human)}";
        }

        protected override bool ShowValueOutputsInFooter()
        {
            return false;
        }

        protected override void FillFromUnit()
        {
            exposedType = unit.type;

            base.FillFromUnit();
        }

        public override void Deserialize(UnitOptionRow row)
        {
            base.Deserialize(row);

            exposedType = Codebase.DeserializeType(row.tag1);
        }

        public override UnitOptionRow Serialize()
        {
            var row = base.Serialize();

            row.tag1 = Codebase.SerializeType(exposedType);

            return row;
        }
    }
}
