namespace Unity.VisualScripting
{
    /// <summary>
    /// Called in flow graphs nested in state graphs before the parent state node is exited.
    /// </summary>
    [UnitCategory("Events/State")]
    public class OnExitState : ManualEventUnit<EmptyEventArgs>
    {
        protected override string hookName => StateEventHooks.OnExitState;
    }
}
