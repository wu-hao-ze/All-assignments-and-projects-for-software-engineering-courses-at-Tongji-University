using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Allows the customisation of the SystemObjectInspector by displaying just the value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ValueAttribute : Attribute
    {
    }
}
