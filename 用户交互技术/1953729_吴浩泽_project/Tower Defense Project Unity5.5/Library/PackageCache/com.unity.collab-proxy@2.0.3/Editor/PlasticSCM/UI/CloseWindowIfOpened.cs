using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class CloseWindowIfOpened
    {
        internal static void Plastic()
        {
            if (!EditorWindow.HasOpenInstances<PlasticWindow>())
                return;

            PlasticWindow window = EditorWindow.
                GetWindow<PlasticWindow>(null, false);

            window.Close();
        }
    }
}
