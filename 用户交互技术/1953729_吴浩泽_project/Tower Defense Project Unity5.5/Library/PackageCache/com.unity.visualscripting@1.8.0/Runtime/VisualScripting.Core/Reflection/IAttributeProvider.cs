using System;

namespace Unity.VisualScripting
{
    public interface IAttributeProvider
    {
        Attribute[] GetCustomAttributes(bool inherit);
    }
}
