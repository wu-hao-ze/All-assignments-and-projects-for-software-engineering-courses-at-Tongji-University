namespace Unity.VisualScripting
{
    /// <summary>
    /// Use to draw gizmos that are always drawn in the editor.
    /// </summary>
    [UnitCategory("Events/Editor")]
    public sealed class OnDrawGizmos : ManualEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnDrawGizmos;
    }
}
