using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

using Debug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public abstract class Inspector : IDisposable
    {
        [OnOpenAsset(Int32.MinValue)]
        public static bool OnOpenVFX(int instanceID, int line)
        {
            UnityObject obj = EditorUtility.InstanceIDToObject(instanceID);
            GraphReference reference = null;
            if (obj is IMacro macro)
                reference = GraphReference.New(macro, true);
            else if (obj is IGraphRoot root)
                reference = GraphReference.New(root, false);
            if (obj is IGraphNesterElement nesterElement)
                reference = LudiqGraphsEditorUtility.editedContext.value.reference.ChildReference(nesterElement, false);
            if (reference == null)
                return false;
            GraphWindow.OpenActive(reference);
            return true;
        }

        protected Inspector(Metadata metadata)
        {
            Ensure.That(nameof(metadata)).IsNotNull(metadata);

            this.metadata = metadata;
            InitializeProfiling();
            metadata.valueChanged += previousValue => SetHeightDirty();

            warnBeforeEditingAttribute = metadata.GetAttribute<WarnBeforeEditingAttribute>();

            if (warnBeforeEditingAttribute != null)
            {
                editLocked = !warnBeforeEditingAttribute.emptyValues.Any(emptyValue => metadata.value == emptyValue);
            }
        }

        public virtual void Initialize() { }  // For more flexibility in call order
        private Exception failure;

        protected float y;
        public Metadata metadata { get; private set; }

        protected virtual bool safe => BoltCore.instance != null && BoltCore.Configuration != null && !BoltCore.Configuration.developerMode;

        protected static bool profile => BoltCore.instance != null && BoltCore.Configuration != null && BoltCore.Configuration.developerMode && BoltCore.Configuration.debugInspectorGUI;

        protected virtual bool indent => true;

        public virtual void Dispose() { }

        protected abstract float GetHeight(float width, GUIContent label);

        private readonly WarnBeforeEditingAttribute warnBeforeEditingAttribute;

        private bool editLocked;

        protected virtual bool SkipEvent(Event e, Rect position)
        {
            return e.ShouldSkip(position);
        }

        public void Draw(Rect position, GUIContent label = null)
        {
            if (SkipEvent(Event.current, position))
            {
                return;
            }

            if (failure == null)
            {
                try
                {
                    var oldIndent = EditorGUI.indentLevel;

                    if (!indent)
                    {
                        EditorGUI.indentLevel = 0;
                    }

                    EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling && !EditorApplicationUtility.isAssemblyReloadLocked);

                    var drawerPosition = position;

                    if (profile)
                    {
                        var drawerHeight = position.height - GetProfilingHeight();

                        drawerPosition = new Rect
                            (
                            position.x,
                            position.y,
                            position.width,
                            drawerHeight
                            );
                    }

                    if (editLocked)
                    {
                        if (GUI.Button(drawerPosition, GUIContent.none, GUIStyle.none))
                        {
                            if (EditorUtility.DisplayDialog(warnBeforeEditingAttribute.warningTitle, warnBeforeEditingAttribute.warningMessage, "Edit", "Cancel"))
                            {
                                editLocked = false;
                            }
                        }
                    }

                    EditorGUI.BeginDisabledGroup(editLocked);


                    y = drawerPosition.y;

                    if (metadata.HasAttribute<EditorPrefAttribute>())
                    {
                        BeginProfiling("OnEditorPrefGUI");
                        OnEditorPrefGUI(drawerPosition, ProcessLabel(metadata, label));
                        EndProfiling("OnEditorPrefGUI");
                    }
                    else
                    {
                        BeginProfiling("OnGUI");
                        OnGUI(drawerPosition, ProcessLabel(metadata, label));
                        EndProfiling("OnGUI");
                    }

                    EditorGUI.EndDisabledGroup();

                    if (profile)
                    {
                        var profilingPosition = new Rect
                            (
                            position.x,
                            drawerPosition.yMax,
                            position.width,
                            position.height - drawerPosition.height
                            );

                        OnProfilingGUI(profilingPosition);
                    }

                    EditorGUI.EndDisabledGroup();

                    if (!indent)
                    {
                        EditorGUI.indentLevel = oldIndent;
                    }
                }
                catch (ExitGUIException)
                {
                    // http://answers.unity3d.com/questions/385235
                    throw;
                }
                catch (Exception ex)
                {
                    if (safe)
                    {
                        failure = ex;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (safe && failure != null)
            {
                EditorGUI.HelpBox(position, $"Error drawing {GetType().Name}.", MessageType.Warning);

                if (GUI.Button(position, GUIContent.none, GUIStyle.none))
                {
                    Debug.LogException(failure);
                }
            }
        }

        protected abstract void OnGUI(Rect position, GUIContent label);

        protected virtual void OnEditorPrefGUI(Rect position, GUIContent label)
        {
            throw new NotImplementedException($"The Inspector for the EditorPref of type {metadata.definedType} did not override OnEditorPrefGUI");
        }

        internal static Stack<InspectorBlock> blockStack { get; private set; }

        #region Height Caching

        private float cachedHeight;
        private int cachedHeightHash;
        public bool isHeightDirty { get; private set; }
        private Inspector parentInspector;

        protected virtual bool cacheHeight => true;

        public void SetHeightDirty()
        {
            isHeightDirty = true;

            parentInspector?.SetHeightDirty();
        }

        public float GetCachedHeight(float width, GUIContent label, Inspector parentInspector)
        {
            try
            {
                label = ProcessLabel(metadata, label);

                BeginProfiling("GetHeight");

                this.parentInspector = parentInspector;

                var heightHash = GetHeightHash(width, label);

                if (!cacheHeight || isHeightDirty || heightHash != cachedHeightHash)
                {
                    cachedHeight = GetHeight(width, label);
                    cachedHeightHash = heightHash;
                    isHeightDirty = false;
                }

                failure = null;

                EndProfiling("GetHeight");

                if (profile)
                {
                    return cachedHeight + GetProfilingHeight();
                }
                else
                {
                    return cachedHeight;
                }
            }
            catch (ExitGUIException)
            {
                // http://answers.unity3d.com/questions/385235
                throw;
            }
            catch (Exception ex)
            {
                if (safe)
                {
                    failure = ex;
                    return LudiqGUIUtility.HelpBoxHeight;
                }
                else
                {
                    throw;
                }
            }
        }

        private int GetHeightHash(float width, GUIContent label)
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + width.GetHashCode();
                hash = hash * 23 + wideMode.GetHashCode();
                hash = hash * 23 + profile.GetHashCode();
                hash = hash * 23 + LudiqGUIUtility.currentInspectorWidthWithoutScrollbar.GetHashCode();
                hash = hash * 23 + LudiqGUIUtility.labelWidth.value.GetHashCode();

                if (label != null)
                {
                    if (label.text != null)
                    {
                        hash = hash * 23 + label.text.GetHashCode();
                    }

                    if (label.image != null)
                    {
                        hash = hash * 23 + label.image.GetHashCode();
                    }

                    if (label.tooltip != null)
                    {
                        hash = hash * 23 + label.tooltip.GetHashCode();
                    }
                }

                return hash;
            }
        }

        #endregion

        #region Helpers

        protected bool wideMode => LudiqGUIUtility.currentInspectorWidth > wideModeThreshold;

        protected float full => 1;

        protected float half => wideMode ? 0.5f : 1;

        protected virtual float wideModeThreshold => 332;

        public Rect GetLayoutPosition(GUIContent label = null, float scrollbarTrigger = 14, RectOffset offset = null)
        {
            var width = LudiqGUIUtility.GetLayoutWidth(offset);

            LudiqGUIUtility.currentInspectorHasScrollbar = width < LudiqGUIUtility.currentInspectorWidth - scrollbarTrigger;

            return LudiqGUIUtility.GetLayoutRect(GetCachedHeight(width, label, null), offset);
        }

        public void DrawLayout(GUIContent label = null, float scrollbarTrigger = 14, RectOffset offset = null)
        {
            Draw(GetLayoutPosition(label, scrollbarTrigger, offset), label);
        }

        protected static Event e => Event.current;

        protected static Rect ReclaimImplementationSelector(Rect position)
        {
            var selectorWidth = ImplementationInspector<object>.compactSelectorSubtractedWidth;
            position.x -= selectorWidth;
            position.width += selectorWidth;
            return position;
        }

        #endregion

        #region Blocks

        private static GUIStyle blockDebugBox;
        private static GUIStyle labelDebugBox;

        static Inspector()
        {
            blockDebugBox = new GUIStyle();
            blockDebugBox.normal.background = ColorUtility.CreateBox($"{EmbeddedResourceProvider.VISUAL_SCRIPTING_PACKAGE}.blockDebugBox", ColorPalette.transparent, Color.yellow);
            blockDebugBox.border = new RectOffset(1, 1, 1, 1);

            labelDebugBox = new GUIStyle();
            labelDebugBox.normal.background = ColorUtility.CreateBox($"{EmbeddedResourceProvider.VISUAL_SCRIPTING_PACKAGE}.labelDebugBox", ColorPalette.transparent, Color.blue);
            labelDebugBox.border = new RectOffset(1, 1, 1, 1);

            blockStack = new Stack<InspectorBlock>();
            EditorApplicationUtility.onSelectionChange += blockStack.Clear;
        }

        public static OverrideStack<bool> expandTooltip { get; } = new OverrideStack<bool>(false);

        public static OverrideStack<bool> adaptiveWidth { get; } = new OverrideStack<bool>(false);

        // HACK: Normally, we should be introducing a labelStyle parameter to OnGUI, GetHeight etc.
        // However this would require a significant refactor, and at this point it's only used for 1 case.
        // It would be something to implement along with the bigger inspector refactor, or maybe UIElements.
        public static OverrideStack<GUIStyle> defaultLabelStyle { get; } = new OverrideStack<GUIStyle>(null);

        public static GUIContent ProcessLabel(Metadata metadata, GUIContent label)
        {
            if (label == GUIContent.none)
            {
                return label;
            }

            if (label == null)
            {
                var attribute = metadata.GetAttribute<InspectorLabelAttribute>();

                if (attribute != null)
                {
                    label = new GUIContent(attribute.text, attribute.image ?? metadata.label.image, attribute.tooltip ?? metadata.label.tooltip);
                }
                else
                {
                    label = metadata.label ?? GUIContent.none;
                }
            }

            return label;
        }

        public static GUIStyle ProcessLabelStyle(Metadata metadata, GUIStyle labelStyle)
        {
            if (labelStyle == null)
            {
                labelStyle = defaultLabelStyle.value ?? EditorStyles.label;
            }

            if (metadata.isPrefabDiff)
            {
                labelStyle = LudiqGUIUtility.BoldedStyle(labelStyle);
            }

            return labelStyle;
        }

        private static float LabelWidth(Metadata metadata, float width)
        {
            var labelWidth = LudiqGUIUtility.labelWidth.value;

            var wideAttribute = metadata.GetAttribute<InspectorWideAttribute>();

            if (wideAttribute != null)
            {
                if (wideAttribute.toEdge)
                {
                    labelWidth = LudiqGUIUtility.currentInspectorWidth;
                }
                else
                {
                    labelWidth = width;
                }
            }

            return labelWidth;
        }

        public static float WidthWithoutLabel(Metadata metadata, float width, GUIContent label = null)
        {
            label = ProcessLabel(metadata, label);

            if (label == GUIContent.none)
            {
                return width;
            }

            return width - LabelWidth(metadata, width);
        }

        public static float HeightWithLabel(Metadata metadata, float width, float height, GUIContent label = null, GUIStyle labelStyle = null)
        {
            label = ProcessLabel(metadata, label);

            if (label == GUIContent.none)
            {
                return height;
            }

            var labelWidth = LabelWidth(metadata, width);

            labelStyle = ProcessLabelStyle(metadata, labelStyle);

            var wide = metadata.HasAttribute<InspectorWideAttribute>();

            var labelHeight = labelStyle.CalcHeight(label, labelWidth);

            if (expandTooltip.value || metadata.HasAttribute<InspectorExpandTooltipAttribute>())
            {
                var tooltipHeight = StringUtility.IsNullOrWhiteSpace(label.tooltip) ? 0 : LudiqStyles.expandedTooltip.CalcHeight(new GUIContent(label.tooltip), labelWidth);

                if (wide)
                {
                    height += labelHeight + tooltipHeight;
                }
                else
                {
                    height = Mathf.Max(height, labelHeight + tooltipHeight);
                }
            }
            else
            {
                if (wide)
                {
                    height += labelHeight;
                }
                else
                {
                    height = Mathf.Max(height, labelHeight);
                }
            }

            return height;
        }

        public static Rect PrefixLabel(Metadata metadata, Rect position, GUIContent label = null, GUIStyle style = null)
        {
            label = ProcessLabel(metadata, label);

            if (label == GUIContent.none)
            {
                return position;
            }

            var width = LabelWidth(metadata, position.width);

            style = ProcessLabelStyle(metadata, style);

            var wide = metadata.HasAttribute<InspectorWideAttribute>();

            var y = position.y;

            var labelPosition = new Rect
                (
                position.x,
                position.y,
                width,
                style.CalcHeight(label, width)
                );

            y = labelPosition.yMax + 2;

            var expandTooltip = Inspector.expandTooltip || metadata.HasAttribute<InspectorExpandTooltipAttribute>();

            EditorGUI.LabelField(labelPosition, expandTooltip ? new GUIContent(label.text, label.image) : label, style);

            if (BoltCore.instance != null && BoltCore.Configuration.developerMode && BoltCore.Configuration.debugInspectorGUI && e.type == EventType.Repaint)
            {
                labelDebugBox.Draw(labelPosition, false, false, false, false);
            }

            if (expandTooltip)
            {
                var tooltip = new GUIContent(label.tooltip);

                var tooltipPosition = new Rect
                    (
                    position.x,
                    y - 2,
                    width,
                    LudiqStyles.expandedTooltip.CalcHeight(tooltip, width)
                    );

                EditorGUI.LabelField(tooltipPosition, tooltip, LudiqStyles.expandedTooltip);

                if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debugInspectorGUI && e.type == EventType.Repaint)
                {
                    labelDebugBox.Draw(tooltipPosition, false, false, false, false);
                }

                y = tooltipPosition.yMax - 4;
            }

            Rect remainingPosition;

            if (wide)
            {
                remainingPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    position.height - labelPosition.height
                    );
            }
            else
            {
                remainingPosition = new Rect
                    (
                    labelPosition.xMax,
                    position.y,
                    position.width - labelPosition.width,
                    position.height
                    );
            }

            return remainingPosition;
        }

        public static void BeginBlock(Metadata metadata, Rect position)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(!metadata.isEditable);
            LudiqGUIUtility.editorHasBoldFont = metadata.isPrefabDiff;
            blockStack.Push(new InspectorBlock(metadata, position));
        }

        public static Rect BeginLabeledBlock(Metadata metadata, Rect position, GUIContent label = null, GUIStyle labelStyle = null)
        {
            BeginBlock(metadata, position);
            return PrefixLabel(metadata, position, label, labelStyle);
        }

        internal static InspectorBlock currentBlock => blockStack.Peek();

        public static bool EndBlock(Metadata metadata) // TODO: Remove metadata parameter
        {
            if (blockStack.Count > 0)
            {
                var block = blockStack.Pop();

                if (block.metadata != metadata)
                {
                    Debug.LogWarning("Inspector block metadata mismatch.");
                }

                if (e.type == EventType.ContextClick && block.position.Contains(e.mousePosition))
                {
                    if (block.metadata.isRevertibleToPrefab)
                    {
                        var menu = new GenericMenu();

                        menu.AddItem(new GUIContent($"Revert {block.metadata.label.text} to Prefab"), false, () => { block.metadata.RevertToPrefab(); });

                        menu.ShowAsContext();
                    }

                    e.Use();
                }

                if (profile && e.type == EventType.Repaint)
                {
                    blockDebugBox.Draw(block.position, false, false, false, false);
                }
            }
            else
            {
                Debug.LogWarning("Ending unstarted inspector block.");
            }

            LudiqGUIUtility.editorHasBoldFont = false;
            EditorGUI.EndDisabledGroup();
            return EditorGUI.EndChangeCheck();
        }

        public virtual float GetAdaptiveWidth()
        {
            return 50;
        }

        #endregion

        #region Profiling

        private Dictionary<string, Stopwatch> stopwatches;

        private void InitializeProfiling()
        {
            stopwatches = new Dictionary<string, Stopwatch>();

            ResetProfiling("GetHeight");
            ResetProfiling("OnGUI");
        }

        private static GUIStyle profilingLabel;

        private float GetProfilingHeight()
        {
            return 2 + GetProfilingLineHeight() * stopwatches.Count() + 10;
        }

        private float GetProfilingLineHeight()
        {
            if (profilingLabel == null)
            {
                profilingLabel = new GUIStyle(EditorStyles.miniLabel);
                profilingLabel.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
            }

            return profilingLabel.CalcHeight(GUIContent.none, 1000);
        }

        private void OnProfilingGUI(Rect profilingPosition)
        {
            var i = 0;

            var lineHeight = GetProfilingLineHeight();

            foreach (var stopwatch in stopwatches)
            {
                var profilingLinePosition = new Rect
                    (
                    profilingPosition.x,
                    profilingPosition.y + 2 + (lineHeight * i),
                    profilingPosition.width,
                    lineHeight
                    );

                OnProfilingLineGUI(profilingLinePosition, stopwatch.Key, stopwatch.Value);

                i++;
            }
        }

        protected void BeginProfiling(string name)
        {
            if (!profile)
            {
                return;
            }

            if (!stopwatches.ContainsKey(name))
            {
                stopwatches.Add(name, new Stopwatch());
            }
            else if (stopwatches[name] == null)
            {
                stopwatches[name] = new Stopwatch();
            }

            stopwatches[name].Reset();
            stopwatches[name].Start();
        }

        protected void EndProfiling(string name)
        {
            if (!profile)
            {
                return;
            }

            if (stopwatches[name] != null)
            {
                stopwatches[name].Stop();
            }
        }

        protected void ResetProfiling(string name)
        {
            if (stopwatches.ContainsKey(name))
            {
                stopwatches[name] = null; // Removing it would mess up the height cache because context doesn't consider stopwatches
            }
            else
            {
                stopwatches.Add(name, null);
            }
        }

        private void OnProfilingLineGUI(Rect profilingLinePosition, string label, Stopwatch stopwatch)
        {
            if (stopwatch != null && stopwatch.IsRunning)
            {
                Debug.LogWarningFormat("Missing EndProfiling call for '{0}'.", label);
            }

            EditorGUI.LabelField(profilingLinePosition, $"{label}: {(stopwatch != null ? stopwatch.ElapsedTicks.ToString() : "-")}", profilingLabel);
        }

        #endregion
    }
}
