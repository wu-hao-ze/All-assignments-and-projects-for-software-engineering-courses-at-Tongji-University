using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DisableAnnotationAttribute : Attribute
    {
        public bool disableIcon { get; set; } = true;
        public bool disableGizmo { get; set; } = false;
    }
}
