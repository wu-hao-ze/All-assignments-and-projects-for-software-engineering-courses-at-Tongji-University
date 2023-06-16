namespace Unity.VisualScripting
{
    [Editor(typeof(IState))]
    public class StateEditor : GraphElementEditor<StateGraphContext>
    {
        public StateEditor(Metadata metadata) : base(metadata) { }

        protected IState state => (IState)element;

        protected new StateDescription description => (StateDescription)base.description;
    }
}
