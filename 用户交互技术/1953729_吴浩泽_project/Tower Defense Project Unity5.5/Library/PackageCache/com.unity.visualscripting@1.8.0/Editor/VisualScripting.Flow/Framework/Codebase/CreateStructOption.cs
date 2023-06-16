using System;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(CreateStruct))]
    public class CreateStructOption : UnitOption<CreateStruct>
    {
        public CreateStructOption() : base() { }

        public CreateStructOption(CreateStruct unit) : base(unit) { }

        public Type structType { get; private set; }

        protected override void FillFromUnit()
        {
            structType = unit.type;
            base.FillFromUnit();
        }

        protected override string Label(bool human)
        {
            if (human)
            {
                return $"Create {structType.HumanName()} ()";
            }
            else
            {
                return $"new {structType.CSharpName()} ()";
            }
        }

        protected override string Haystack(bool human)
        {
            if (human)
            {
                return $"{structType.HumanName()}: Create {structType.HumanName()}";
            }
            else
            {
                return $"new {structType.CSharpName()}";
            }
        }

        public override string SearchResultLabel(string query)
        {
            return base.SearchResultLabel(query) + " ()";
        }

        protected override int Order()
        {
            return 0;
        }

        protected override string FavoriteKey()
        {
            return $"{structType.FullName}@create";
        }

        public override void Deserialize(UnitOptionRow row)
        {
            base.Deserialize(row);

            structType = Codebase.DeserializeType(row.tag1);
        }

        public override UnitOptionRow Serialize()
        {
            var row = base.Serialize();

            row.tag1 = Codebase.SerializeType(structType);

            return row;
        }
    }
}
