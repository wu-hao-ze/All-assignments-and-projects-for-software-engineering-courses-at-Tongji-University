using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IMemberUnitOption : IUnitOption
    {
        Type targetType { get; }
        Member member { get; }
        Member pseudoDeclarer { get; }
    }

    public abstract class MemberUnitOption<TMemberUnit> : UnitOption<TMemberUnit>, IMemberUnitOption where TMemberUnit : MemberUnit
    {
        protected MemberUnitOption() : base() { }

        protected MemberUnitOption(TMemberUnit unit) : base(unit)
        {
            sourceScriptGuids = UnitBase.GetScriptGuids(unit.member.targetType).ToHashSet();
        }

        private Member _member;

        private Member _pseudoDeclarer;

        public Member member
        {
            get => _member ?? unit.member;
            set => _member = value;
        }

        public Member pseudoDeclarer
        {
            get => _pseudoDeclarer ?? member.ToPseudoDeclarer();
            set => _pseudoDeclarer = value;
        }

        public bool isPseudoInherited => member == pseudoDeclarer;

        protected abstract ActionDirection direction { get; }

        public Type targetType { get; private set; }

        protected override GUIStyle Style()
        {
            if (unit.member.isPseudoInherited)
            {
                return FuzzyWindow.Styles.optionWithIconDim;
            }

            return base.Style();
        }

        protected override string Label(bool human)
        {
            return unit.member.info.SelectedName(human, direction);
        }

        protected override string FavoriteKey()
        {
            return $"{member.ToUniqueString()}@{direction.ToString().ToLower()}";
        }

        protected override int Order()
        {
            if (member.isConstructor)
            {
                return 0;
            }

            return base.Order();
        }

        protected override string Haystack(bool human)
        {
            return $"{targetType.SelectedName(human)}{(human ? ": " : ".")}{Label(human)}";
        }

        protected override void FillFromUnit()
        {
            targetType = unit.member.targetType;
            member = unit.member;
            pseudoDeclarer = member.ToPseudoDeclarer();

            base.FillFromUnit();
        }

        public override void Deserialize(UnitOptionRow row)
        {
            base.Deserialize(row);

            targetType = Codebase.DeserializeType(row.tag1);

            if (!string.IsNullOrEmpty(row.tag2))
            {
                member = Codebase.DeserializeMember(row.tag2);
            }

            if (!string.IsNullOrEmpty(row.tag3))
            {
                pseudoDeclarer = Codebase.DeserializeMember(row.tag3);
            }
        }

        public override UnitOptionRow Serialize()
        {
            var row = base.Serialize();

            row.tag1 = Codebase.SerializeType(targetType);
            row.tag2 = Codebase.SerializeMember(member);
            row.tag3 = Codebase.SerializeMember(pseudoDeclarer);

            return row;
        }

        public override void OnPopulate()
        {
            // Members are late-reflected to speed up loading and search
            // We only reflect them when we're just about to populate their node
            // By doing it in OnPopulate instead of on-demand later, we ensure
            // any error will be gracefully catched and shown as a warning by
            // the fuzzy window

            member.EnsureReflected();
            pseudoDeclarer.EnsureReflected();

            base.OnPopulate();
        }
    }
}
