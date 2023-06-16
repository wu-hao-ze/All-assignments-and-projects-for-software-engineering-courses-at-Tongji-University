namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the machine becomes disabled or inactive.
    /// </summary>
    [UnitCategory("Events/Lifecycle")]
    [UnitOrder(6)]
    public sealed class OnDisable : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnDisable;
    }
}
