using System;

namespace Unity.VisualScripting
{
    public class CastMetadata : ProxyMetadata
    {
        public CastMetadata(Type newType, Metadata parent) : base(newType, parent, parent)
        {
            this.newType = newType;

            definedType = newType;
        }

        public Type newType { get; private set; }

        protected override string SubpathToString()
        {
            return "(" + newType.CSharpName(false) + ")";
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return parent.GetCustomAttributes(inherit);
        }
    }
}
