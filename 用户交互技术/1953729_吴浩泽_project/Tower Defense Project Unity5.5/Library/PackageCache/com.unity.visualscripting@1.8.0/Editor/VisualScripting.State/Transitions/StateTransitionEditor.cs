namespace Unity.VisualScripting
{
    [Editor(typeof(IStateTransition))]
    public class StateTransitionEditor : GraphElementEditor<StateGraphContext>
    {
        public StateTransitionEditor(Metadata metadata) : base(metadata) { }

        private IStateTransition transition => (IStateTransition)element;

        protected new StateTransitionDescription description => (StateTransitionDescription)base.description;
    }
}
