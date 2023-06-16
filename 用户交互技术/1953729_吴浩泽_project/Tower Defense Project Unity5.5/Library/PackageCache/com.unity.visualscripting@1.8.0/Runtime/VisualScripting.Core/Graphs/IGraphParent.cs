using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public interface IGraphParent
    {
        IGraph childGraph { get; }

        bool isSerializationRoot { get; }

        UnityObject serializedObject { get; }

        IGraph DefaultGraph();
    }
}
