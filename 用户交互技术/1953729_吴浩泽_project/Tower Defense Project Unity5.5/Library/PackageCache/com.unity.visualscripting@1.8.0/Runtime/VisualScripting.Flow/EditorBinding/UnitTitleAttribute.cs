using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class UnitTitleAttribute : Attribute
    {
        public UnitTitleAttribute(string title)
        {
            this.title = title;
        }

        public string title { get; private set; }
    }
}
