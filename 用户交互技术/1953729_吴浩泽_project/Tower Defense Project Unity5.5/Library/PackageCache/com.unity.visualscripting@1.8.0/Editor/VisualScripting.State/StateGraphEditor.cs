namespace Unity.VisualScripting
{
    [Editor(typeof(StateGraph))]
    public class StateGraphEditor : GraphEditor
    {
        public StateGraphEditor(Metadata metadata) : base(metadata) { }

        private new StateGraph graph => (StateGraph)base.graph;
    }
}
