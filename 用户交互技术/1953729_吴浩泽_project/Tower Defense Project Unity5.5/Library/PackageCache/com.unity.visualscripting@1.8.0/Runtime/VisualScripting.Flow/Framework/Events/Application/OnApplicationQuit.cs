namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the application quits.
    /// </summary>
    [UnitCategory("Events/Application")]
    public sealed class OnApplicationQuit : GlobalEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnApplicationQuit;
    }
}
