using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_0 : PluginChangelog
    {
        public Changelog_1_4_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0";
        public override DateTime date => new DateTime(2018, 07, 13);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Coroutine flow toggle on events";
                yield return "[Added] Support for wait units in loops and sequences";
                yield return "[Added] Support for gizmos and in-editor events";
                yield return "[Added] Local port caching";
                yield return "[Added] Flow variables";
                yield return "[Added] Try, Catch and Throw units";
                yield return "[Added] Timer unit";
                yield return "[Added] Cooldown unit";
                yield return "[Added] Once unit";
                yield return "[Added] Wait For Flow unit";
                yield return "[Added] Select On Flow unit";
                yield return "[Added] Toggle Flow and Toggle Value units";
                yield return "[Added] On Animator Move and On Animator IK events";
                yield return "[Added] Drag, Drop, Scroll, Move, Cancel and Submit GUI events";
                yield return "[Added] Chainable option for set and invoke units";
                yield return "[Added] Literal option in Unity object drag & drop";
                yield return "[Optimized] Event triggering and trickling";
                yield return "[Optimized] Play mode entry";
                yield return "[Changed] Break Loop icon";
                yield return "[Changed] Debug icon";
                yield return "[Obsoleted] On Timer Elapsed event";
                yield return "[Fixed] Literal failing to render missing types";
                yield return "[Fixed] Missing types causing errors in the fuzzy finder";
                yield return "[Fixed] Switch and other units not appearing in the fuzzy finder";
                yield return "[Fixed] Various naming issues and typos in the fuzzy finder";
                yield return "[Fixed] Missing GameObject from drag & drop";
                yield return "[Fixed] Replaced obsolete Unity members";
                yield return "[Fixed] Typo in Insert List Item";
                yield return "[Fixed] Recursive value fetching analysis";
                yield return "[Fixed] Adaptive field widths on literals";
            }
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_0f4 : PluginChangelog
    {
        public Changelog_1_4_0f4(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f4";
        public override DateTime date => new DateTime(2018, 08, 02);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] GraphPointerException when using loops and sequences with super units";
                yield return "[Fixed] Error when undoing events with input ports";
                yield return "[Fixed] Scene references being allowed in macros via drag & drop";
            }
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_0f5 : PluginChangelog
    {
        public Changelog_1_4_0f5(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f5";
        public override DateTime date => new DateTime(2018, 08, 14);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Generic classes of Material and Color breaking rich text";
            }
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_0f6 : PluginChangelog
    {
        public Changelog_1_4_0f6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f6";
        public override DateTime date => new DateTime(2018, 09, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Create Struct units for default struct initializers";
                yield return "[Improved] Shortened constructor units node title";
                yield return "[Improved] Member unit reflection";
                yield return "[Fixed] Recursion fake positive when using super units referencing the same macro";
                yield return "[Fixed] Unit favorites not saving";
            }
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_0f10 : PluginChangelog
    {
        public Changelog_1_4_0f10(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f10";
        public override DateTime date => new DateTime(2018, 10, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Changed] Stop coroutines when their parent event stops listening (to fix asynchronous transitions)";
                yield return "[Fixed] Timer not assigning metrics value outputs in Started flow";
                yield return "[Fixed] P/Invoke parameters marked with [Out] but without out keyword not showing on units";
                yield return "[Fixed] Height not updating on literal widgets for custom property drawers";
            }
        }
    }

    [Plugin(BoltFlow.ID)]
    internal class Changelog_1_4_0f11 : PluginChangelog
    {
        public Changelog_1_4_0f11(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f11";
        public override DateTime date => new DateTime(2018, 11, 08);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Formula units not caching input parameters";
            }
        }
    }
}
