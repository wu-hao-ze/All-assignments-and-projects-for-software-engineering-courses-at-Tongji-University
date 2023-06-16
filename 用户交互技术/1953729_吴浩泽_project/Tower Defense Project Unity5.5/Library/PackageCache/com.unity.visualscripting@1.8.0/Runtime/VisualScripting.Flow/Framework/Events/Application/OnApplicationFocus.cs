namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the application gains focus.
    /// </summary>
    [UnitCategory("Events/Application")]
    public sealed class OnApplicationFocus : GlobalEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnApplicationFocus;
    }
}
