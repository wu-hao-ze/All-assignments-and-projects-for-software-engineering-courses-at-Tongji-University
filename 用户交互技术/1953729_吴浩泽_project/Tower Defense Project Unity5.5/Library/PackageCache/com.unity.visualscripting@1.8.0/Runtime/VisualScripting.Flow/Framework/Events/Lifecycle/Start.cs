namespace Unity.VisualScripting
{
    /// <summary>
    /// Called the first time a machine is enabled before any update method.
    /// </summary>
    [UnitCategory("Events/Lifecycle")]
    [UnitOrder(2)]
    [UnitTitle("On Start")]
    public sealed class Start : MachineEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.Start;
    }
}
