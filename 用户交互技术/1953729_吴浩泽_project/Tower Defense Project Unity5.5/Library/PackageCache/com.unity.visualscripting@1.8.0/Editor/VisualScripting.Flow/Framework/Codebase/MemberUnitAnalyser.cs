using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Analyser(typeof(MemberUnit))]
    public class MemberUnitAnalyser : UnitAnalyser<MemberUnit>
    {
        public MemberUnitAnalyser(GraphReference reference, MemberUnit target) : base(reference, target) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (target.member != null && target.member.isReflected)
            {
                var obsoleteAttribute = target.member.info.GetAttribute<ObsoleteAttribute>();

                if (obsoleteAttribute != null)
                {
                    if (obsoleteAttribute.Message != null)
                    {
                        Debug.LogWarning($"\"{target.member.name}\" node member is deprecated: {obsoleteAttribute.Message}");
                        yield return Warning.Caution("Deprecated: " + obsoleteAttribute.Message);
                    }
                    else
                    {
                        Debug.LogWarning($"\"{target.member.name}\" node member is deprecated.");
                        yield return Warning.Caution($"Member {target.member.name} is deprecated.");
                    }
                }
            }
        }
    }
}
