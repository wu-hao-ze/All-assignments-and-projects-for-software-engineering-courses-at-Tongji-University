using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PortLabelHiddenAttribute : Attribute
    { }
}
