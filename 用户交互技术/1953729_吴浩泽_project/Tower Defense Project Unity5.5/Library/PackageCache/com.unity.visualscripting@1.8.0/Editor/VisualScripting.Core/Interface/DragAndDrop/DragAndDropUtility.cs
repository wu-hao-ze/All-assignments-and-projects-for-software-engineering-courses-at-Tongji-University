using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class DragAndDropUtility
    {
        public static Vector2 position => Event.current.mousePosition;
        public static Vector2 offsetedPosition => Event.current.mousePosition + mouseCursorOffset;

        public static bool Contains<T>() where T : UnityObject
        {
            return DragAndDrop.objectReferences.Any(o => o is T);
        }

        public static bool Is<T>()
        {
            return DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] is T;
        }

        public static T First<T>()
        {
            return DragAndDrop.objectReferences.OfType<T>().First();
        }

        public static T Single<T>()
        {
            return DragAndDrop.objectReferences.OfType<T>().Single();
        }

        public static T Get<T>()
        {
            return Single<T>();
        }

        public static readonly Vector2 mouseCursorOffset = new Vector2(23, 10);
    }
}
