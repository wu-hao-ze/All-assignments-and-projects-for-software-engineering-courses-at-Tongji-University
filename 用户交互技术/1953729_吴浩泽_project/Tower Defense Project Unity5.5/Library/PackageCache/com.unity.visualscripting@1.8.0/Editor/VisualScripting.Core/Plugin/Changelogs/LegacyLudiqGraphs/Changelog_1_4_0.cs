using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0";
        public override DateTime date => new DateTime(2018, 05, 16);

        public override string description => "Live editing is here! Edit your graphs while in play mode and changes will be saved and propagated.";

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Added] Saving of changes made to macros in play mode";
                yield return "[Added] Propagation of changes made to macros in play mode to all instance";
                yield return "[Added] Ability to modify graph nests while in play mode";
                yield return "[Added] Multiple graph window tabs";
                yield return "[Added] Graph reference preservation across assembly reloads";
                yield return "[Added] Sidebar when graph window viewport is maximized";
                yield return "[Added] Double-click / context-menu to maximize";
                yield return "[Fixed] Duplicating from the context menu";
                yield return "[Fixed] Checkbox inspector display issues";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f2 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f2(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f2";
        public override DateTime date => new DateTime(2018, 07, 13);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] API thread error during time / frame fetching";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f5 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f5(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f5";
        public override DateTime date => new DateTime(2018, 08, 14);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Widgets repositioning on paste";
                yield return "[Fixed] Reverted horfix for unit heading labels width";
                yield return "[Changed] Moved window maximization to toolbar";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f6 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f6(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f6";
        public override DateTime date => new DateTime(2018, 09, 06);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Graph window focusing on other machine on the same game object";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f7 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f7(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f7";
        public override DateTime date => new DateTime(2018, 09, 25);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Optimized] Graph window performance, especially while dragging";
                yield return "[Fixed] Lag spike after destroying machines in the editor";
                yield return "[Fixed] Macro selection not automatically changing graph context";
                yield return "[Fixed] Graph analysis not updating on context change";
                yield return "[Fixed] Z-ordering issues on widget paste";
                yield return "[Fixed] Error when deleting a dragged widget";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f8 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f8(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f8";
        public override DateTime date => new DateTime(2018, 10, 05);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Optimized] Debugging Data Assignment";
                yield return "[Fixed] Recursion memory allocation";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f9 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f9(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f9";
        public override DateTime date => new DateTime(2018, 10, 11);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Clipboard events in sidebar being handled by the canvas instead";
                yield return "[Fixed] Error when undoing a selected graph element";
                yield return "[Fixed] Edge pan not triggering on mouse move or idling near canvas edge";
            }
        }
    }

    [Plugin(BoltCore.ID)]
    internal class LudiqGraphsChangelog_1_4_0f10 : PluginChangelog
    {
        public LudiqGraphsChangelog_1_4_0f10(Plugin plugin) : base(plugin) { }

        public override SemanticVersion version => "1.4.0f10";
        public override DateTime date => new DateTime(2018, 10, 29);

        public override IEnumerable<string> changes
        {
            get
            {
                yield return "[Fixed] Runtime scene loading causing a concurrency error with graph instantiation (tentative)";
            }
        }
    }
}
