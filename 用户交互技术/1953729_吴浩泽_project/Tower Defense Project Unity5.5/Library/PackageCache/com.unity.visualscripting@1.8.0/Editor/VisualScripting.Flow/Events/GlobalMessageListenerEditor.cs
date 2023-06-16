using UnityEditor;

namespace Unity.VisualScripting
{
    [CustomEditor(typeof(GlobalMessageListener), true)]
    public class GlobalMessageListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is automatically added to relay Unity messages to Visual Scripting.", MessageType.Info);
        }
    }
}
