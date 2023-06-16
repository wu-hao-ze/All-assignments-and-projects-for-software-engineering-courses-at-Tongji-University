using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class RequiresUnityAPIAttribute : Attribute
    {
        public RequiresUnityAPIAttribute() : base() { }
    }
}
