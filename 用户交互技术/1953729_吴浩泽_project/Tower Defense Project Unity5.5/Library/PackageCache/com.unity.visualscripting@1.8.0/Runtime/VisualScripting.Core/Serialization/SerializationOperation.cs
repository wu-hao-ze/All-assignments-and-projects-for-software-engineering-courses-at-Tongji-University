using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public class SerializationOperation
    {
        public SerializationOperation()
        {
            objectReferences = new List<UnityObject>();
            serializer = new fsSerializer();
            serializer.AddConverter(new UnityObjectConverter());
            serializer.AddConverter(new RayConverter());
            serializer.AddConverter(new Ray2DConverter());
            serializer.AddConverter(new NamespaceConverter());
            serializer.AddConverter(new LooseAssemblyNameConverter());
            serializer.Context.Set(objectReferences);
        }

        public fsSerializer serializer { get; private set; }
        public List<UnityObject> objectReferences { get; private set; }

        public void Reset()
        {
            objectReferences.Clear();
        }
    }
}
