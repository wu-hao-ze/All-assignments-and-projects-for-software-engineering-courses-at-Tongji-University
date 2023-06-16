using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [GraphContext(typeof(StateGraph))]
    public class StateGraphContext : GraphContext<StateGraph, StateCanvas>
    {
        public StateGraphContext(GraphReference reference) : base(reference) { }

        public override string windowTitle => "State Graph";

        protected override IEnumerable<ISidebarPanelContent> SidebarPanels()
        {
            yield return new GraphInspectorPanel(this);
            yield return new VariablesPanel(this);
        }
    }
}
