using System;
using Unity.VisualScripting.FullSerializer;

namespace Unity.VisualScripting
{
    public class NamespaceConverter : fsDirectConverter
    {
        public override Type ModelType => typeof(Namespace);

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new object();
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            serialized = new fsData(((Namespace)instance).FullName);

            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (!data.IsString)
            {
                return fsResult.Fail("Expected string in " + data);
            }

            instance = Namespace.FromFullName(data.AsString);

            return fsResult.Success;
        }
    }
}
