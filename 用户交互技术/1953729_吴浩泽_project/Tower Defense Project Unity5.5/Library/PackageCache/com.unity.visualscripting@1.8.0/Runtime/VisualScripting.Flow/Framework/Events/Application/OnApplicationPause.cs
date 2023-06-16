namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the application pauses.
    /// </summary>
    [UnitCategory("Events/Application")]
    public sealed class OnApplicationPause : GlobalEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnApplicationPause;
    }
}
