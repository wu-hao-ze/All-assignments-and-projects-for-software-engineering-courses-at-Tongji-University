using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Plugin(BoltFlow.ID)]
    public sealed class BoltFlowConfiguration : PluginConfiguration
    {
        private BoltFlowConfiguration(BoltFlow plugin) : base(plugin) { }

        public override string header => "Script Graphs";

        /// <summary>
        /// (Experimental) Whether the node database should be incrementally updated
        /// whenever a codebase change is detected.
        /// </summary>
        [EditorPref, RenamedFrom("updateUnitsAutomatically")]
        public bool updateNodesAutomatically { get; set; } = false;

        /// <summary>
        /// Whether predictive debugging should warn about null value inputs.
        /// Note that in some cases, this setting may report false positives.
        /// </summary>
        [EditorPref]
        public bool predictPotentialNullReferences { get; set; } = true;

        /// <summary>
        /// Whether predictive debugging should warn about missing components.
        /// Note that in some cases, this setting may report false positives.
        /// </summary>
        [EditorPref]
        public bool predictPotentialMissingComponents { get; set; } = true;

        /// <summary>
        /// Whether values should be shown on flow graph connections.
        /// </summary>
        [EditorPref]
        public bool showConnectionValues { get; set; } = true;

        /// <summary>
        /// Whether predictable values should be shown on flow graph connections.
        /// </summary>
        [EditorPref]
        public bool predictConnectionValues { get; set; } = false;

        /// <summary>
        /// Whether labels should be hidden on ports when the value can be deduced from the context.
        /// Disabling will make nodes more explicit but less compact.
        /// </summary>
        [EditorPref]
        public bool hidePortLabels { get; set; } = true;

        /// <summary>
        /// Whether active control connections should show a droplet animation.
        /// </summary>
        [EditorPref]
        public bool animateControlConnections { get; set; } = true;

        /// <summary>
        /// Whether active value connections should show a droplet animation.
        /// </summary>
        [EditorPref]
        public bool animateValueConnections { get; set; } = true;

        /// <summary>
        /// When active, right-clicking a flow graph will skip the context menu
        /// and instantly open the fuzzy finder. To open the context menu, hold shift.
        /// </summary>
        [EditorPref]
        public bool skipContextMenu { get; set; } = false;

        [ProjectSetting(visible = false, resettable = false)]
        public HashSet<string> favoriteUnitOptions { get; set; } = new HashSet<string>();
    }
}
