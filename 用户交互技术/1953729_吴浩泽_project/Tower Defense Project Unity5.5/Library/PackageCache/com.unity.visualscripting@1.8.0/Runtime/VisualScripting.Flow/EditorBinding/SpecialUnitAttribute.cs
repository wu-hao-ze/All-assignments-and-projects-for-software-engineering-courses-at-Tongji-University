using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class SpecialUnitAttribute : Attribute
    { }
}
