using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class LudiqEditorUtility
    {
        public static OverrideStack<UnityObject> editedObject { get; } = new OverrideStack<UnityObject>(null);
    }
}
