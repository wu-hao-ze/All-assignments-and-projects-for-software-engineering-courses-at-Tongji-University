using UnityEditor;

namespace Unity.VisualScripting
{
    [CustomEditor(typeof(MessageListener), true)]
    public class MessageListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is automatically added to relay Unity messages to Visual Scripting.", MessageType.Info);
        }
    }
}
