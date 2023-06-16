using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ProductAttribute : Attribute
    {
        public ProductAttribute(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            this.id = id;
        }

        public string id { get; private set; }
    }
}
