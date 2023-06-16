namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the machine becomes enabled and active.
    /// </summary>
    [UnitCategory("Events/Lifecycle")]
    [UnitOrder(1)]
    public sealed class OnEnable : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnEnable;
    }
}
