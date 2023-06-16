namespace Unity.VisualScripting
{
    /// <summary>
    /// Called in flow graphs nested in state graphs when the parent state node is entered.
    /// </summary>
    [UnitCategory("Events/State")]
    public class OnEnterState : ManualEventUnit<EmptyEventArgs>
    {
        protected override string hookName => StateEventHooks.OnEnterState;
    }
}
