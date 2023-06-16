using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    // Simple inspector used by built in assets
    //  that only need to hide the script field
    class BasicAssetInspector : Editor, IInspectorChangeHandler
    {
        bool m_ShouldRebuild;
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                expanded = false;
                if (SkipField(property.propertyPath))
                    continue;
                EditorGUILayout.PropertyField(property, true);
            }

            m_ShouldRebuild = serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        public virtual void OnPlayableAssetChangedInInspector()
        {
            if (m_ShouldRebuild)
            {
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }

            m_ShouldRebuild = false;
        }

        static bool SkipField(string fieldName)
        {
            return fieldName == "m_Script";
        }
    }

    [CustomEditor(typeof(ActivationPlayableAsset))]
    class ActivationPlayableAssetInspector : BasicAssetInspector { }
}
