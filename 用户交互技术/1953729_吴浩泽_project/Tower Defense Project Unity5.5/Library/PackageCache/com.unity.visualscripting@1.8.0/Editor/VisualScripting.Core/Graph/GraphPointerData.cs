using System;
using System.Linq;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class GraphPointerData
    {
        [Serialize]
        private int rootObjectInstanceID;

        [Serialize]
        private Guid[] parentElementGuids;

        private GraphPointerData(GraphPointer pointer)
        {
            rootObjectInstanceID = pointer.rootObject.GetInstanceID();
            parentElementGuids = pointer.parentElementGuids.ToArray();
        }

        public static GraphPointerData FromPointer(GraphPointer pointer)
        {
            if (pointer == null || !pointer.isValid)
            {
                return null;
            }

            return new GraphPointerData(pointer);
        }

        public GraphReference ToReference(bool ensureValid)
        {
            return GraphReference.New(EditorUtility.InstanceIDToObject(rootObjectInstanceID), parentElementGuids, ensureValid);
        }
    }
}
