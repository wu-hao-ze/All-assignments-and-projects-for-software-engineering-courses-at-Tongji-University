using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_2_0 : PluginChangelog
    {
        public Changelog_1_2_0(Plugin plugin) : base(plugin) { }

        public override string description => null;
        public override SemanticVersion version => "1.2.0";
        public override DateTime date => new DateTime(2017, 11, 16);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Component drag & drop";
                yield return "[Added] Game Object drag & drop";
                yield return "[Added] Scriptable Object drag & drop";
                yield return "[Added] Macro drag & drop onto nester units for replacement";
                yield return "[Added] Variable drag & drop";
                yield return "[Added] Variable kind dropdown";
                yield return "[Added] Variable name suggestion dropdown";
                yield return "[Added] Object variable name suggestions from current parent object";
                yield return "[Added] Dynamic variable name suggestions from current graph";
                yield return "[Added] Subcategories to variables category in fuzzy finder";
                yield return "[Added] Parent object components category to fuzzy finder";
                yield return "[Added] Automatic target port selection when creating connections";
                yield return "[Added] Connection target preview overlay";
                yield return "[Added] Dimming of incompatible nodes and ports";
                yield return "[Added] Hovered port and connection highlight";
                yield return "[Added] Option to skip context menu";
                yield return "[Added] Add Unit option to context menu";
                yield return "[Added] Non-destructive unit replacement (Context > Replace Unit...)";
                yield return "[Added] Option to disable editor value prediction";
                yield return "[Added] Option for dictionary enumeration in for each loop";
                yield return "[Added] Fallback value option for get variable units";
                yield return "[Fixed] Obsolete unit warning on inherited types";
                yield return "[Obsoleted] Previous variable units";
            }
        }
    }
}
