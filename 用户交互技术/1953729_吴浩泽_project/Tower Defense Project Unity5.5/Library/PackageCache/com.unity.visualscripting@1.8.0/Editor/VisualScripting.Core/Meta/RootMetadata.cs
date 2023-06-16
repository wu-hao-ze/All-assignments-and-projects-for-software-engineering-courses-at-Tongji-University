using System;

namespace Unity.VisualScripting
{
    public sealed class RootMetadata : Metadata
    {
        public RootMetadata() : base("Root", null) { }

        protected override bool isRoot => true;

        protected override object rawValue
        {
            get
            {
                return null;
            }
            set { }
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return Empty<Attribute>.array;
        }
    }
}
