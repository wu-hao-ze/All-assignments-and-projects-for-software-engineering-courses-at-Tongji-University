using System;

namespace Unity.VisualScripting
{
    public class ProxyMetadata : Metadata
    {
        public ProxyMetadata(object subpath, Metadata binding, Metadata parent) : base(subpath, parent)
        {
            this.binding = binding;

            if (binding != null)
            {
                definedType = binding.definedType;
                label = binding.label;
            }
        }

        public Metadata binding { get; private set; }

        protected override object rawValue
        {
            get
            {
                if (binding == null)
                {
                    return null;
                }

                return binding.value;
            }
            set
            {
                if (binding == null)
                {
                    return;
                }

                binding.value = value;
            }
        }

        public override Attribute[] GetCustomAttributes(bool inherit = true)
        {
            return binding.GetCustomAttributes(inherit);
        }
    }
}
