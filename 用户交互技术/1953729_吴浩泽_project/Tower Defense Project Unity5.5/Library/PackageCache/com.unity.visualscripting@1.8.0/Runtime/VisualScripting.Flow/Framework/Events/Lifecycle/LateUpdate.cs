namespace Unity.VisualScripting
{
    /// <summary>
    /// Called every frame after all update functions have been called.
    /// </summary>
    [UnitCategory("Events/Lifecycle")]
    [UnitOrder(5)]
    [UnitTitle("On Late Update")]
    public sealed class LateUpdate : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.LateUpdate;
    }
}
