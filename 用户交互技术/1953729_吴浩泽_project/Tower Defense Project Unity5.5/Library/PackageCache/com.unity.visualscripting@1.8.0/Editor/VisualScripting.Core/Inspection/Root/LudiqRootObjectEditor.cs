using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class LudiqRootObjectEditor : OptimizedEditor<LudiqRootObjectEditor.Individual>
    {
        public class Individual : IndividualEditor
        {
            protected override void Initialize()
            {
                dataProperty = serializedObject.FindPropertyOrFail("_data");

                metadata = Metadata.Root().StaticObject(serializedObject.targetObject);
            }

            private Metadata metadata;
            private SerializedProperty dataProperty;
            private Inspector inspector;
            private bool debugFoldout;

            public override void Dispose()
            {
                inspector?.Dispose();
            }

            public override void OnGUI()
            {
                if (EditorApplication.isCompiling)
                {
                    LudiqGUI.CenterLoader();
                    return;
                }

                using (LudiqEditorUtility.editedObject.Override(serializedObject.targetObject))
                {
                    if (PluginContainer.anyVersionMismatch)
                    {
                        LudiqGUI.VersionMismatchShieldLayout();
                        return;
                    }

                    if (inspector == null)
                    {
                        inspector = metadata.Editor();
                    }

                    EditorGUI.BeginChangeCheck();

                    LudiqGUI.Space(EditorGUIUtility.standardVerticalSpacing);

                    inspector.DrawLayout(GUIContent.none, 20);

                    if (EditorGUI.EndChangeCheck())
                    {
                        editorParent.Repaint();
                    }

                    if (BoltCore.instance != null && BoltCore.Configuration.developerMode)
                    {
                        debugFoldout = EditorGUILayout.Foldout(debugFoldout, "Developer", true);

                        if (debugFoldout)
                        {
                            var target = serializedObject.targetObject;

                            if (GUILayout.Button("Show Serialized Data"))
                            {
                                ((SerializationData)dataProperty.GetUnderlyingValue()).ShowString(target.ToString());
                            }

                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.PropertyField(dataProperty.FindPropertyRelativeOrFail("_" + nameof(SerializationData.objectReferences)));
                            EditorGUILayout.Toggle("Prefab definition", target.IsPrefabDefinition());
                            EditorGUILayout.Toggle("Prefab instance", target.IsPrefabInstance());
                            EditorGUILayout.Toggle("Connected prefab instance", target.IsConnectedPrefabInstance());
                            EditorGUILayout.Toggle("Disconnected prefab instance", target.IsDisconnectedPrefabInstance());
                            EditorGUILayout.Toggle("Scene bound", target.IsSceneBound());
                            EditorGUILayout.ObjectField("Prefab definition", target.GetPrefabDefinition(), typeof(UnityEngine.Object), true);
                            EditorGUI.EndDisabledGroup();
                        }
                    }
                    else
                    {
                        LudiqGUI.Space(EditorGUIUtility.standardVerticalSpacing);
                    }
                }
            }
        }
    }
}
