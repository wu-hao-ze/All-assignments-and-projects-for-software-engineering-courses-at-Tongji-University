using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two objects.
    /// </summary>
    [UnitCategory("Math/Generic")]
    [UnitTitle("Add")]
    [RenamedFrom("Bolt.GenericAdd")]
    [RenamedFrom("Unity.VisualScripting.GenericAdd")]
    [Obsolete("Use the new \"Add (Math/Generic)\" node instead.")]
    public sealed class DeprecatedGenericAdd : Add<object>
    {
        public override object Operation(object a, object b)
        {
            return OperatorUtility.Add(a, b);
        }
    }
}
