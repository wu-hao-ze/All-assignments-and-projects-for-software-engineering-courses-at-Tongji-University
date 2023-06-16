using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_3_0 : PluginChangelog
    {
        public Changelog_1_3_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.3.0";
        public override DateTime date => new DateTime(2018, 04, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Unit connection preservation";
                yield return "[Refactored] Unit definition error recovery";
                yield return "[Refactored] Unit port description";
                yield return "[Fixed] Internal KeyedCollection NullReferenceException";
                yield return "[Fixed] Non-component UnityEngine.Object drag & drop";
                yield return "[Added] Multiple graph outputs warning";
                yield return "[Fixed] Conversion errors for missing component prediction";
                yield return "[Fixed] Dictionary units not caching input";
                yield return "[Fixed] Null warning for nullable value types";
                yield return "[Added] Inline inspector for nullable value types";
                yield return "[Fixed] Unit heading text cutoff when zoomed out";
                yield return "[Fixed] CreateDictionary class name typo";
            }
        }
    }
}
