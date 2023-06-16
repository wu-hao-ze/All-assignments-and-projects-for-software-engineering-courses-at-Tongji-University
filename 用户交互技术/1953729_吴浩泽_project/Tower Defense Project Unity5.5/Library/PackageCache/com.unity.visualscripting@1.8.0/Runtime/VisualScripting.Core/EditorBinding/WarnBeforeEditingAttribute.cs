using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class WarnBeforeEditingAttribute : Attribute
    {
        public WarnBeforeEditingAttribute(string warningTitle, string warningMessage)
        {
            this.warningTitle = warningTitle;
            this.warningMessage = warningMessage;
        }

        public WarnBeforeEditingAttribute(string warningTitle, string warningMessage, params object[] emptyValues) : this(warningTitle, warningMessage)
        {
            this.emptyValues = emptyValues;
        }

        public string warningTitle { get; }
        public string warningMessage { get; }
        public object[] emptyValues { get; }
    }
}
