using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class EditorVariablesUtility
    {
        static EditorVariablesUtility()
        {
            EditorApplicationUtility.onEnterEditMode += SavedVariables.OnEnterEditMode;
            EditorApplicationUtility.onExitEditMode += SavedVariables.OnExitEditMode;

            EditorApplicationUtility.onEnterEditMode += ApplicationVariables.OnEnterEditMode;
            EditorApplicationUtility.onExitEditMode += ApplicationVariables.OnExitEditMode;
        }

        public static bool isDraggingVariable => nullableKind != null;

        private static VariableKind? nullableKind => ((DragAndDrop.GetGenericData(DraggedListItem.TypeName) as DraggedListItem)?.sourceListAdaptor as VariableDeclarationsInspector.ListAdaptor)?.parentInspector?.kind;

        public static VariableKind kind => nullableKind.Value;

        public static VariableDeclaration declaration => ((DragAndDrop.GetGenericData(DraggedListItem.TypeName) as DraggedListItem)?.item as VariableDeclaration);

        public static IEnumerable<string> GetDynamicVariableNames(VariableKind kind, GraphReference reference)
        {
            var graph = reference.graph as IGraphWithVariables;

            if (graph == null)
            {
                return Enumerable.Empty<string>();
            }

            return graph.GetDynamicVariableNames(kind, reference);
        }

        public static IEnumerable<string> GetPredefinedVariableNames(VariableKind kind, GraphReference reference)
        {
            VariableDeclarations source = null;

            switch (kind)
            {
                case VariableKind.Flow:
                    break;

                case VariableKind.Graph:
                    {
                        source = Variables.Graph(reference);
                        break;
                    }

                case VariableKind.Object:
                    {
                        if (reference.gameObject != null)
                        {
                            source = Variables.Object(reference.gameObject);
                        }
                        break;
                    }

                case VariableKind.Scene:
                    {
                        if (reference.scene != null && Variables.ExistInScene(reference.scene))
                        {
                            source = Variables.Scene(reference.scene);
                        }
                        break;
                    }

                case VariableKind.Application:
                    {
                        source = Variables.Application;
                        break;
                    }

                case VariableKind.Saved:
                    {
                        source = Variables.Saved;
                        break;
                    }

                default:
                    throw new UnexpectedEnumValueException<VariableKind>(kind);
            }

            if (source == null)
            {
                return Enumerable.Empty<string>();
            }

            return source.Select(d => d.name).Where(name => !StringUtility.IsNullOrWhiteSpace(name)).OrderBy(name => name);
        }

        public static IEnumerable<string> GetVariableNameSuggestions(VariableKind kind, GraphReference reference)
        {
            return LinqUtility.Concat<string>(GetPredefinedVariableNames(kind, reference), GetDynamicVariableNames(kind, reference)).Distinct().OrderBy(name => name);
        }

        [MenuItem("GameObject/Visual Scripting Scene Variables", false, 12)]
        private static void MenuCommand(MenuCommand menuCommand)
        {
            var scene = SceneManager.GetActiveScene();

            if (SceneSingleton<SceneVariables>.InstantiatedIn(scene))
            {
                EditorUtility.DisplayDialog("Scene Variables", "The scene already contains a variables object.", "OK");
                return;
            }

            var go = SceneSingleton<SceneVariables>.InstanceIn(scene).gameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Scene Variables");
            Selection.activeObject = go;
        }

        public static void SaveVariableAsset(VariablesAsset asset, string fileName)
        {
            var path = Path.Combine(BoltCore.Paths.variableResources, fileName + ".asset");
            if (String.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset)))
            {
                var assetDatabasePath = PathUtility.FromProject(path);
                PathUtility.CreateParentDirectoryIfNeeded(path);
                AssetDatabase.CreateAsset(asset, assetDatabasePath);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
