using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [GraphContext(typeof(FlowGraph))]
    public class FlowGraphContext : GraphContext<FlowGraph, FlowCanvas>
    {
        public FlowGraphContext(GraphReference reference) : base(reference) { }

        public override string windowTitle => "Script Graph";

        protected override IEnumerable<ISidebarPanelContent> SidebarPanels()
        {
            yield return new GraphInspectorPanel(this);
            yield return new VariablesPanel(this);
        }
    }
}
