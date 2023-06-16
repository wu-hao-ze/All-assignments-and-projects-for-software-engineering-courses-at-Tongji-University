namespace Unity.VisualScripting
{
    public interface IStateTransition : IGraphElementWithDebugData, IConnection<IState, IState>
    {
        void Branch(Flow flow);

        void OnEnter(Flow flow);

        void OnExit(Flow flow);
    }
}
