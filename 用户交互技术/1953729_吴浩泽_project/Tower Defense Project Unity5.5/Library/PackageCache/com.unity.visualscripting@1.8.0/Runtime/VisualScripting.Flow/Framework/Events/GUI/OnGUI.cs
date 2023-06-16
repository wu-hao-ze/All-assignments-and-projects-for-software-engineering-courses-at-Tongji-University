using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Use to draw immediate mode GUI components.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(GUI))]
    [UnitOrder(0)]
    public sealed class OnGUI : GlobalEventUnit<EmptyEventArgs>
    {
        protected override string hookName => EventHooks.OnGUI;
    }
}
