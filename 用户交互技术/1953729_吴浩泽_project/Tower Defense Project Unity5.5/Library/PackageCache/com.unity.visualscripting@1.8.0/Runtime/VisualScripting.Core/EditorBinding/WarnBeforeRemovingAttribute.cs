using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class WarnBeforeRemovingAttribute : Attribute
    {
        public WarnBeforeRemovingAttribute(string warningTitle, string warningMessage)
        {
            this.warningTitle = warningTitle;
            this.warningMessage = warningMessage;
        }

        public string warningTitle { get; }
        public string warningMessage { get; }
    }
}
