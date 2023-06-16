using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Analyser(typeof(For))]
    public class ForAnalyser : UnitAnalyser<For>
    {
        public ForAnalyser(GraphReference reference, For target)
            : base(reference, target) { }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
            {
                yield return baseWarning;
            }

            if (unit.IsStepValueZero())
            {
                yield return Warning.Caution("The step value is 0. This will prevent the For node to be executed or can cause an infinite loop.");
            }
        }
    }
}
