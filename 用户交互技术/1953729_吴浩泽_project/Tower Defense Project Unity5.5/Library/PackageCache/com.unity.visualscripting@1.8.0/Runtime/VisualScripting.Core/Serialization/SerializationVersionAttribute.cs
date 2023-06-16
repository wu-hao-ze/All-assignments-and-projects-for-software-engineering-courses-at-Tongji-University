using System;
using Unity.VisualScripting.FullSerializer;

namespace Unity.VisualScripting
{
    public class SerializationVersionAttribute : fsObjectAttribute
    {
        public SerializationVersionAttribute(string versionString, params Type[] previousModels) : base(versionString, previousModels) { }
    }
}
