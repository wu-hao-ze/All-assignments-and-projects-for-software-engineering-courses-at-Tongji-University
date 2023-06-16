namespace Unity.VisualScripting
{
    /// <summary>
    /// Use to draw gizmos that are drawn in the editor when the object is selected.
    /// </summary>
    [UnitCategory("Events/Editor")]
    public sealed class OnDrawGizmosSelected : ManualEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnDrawGizmosSelected;
    }
}
