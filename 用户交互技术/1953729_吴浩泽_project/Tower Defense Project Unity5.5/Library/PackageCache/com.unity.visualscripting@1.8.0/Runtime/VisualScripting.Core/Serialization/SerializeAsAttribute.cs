using Unity.VisualScripting.FullSerializer;

namespace Unity.VisualScripting
{
    public class SerializeAsAttribute : fsPropertyAttribute
    {
        public SerializeAsAttribute(string name) : base(name) { }
    }
}
