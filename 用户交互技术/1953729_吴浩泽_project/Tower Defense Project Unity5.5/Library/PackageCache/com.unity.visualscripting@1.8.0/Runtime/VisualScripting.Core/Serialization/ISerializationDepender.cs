using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface ISerializationDepender : ISerializationCallbackReceiver
    {
        IEnumerable<ISerializationDependency> deserializationDependencies { get; }

        void OnAfterDependenciesDeserialized();
    }
}
