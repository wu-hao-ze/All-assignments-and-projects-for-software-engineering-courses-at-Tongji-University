using System;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(Literal))]
    public class LiteralOption : UnitOption<Literal>
    {
        public LiteralOption() : base() { }

        public LiteralOption(Literal unit) : base(unit)
        {
            sourceScriptGuids = UnitBase.GetScriptGuids(unit.type).ToHashSet();
        }

        public Type literalType { get; private set; }

        protected override void FillFromUnit()
        {
            literalType = unit.type;
            base.FillFromUnit();
        }

        protected override string Label(bool human)
        {
            if (unit.value is UnityObject uo && !uo.IsUnityNull())
            {
                return UnityAPI.Await(() => uo.name);
            }

            return unit.type.SelectedName(human) + " Literal";
        }

        protected override EditorTexture Icon()
        {
            if (unit.value is UnityObject uo && !uo.IsUnityNull())
            {
                return uo.Icon();
            }

            return base.Icon();
        }

        protected override string FavoriteKey()
        {
            return $"{literalType.FullName}@literal";
        }

        public override void Deserialize(UnitOptionRow row)
        {
            base.Deserialize(row);

            literalType = Codebase.DeserializeType(row.tag1);
        }

        public override UnitOptionRow Serialize()
        {
            var row = base.Serialize();

            row.tag1 = Codebase.SerializeType(literalType);

            return row;
        }
    }
}
