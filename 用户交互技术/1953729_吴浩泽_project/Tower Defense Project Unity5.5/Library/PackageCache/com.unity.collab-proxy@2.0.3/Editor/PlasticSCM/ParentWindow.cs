using UnityEditor;

namespace Unity.PlasticSCM.Editor
{
    internal class ParentWindow
    {
        internal static EditorWindow Get()
        {
            if (EditorWindow.HasOpenInstances<PlasticWindow>())
                return EditorWindow.GetWindow<PlasticWindow>(false, null, false);

            return EditorWindow.focusedWindow;
        }
    }
}
