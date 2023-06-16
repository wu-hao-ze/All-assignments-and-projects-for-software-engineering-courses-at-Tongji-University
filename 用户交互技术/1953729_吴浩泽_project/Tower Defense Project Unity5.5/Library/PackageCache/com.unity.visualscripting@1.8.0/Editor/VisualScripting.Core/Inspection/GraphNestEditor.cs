using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Editor(typeof(IGraphNest))]
    public class GraphNestEditor : Inspector
    {
        public GraphNestEditor(Metadata metadata) : base(metadata) { }

        private bool warnComponentPrefab => (GraphSource)sourceMetadata.value == GraphSource.Embed && (LudiqEditorUtility.editedObject.value?.IsConnectedPrefabInstance() ?? false);

        private bool warnBackgroundEmbed => ((IGraphNest)metadata.value).hasBackgroundEmbed;

        private const string ComponentPrefabWarning = "Changes made to embed graphs on prefabs will not be synced with the prefab instance. Consider using a macro instead.";

        private const string BackgroundEmbedWarning = "Background embed graph detected.";

        public static class Styles
        {
            static Styles()
            {
                convertButton = new GUIStyle(EditorStyles.miniButton);
                newButton = new GUIStyle(EditorStyles.miniButton);
                editButton = new GUIStyle("Button");
                editButton.fontSize = 13;
                fixBackgroundEmbedButton = new GUIStyle("Button");
                fixBackgroundEmbedButton.padding.left += 4;
                fixBackgroundEmbedButton.padding.right += 4;
            }

            public static readonly float spaceBeforeButton = 3;

            public static readonly float spaceBeforeEditButton = 3;

            public static readonly GUIStyle convertButton;

            public static readonly GUIStyle newButton;

            public static readonly GUIStyle editButton;

            public static readonly GUIStyle fixBackgroundEmbedButton;
        }

        protected virtual IGraphNester nester => (IGraphNester)nesterMetadata.value;

        protected virtual IGraph NewGraph()
        {
            return nester.DefaultGraph();
        }

        protected virtual GraphReference reference
        {
            get
            {
                if (nester is IGraphRoot root)
                {
                    return GraphReference.New(root, false);
                }

                if (nester is IGraphNesterElement nesterElement)
                {
                    return LudiqGraphsEditorUtility.editedContext.value.reference.ChildReference(nesterElement, false);
                }

                throw new NotSupportedException("Could not infer graph nest reference.");
            }
        }

        private void UpdateActiveGraph()
        {
            if (reference?.isRoot ?? false)
            {
                GraphWindow.activeReference = reference;
            }
        }

        #region Metadata

        protected Metadata nesterMetadata => metadata[nameof(IGraphNest.nester)];

        protected Metadata sourceMetadata => metadata[nameof(IGraphNest.source)];

        protected Metadata macroMetadata => metadata[nameof(IGraphNest.macro)];

        protected Metadata embedGraphMetadata => metadata[nameof(IGraphNest.embed)];

        protected Metadata macroGraphMetadata => macroMetadata[nameof(IMacro.graph)];

        protected Metadata graphMetadata => metadata[nameof(IGraphNest.graph)];

        protected Type graphType => ((IGraphNest)metadata.value).graphType;

        protected Type macroType => ((IGraphNest)metadata.value).macroType;

        #endregion


        #region Controls

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            height += EditorGUIUtility.standardVerticalSpacing;

            height += GetSourceHeight(width);

            var source = (GraphSource)sourceMetadata.value;

            if (source == GraphSource.Embed) { }
            else if (source == GraphSource.Macro)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += GetMacroHeight(width);
            }

            if (warnBackgroundEmbed)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += GetBackgroundEmbedWarningHeight(width);
            }

            height += EditorGUIUtility.standardVerticalSpacing;
            height += Styles.spaceBeforeEditButton;
            height += GetEditButtonHeight(width);
            height += EditorGUIUtility.standardVerticalSpacing;

            if (warnComponentPrefab)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += GetComponentPrefabWarningHeight(width);
            }

            height += EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, GUIContent.none);

            y += EditorGUIUtility.standardVerticalSpacing;

            var sourceAndConvertPosition = PrefixLabel(sourceMetadata, position.VerticalSection(ref y, GetSourceHeight(position.width)));

            var sourcePosition = new Rect
                (
                sourceAndConvertPosition.x,
                sourceAndConvertPosition.y,
                (sourceAndConvertPosition.width - Styles.spaceBeforeButton) / 2,
                sourceAndConvertPosition.height
                );

            var convertButtonPosition = new Rect
                (
                sourcePosition.xMax + Styles.spaceBeforeButton,
                sourceAndConvertPosition.y,
                (sourceAndConvertPosition.width - Styles.spaceBeforeButton) / 2,
                sourceAndConvertPosition.height - 1
                );

            OnSourceGUI(sourcePosition);

            var source = (GraphSource)sourceMetadata.value;

            if (source == GraphSource.Embed)
            {
                if (embedGraphMetadata.value == null)
                {
                    OnNewEmbedGraphButtonGUI(convertButtonPosition);
                }
                else
                {
                    OnConvertToMacroButtonGUI(convertButtonPosition);
                }
            }
            else if (source == GraphSource.Macro)
            {
                EditorGUI.BeginDisabledGroup(macroMetadata.value == null);
                OnConvertToEmbedButtonGUI(convertButtonPosition);
                EditorGUI.EndDisabledGroup();
            }

            if (source == GraphSource.Embed) { }
            else if (source == GraphSource.Macro)
            {
                y += EditorGUIUtility.standardVerticalSpacing;

                if (macroMetadata.value == null)
                {
                    var macroAndNewButtonPosition = PrefixLabel(macroMetadata, position.VerticalSection(ref y, GetMacroHeight(position.width)));

                    var macroPosition = new Rect
                        (
                        macroAndNewButtonPosition.x,
                        macroAndNewButtonPosition.y,
                        (macroAndNewButtonPosition.width - Styles.spaceBeforeButton) / 2,
                        macroAndNewButtonPosition.height
                        );

                    var newButtonPosition = new Rect
                        (
                        macroPosition.xMax + Styles.spaceBeforeButton,
                        macroAndNewButtonPosition.y,
                        (macroAndNewButtonPosition.width - Styles.spaceBeforeButton) / 2,
                        macroAndNewButtonPosition.height - 1
                        );

                    OnMacroGUI(macroPosition);
                    OnNewMacroButtonGUI(newButtonPosition);
                }
                else
                {
                    var macroPosition = PrefixLabel(macroMetadata, position.VerticalSection(ref y, GetMacroHeight(position.width)));

                    OnMacroGUI(macroPosition);
                }
            }

            if (warnBackgroundEmbed)
            {
                y += EditorGUIUtility.standardVerticalSpacing;

                OnBackgroundEmbedWarningGUI(position, ref y);
            }

            EndBlock(metadata);

            y += EditorGUIUtility.standardVerticalSpacing;

            y += Styles.spaceBeforeEditButton;

            OnEditButtonGUI(position.VerticalSection(ref y, GetEditButtonHeight(position.width)));

            y += EditorGUIUtility.standardVerticalSpacing;

            if (warnComponentPrefab)
            {
                y += EditorGUIUtility.standardVerticalSpacing;

                var componentPrefabWarningPosition = position.VerticalSection(ref y, GetComponentPrefabWarningHeight(position.width));

                OnComponentPrefabWarningGUI(componentPrefabWarningPosition);
            }
        }

        private float GetComponentPrefabWarningHeight(float width)
        {
            return LudiqGUIUtility.GetHelpBoxHeight(ComponentPrefabWarning, MessageType.Warning, width);
        }

        private float GetSourceHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, sourceMetadata, width);
        }

        private float GetMacroHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, macroMetadata, width);
        }

        private float GetEditButtonHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight + 7;
        }

        private float GetBackgroundEmbedWarningHeight(float width)
        {
            var fixContent = new GUIContent("Fix");

            var fixButtonWidth = Styles.fixBackgroundEmbedButton.CalcSize(fixContent).x - (Styles.spaceBeforeButton / 2);
            var warningWidth = width - fixButtonWidth - (Styles.spaceBeforeButton / 2);
            var warningHeight = LudiqGUIUtility.GetHelpBoxHeight(BackgroundEmbedWarning, MessageType.Warning, warningWidth);

            return warningHeight;
        }

        private void OnComponentPrefabWarningGUI(Rect position)
        {
            EditorGUI.HelpBox(position, ComponentPrefabWarning, MessageType.Warning);
        }

        private void OnBackgroundEmbedWarningGUI(Rect position, ref float y)
        {
            var fixContent = new GUIContent("Fix");

            var fixButtonWidth = Styles.fixBackgroundEmbedButton.CalcSize(fixContent).x - (Styles.spaceBeforeButton / 2);
            var warningWidth = position.width - fixButtonWidth - (Styles.spaceBeforeButton / 2);
            var warningHeight = LudiqGUIUtility.GetHelpBoxHeight(BackgroundEmbedWarning, MessageType.Warning, warningWidth);

            var warningPosition = new Rect
                (
                position.x,
                y,
                warningWidth,
                warningHeight
                );

            var fixButtonPosition = new Rect
                (
                warningPosition.xMax + Styles.spaceBeforeButton,
                y,
                fixButtonWidth,
                warningHeight
                );

            EditorGUI.HelpBox(warningPosition, BackgroundEmbedWarning, MessageType.Warning);

            if (GUI.Button(fixButtonPosition, fixContent))
            {
                if (EditorUtility.DisplayDialog("Background Embed Graph", "A background embed graph has been detected on this nest. This may cause slowdowns on serialization operations and unfixable background warnings. Do you want to delete the embed graph?", "Delete", "Cancel"))
                {
                    metadata.RecordUndo();
                    embedGraphMetadata.value = null;
                }
            }

            y += warningPosition.height;
        }

        private void OnSourceGUI(Rect position)
        {
            position = BeginLabeledBlock(sourceMetadata, position, GUIContent.none);

            var previousSource = (GraphSource)sourceMetadata.value;
            var newSource = (GraphSource)EditorGUI.EnumPopup(position, (Enum)sourceMetadata.value);

            if (EndBlock(sourceMetadata))
            {
                if (previousSource == GraphSource.Embed &&
                    newSource == GraphSource.Macro &&
                    embedGraphMetadata.value != null &&
                    !EditorUtility.DisplayDialog("Delete Embed Graph", "Switching to a macro source will delete the current embed graph. Are you sure you want to switch and delete the current embed graph?", "Switch", "Cancel"))
                {
                    return;
                }

                sourceMetadata.RecordUndo();

                if (previousSource == GraphSource.Embed && newSource == GraphSource.Macro)
                {
                    embedGraphMetadata.value = null;
                }
                else if (previousSource == GraphSource.Macro && newSource == GraphSource.Embed)
                {
                    macroMetadata.value = null;
                    embedGraphMetadata.value = NewGraph();
                }

                sourceMetadata.value = newSource;
                UpdateActiveGraph();
            }
        }

        private void OnMacroGUI(Rect position)
        {
            EditorGUI.BeginChangeCheck();

            LudiqGUI.Inspector(macroMetadata, position, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateActiveGraph();
            }
        }

        private void OnEditButtonGUI(Rect position)
        {
            EditorGUI.BeginDisabledGroup(reference == null);

            if (GUI.Button(position, "Edit Graph", Styles.editButton))
            {
                GraphWindow.OpenActive(reference);
            }

            EditorGUI.EndDisabledGroup();
        }

        private void OnConvertToMacroButtonGUI(Rect position)
        {
            if (GUI.Button(position, "Convert", Styles.convertButton))
            {
                var embedGraph = (IGraph)embedGraphMetadata.value;
                var hasSceneReferences = embedGraph.Serialize().objectReferences.Any(uo => uo.IsSceneBound());

                if (hasSceneReferences && !EditorUtility.DisplayDialog("Scene References Detected", "This graph contains references to objects in the scene that will be lost when converting to a macro. Are you sure you want to continue?", "Convert", "Cancel"))
                {
                    return;
                }

                var macro = (IMacro)ScriptableObject.CreateInstance(macroType);
                var macroObject = (UnityObject)macro;
                macro.graph = (IGraph)embedGraphMetadata.value.CloneViaSerialization();

                var path = EditorUtility.SaveFilePanelInProject("Save Graph", macroObject.name, "asset", null);

                if (!string.IsNullOrEmpty(path))
                {
                    metadata.RecordUndo();
                    AssetDatabase.CreateAsset(macroObject, path);
                    sourceMetadata.value = GraphSource.Macro;
                    macroMetadata.value = macro;
                    embedGraphMetadata.value = null;
                    UpdateActiveGraph();
                }
            }
        }

        private void OnConvertToEmbedButtonGUI(Rect position)
        {
            if (GUI.Button(position, "Convert", Styles.convertButton))
            {
                if (embedGraphMetadata.value == null || EditorUtility.DisplayDialog("Overwrite Embed Graph", "Are you sure you want to overwrite the current embed graph?", "Overwrite", "Cancel"))
                {
                    metadata.RecordUndo();
                    var newEmbedGraph = (IGraph)macroGraphMetadata.value.CloneViaSerialization();
                    sourceMetadata.value = GraphSource.Embed;
                    embedGraphMetadata.value = newEmbedGraph;
                    macroMetadata.value = null;
                    UpdateActiveGraph();
                }
            }
        }

        private void OnNewEmbedGraphButtonGUI(Rect position)
        {
            if (GUI.Button(position, "New", Styles.newButton))
            {
                metadata.RecordUndo();
                sourceMetadata.value = GraphSource.Embed;
                embedGraphMetadata.value = NewGraph();
                macroMetadata.value = null;
                UpdateActiveGraph();
            }
        }

        private void OnNewMacroButtonGUI(Rect position)
        {
            if (GUI.Button(position, "New", Styles.newButton))
            {
                var macro = (IMacro)ScriptableObject.CreateInstance(macroType);
                var macroObject = (UnityObject)macro;
                macro.graph = NewGraph();

                var path = EditorUtility.SaveFilePanelInProject("Save Graph", macroObject.name, "asset", null);

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(macroObject, path);
                    metadata.RecordUndo();
                    sourceMetadata.value = GraphSource.Macro;
                    macroMetadata.value = macro;
                    embedGraphMetadata.value = null;
                    UpdateActiveGraph();
                }
            }
        }

        #endregion
    }
}
