using UnityEngine;

namespace Unity.VisualScripting
{
    // The dependency must always implement the receiver,
    // because it must always notify the dependency manager of its deserialization.
    public interface ISerializationDependency : ISerializationCallbackReceiver
    {
        internal bool IsDeserialized { set; get; }
    }
}
