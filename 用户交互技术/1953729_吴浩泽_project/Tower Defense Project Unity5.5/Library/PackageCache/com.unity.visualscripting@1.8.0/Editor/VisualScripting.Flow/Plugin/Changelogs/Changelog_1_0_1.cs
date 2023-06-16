using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_0_1 : PluginChangelog
    {
        public Changelog_1_0_1(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.0.1";
        public override DateTime date => new DateTime(2017, 07, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Editor crash when units describe infinite indirect recursion";
                yield return "[Fixed] Automatic type conversion not happening on member invocation units";
                yield return "[Fixed] Reflection bug for methods declaring value type parameters with default values";
                yield return "[Fixed] Inline values for game objects and components getting overridden by self";
                yield return "[Fixed] Runtime added events not listening instantly";
            }
        }
    }
}
