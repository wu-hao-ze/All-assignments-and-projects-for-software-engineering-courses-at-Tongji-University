namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the application loses focus.
    /// </summary>
    [UnitCategory("Events/Application")]
    public sealed class OnApplicationLostFocus : GlobalEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnApplicationLostFocus;
    }
}
