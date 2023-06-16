using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [SpecialUnit]
    public abstract class MemberUnit : Unit, IAotStubbable
    {
        protected MemberUnit() : base() { }

        protected MemberUnit(Member member) : this()
        {
            this.member = member;
        }

        [Serialize]
        [MemberFilter(Fields = true, Properties = true, Methods = true, Constructors = true)]
        public Member member { get; set; }

        /// <summary>
        /// The target object.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput target { get; private set; }

        public override bool canDefine => member != null;

        protected override void Definition()
        {
            member.EnsureReflected();

            if (!IsMemberValid(member))
            {
                throw new NotSupportedException("The member type is not valid for this unit.");
            }

            if (member.requiresTarget)
            {
                target = ValueInput(member.targetType, nameof(target));

                target.SetDefaultValue(member.targetType.PseudoDefault());

                if (typeof(UnityObject).IsAssignableFrom(member.targetType))
                {
                    target.NullMeansSelf();
                }
            }
        }

        protected abstract bool IsMemberValid(Member member);

        public override void Prewarm()
        {
            if (member != null && member.isReflected)
            {
                member.Prewarm();
            }
        }

        public override IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            if (member != null && member.isReflected)
            {
                yield return member.info;
            }
        }
    }
}
