using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_0_4 : PluginChangelog
    {
        public Changelog_1_0_4(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.4";
        public override DateTime date => new DateTime(2017, 09, 15);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Constructors at the root of contextual fuzzy finders";
                yield return "[Added] Scalar normalize unit";
                yield return "[Changed] Super units with nested events color to green";
                yield return "[Changed] Header display for variable units";
                yield return "[Changed] Error message for member units edge case";
                yield return "[Fixed] Type downcasting only working for object";
                yield return "[Fixed] Nested state machines not updating";
                yield return "[Fixed] Nested graphs not creating AOT stubs";
                yield return "[Fixed] Custom event argument count error";
                yield return "[Fixed] Various small recursion related issues";
                yield return "[Fixed] Variable units not pre-filling name";
                yield return "[Fixed] Unscaled delta time for On Timer Elapsed event";
                yield return "[Fixed] Scene variables singleton error with additive scene loading";
            }
        }
    }
}
