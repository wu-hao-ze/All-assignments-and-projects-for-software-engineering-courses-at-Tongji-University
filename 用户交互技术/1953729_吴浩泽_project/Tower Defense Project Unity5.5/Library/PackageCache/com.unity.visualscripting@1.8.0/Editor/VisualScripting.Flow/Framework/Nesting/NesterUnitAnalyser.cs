using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Analyser(typeof(INesterUnit))]
    public class NesterUnitAnalyser<TNesterUnit> : UnitAnalyser<TNesterUnit> where TNesterUnit : class, INesterUnit
    {
        public NesterUnitAnalyser(GraphReference reference, TNesterUnit unit) : base(reference, unit) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (unit.childGraph == null)
            {
                yield return Warning.Caution("Missing nested graph.");
            }

            if (unit.nest.hasBackgroundEmbed)
            {
                yield return Warning.Caution("Background embed graph detected.");
            }
        }
    }
}
