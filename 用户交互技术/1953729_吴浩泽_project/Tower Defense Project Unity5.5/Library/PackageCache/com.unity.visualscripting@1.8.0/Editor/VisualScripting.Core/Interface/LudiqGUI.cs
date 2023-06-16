using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using GUIEvent = UnityEngine.Event;

namespace Unity.VisualScripting
{
    public static class LudiqGUI
    {
        #region Overrides

        public static readonly OverrideStack<Color> color = new OverrideStack<Color>(
            () => GUI.color,
            (value) => GUI.color = value
        );

        public static readonly OverrideStack<Matrix4x4> matrix = new OverrideStack<Matrix4x4>(
            () => GUI.matrix,
            (value) => GUI.matrix = value
        );

        #endregion

        #region Drawing

        private static GUIStyle emptyRect;

        public static void DrawEmptyRect(Rect position, Color color)
        {
            if (emptyRect == null)
            {
                emptyRect = new GUIStyle();
                emptyRect.normal.background = ColorUtility.CreateBox($"{EmbeddedResourceProvider.VISUAL_SCRIPTING_PACKAGE}.emptyRect", ColorPalette.transparent, Color.white);
                emptyRect.border = new RectOffset(1, 1, 1, 1);
            }

            if (e.type == EventType.Repaint)
            {
                using (LudiqGUI.color.Override(color))
                {
                    emptyRect.Draw(position, false, false, false, false);
                }
            }
        }

        #endregion

        private static GUIEvent e => GUIEvent.current;

        public static void WindowHeader(string label, EditorTexture icon)
        {
            GUILayout.BeginVertical(LudiqStyles.windowHeaderBackground, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            LudiqGUI.FlexibleSpace();

            if (icon != null)
            {
                GUILayout.Box(new GUIContent(icon?[(int)LudiqStyles.windowHeaderIcon.fixedWidth]), LudiqStyles.windowHeaderIcon);
                LudiqGUI.Space(LudiqStyles.spaceBetweenWindowHeaderIconAndTitle);
            }

            GUILayout.Label(label, LudiqStyles.windowHeaderTitle);
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
            LudiqGUI.EndVertical();
        }

        #region Inspectors & Editors

        public static float GetInspectorHeight(Inspector parentInspector, Metadata metadata, float width, GUIContent label = null)
        {
            return metadata.Inspector().GetCachedHeight(width, label, parentInspector);
        }

        public static float GetInspectorAdaptiveWidth(Metadata metadata)
        {
            return metadata.Inspector().GetAdaptiveWidth();
        }

        public static void Inspector(Metadata metadata, Rect position, GUIContent label = null)
        {
            metadata.Inspector().Draw(position, label);
        }

        public static void InspectorLayout(Metadata metadata, GUIContent label = null, float scrollbarTrigger = 14, RectOffset offset = null)
        {
            metadata.Inspector().DrawLayout(label, scrollbarTrigger, offset);
        }

        public static float GetEditorHeight(Inspector parentInspector, Metadata metadata, float width)
        {
            return metadata.Editor().GetCachedHeight(width, GUIContent.none, parentInspector);
        }

        public static void Editor(Metadata metadata, Rect position)
        {
            metadata.Editor().Draw(position, GUIContent.none);
        }

        public static void EditorLayout(Metadata metadata)
        {
            metadata.Editor().DrawLayout(GUIContent.none);
        }

        #endregion

        #region Loaders

        public static readonly TextureResolution loaderResolution = new TextureResolution(loaderSize * loaderFrames, loaderSize);
        public const int loaderSize = 24;
        private const int loaderFrames = 12;
        private const float loaderSpeed = 12; // FPS
        private static EditorTexture temporaryLoader;

        public static void Loader(Rect position)
        {
            EditorTexture loader;

            if (PluginContainer.initialized)
            {
                loader = BoltCore.Resources.loader;
            }
            else
            {
                if (temporaryLoader == null && File.Exists(PluginPaths.resourcesBundle))
                {
                    var assetBundleResourceProvider = new AssetBundleResourceProvider(AssetUtility.AssetBundleEditor);
                    temporaryLoader = EditorTexture.Load(assetBundleResourceProvider, "Loader.png", CreateTextureOptions.PixelPerfect, true);
                }

                loader = temporaryLoader;
            }

            if (loader != null)
            {
                var frame = (int)(EditorApplication.timeSinceStartup * loaderSpeed % loaderFrames);
                var uv = new Rect((float)frame / loaderFrames, 0, 1f / loaderFrames, 1);
                GUI.DrawTextureWithTexCoords(position, loader[loaderSize], uv);
            }
        }

        public static void LoaderLayout()
        {
            Loader(GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(loaderSize), GUILayout.Height(loaderSize)));
        }

        public static void CenterLoader()
        {
            BeginVertical();
            FlexibleSpace();
            BeginHorizontal();
            FlexibleSpace();
            LoaderLayout();
            FlexibleSpace();
            EndHorizontal();
            FlexibleSpace();
            EndVertical();
        }

        #endregion

        #region Fields

        public static float GetTypeFieldHeight(GUIContent label, Type type)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public static float GetTypeFieldAdaptiveWidth(Type type, GUIContent nullLabel = null)
        {
            return Mathf.Max(18, EditorStyles.popup.CalcSize(GetTypeFieldPopupLabel(type)).x + 1);
        }

        private static GUIContent GetTypeFieldPopupLabel(Type type, GUIContent nullLabel = null)
        {
            GUIContent popupLabel;

            if (type != null)
            {
                popupLabel = new GUIContent(type.DisplayName(), type.Icon()?[IconSize.Small]);
            }
            else
            {
                if (nullLabel == null)
                {
                    nullLabel = new GUIContent("(No Type)");
                }

                popupLabel = nullLabel;
            }

            if (popupLabel.image != null)
            {
                popupLabel.text = " " + popupLabel.text;
            }

            return popupLabel;
        }

        public static Type TypeField(Rect position, GUIContent label, Type type, Func<IFuzzyOptionTree> getOptions, GUIContent nullLabel = null)
        {
            position = EditorGUI.PrefixLabel(position, label);

            return (Type)FuzzyPopup
                (
                position,
                getOptions,
                type,
                GetTypeFieldPopupLabel(type, nullLabel)
                );
        }

        public static Vector2 CompactVector2Field(Rect position, GUIContent label, Vector2 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var xPosition = new Rect
                (
                position.x,
                position.y,
                (position.width / 2) - (LudiqStyles.compactHorizontalSpacing) / 1,
                EditorGUIUtility.singleLineHeight
                );

            var yPosition = new Rect
                (
                xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                (position.width / 2) - (LudiqStyles.compactHorizontalSpacing) / 1,
                EditorGUIUtility.singleLineHeight
                );

            return new Vector2
            (
                DraggableFloatField(xPosition, value.x),
                DraggableFloatField(yPosition, value.y)
            );
        }

        public static Vector3 CompactVector3Field(Rect position, GUIContent label, Vector3 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var xPosition = new Rect
                (
                position.x,
                position.y,
                (position.width / 3) - (LudiqStyles.compactHorizontalSpacing) / 2,
                EditorGUIUtility.singleLineHeight
                );

            var yPosition = new Rect
                (
                xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                (position.width / 3) - (LudiqStyles.compactHorizontalSpacing) / 2,
                EditorGUIUtility.singleLineHeight
                );

            var zPosition = new Rect
                (
                yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                (position.width / 3) - (LudiqStyles.compactHorizontalSpacing) / 2,
                EditorGUIUtility.singleLineHeight
                );

            return new Vector3
            (
                DraggableFloatField(xPosition, value.x),
                DraggableFloatField(yPosition, value.y),
                DraggableFloatField(zPosition, value.z)
            );
        }

        public static Vector4 CompactVector4Field(Rect position, GUIContent label, Vector4 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var xPosition = new Rect
                (
                position.x,
                position.y,
                (position.width / 4) - (LudiqStyles.compactHorizontalSpacing) / 3,
                EditorGUIUtility.singleLineHeight
                );

            var yPosition = new Rect
                (
                xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                (position.width / 4) - (LudiqStyles.compactHorizontalSpacing) / 3,
                EditorGUIUtility.singleLineHeight
                );

            var zPosition = new Rect
                (
                yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                (position.width / 4) - (LudiqStyles.compactHorizontalSpacing) / 3,
                EditorGUIUtility.singleLineHeight
                );

            var wPosition = new Rect
                (
                zPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                (position.width / 4) - (LudiqStyles.compactHorizontalSpacing) / 3,
                EditorGUIUtility.singleLineHeight
                );

            return new Vector4
            (
                DraggableFloatField(xPosition, value.x),
                DraggableFloatField(yPosition, value.y),
                DraggableFloatField(zPosition, value.z),
                DraggableFloatField(wPosition, value.w)
            );
        }

        public static Vector2 AdaptiveVector2Field(Rect position, GUIContent label, Vector2 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var xPosition = new Rect
                (
                position.x,
                position.y,
                GetTextFieldAdaptiveWidth(value.x),
                EditorGUIUtility.singleLineHeight
                );

            var yPosition = new Rect
                (
                xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                GetTextFieldAdaptiveWidth(value.y),
                EditorGUIUtility.singleLineHeight
                );

            return new Vector2
            (
                DraggableFloatField(xPosition, value.x),
                DraggableFloatField(yPosition, value.y)
            );
        }

        public static Vector3 AdaptiveVector3Field(Rect position, GUIContent label, Vector3 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var xPosition = new Rect
                (
                position.x,
                position.y,
                GetTextFieldAdaptiveWidth(value.x),
                EditorGUIUtility.singleLineHeight
                );

            var yPosition = new Rect
                (
                xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                GetTextFieldAdaptiveWidth(value.y),
                EditorGUIUtility.singleLineHeight
                );

            var zPosition = new Rect
                (
                yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                GetTextFieldAdaptiveWidth(value.z),
                EditorGUIUtility.singleLineHeight
                );

            return new Vector3
            (
                DraggableFloatField(xPosition, value.x),
                DraggableFloatField(yPosition, value.y),
                DraggableFloatField(zPosition, value.z)
            );
        }

        public static Vector4 AdaptiveVector4Field(Rect position, GUIContent label, Vector4 value)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var xPosition = new Rect
                (
                position.x,
                position.y,
                GetTextFieldAdaptiveWidth(value.x),
                EditorGUIUtility.singleLineHeight
                );

            var yPosition = new Rect
                (
                xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                GetTextFieldAdaptiveWidth(value.y),
                EditorGUIUtility.singleLineHeight
                );

            var zPosition = new Rect
                (
                yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                GetTextFieldAdaptiveWidth(value.z),
                EditorGUIUtility.singleLineHeight
                );

            var wPosition = new Rect
                (
                zPosition.xMax + LudiqStyles.compactHorizontalSpacing,
                position.y,
                GetTextFieldAdaptiveWidth(value.w),
                EditorGUIUtility.singleLineHeight
                );

            return new Vector4
            (
                DraggableFloatField(xPosition, value.x),
                DraggableFloatField(yPosition, value.y),
                DraggableFloatField(zPosition, value.z),
                DraggableFloatField(wPosition, value.w)
            );
        }

        public static GUIContent GetEnumPopupContent(Enum value)
        {
            Ensure.That(nameof(value)).IsNotNull(value);

            if (EditorGUI.showMixedValue)
            {
                return new GUIContent("Mixed ...");
            }

            var enumType = value.GetType();

            if (enumType.HasAttribute<FlagsAttribute>())
            {
                var mask = Convert.ToInt64(value);

                if (mask == 0)
                {
                    return new GUIContent("None");
                }
                else if (mask == ~0)
                {
                    return new GUIContent("Everything");
                }

                var flags = Enum.GetValues(enumType).Cast<Enum>().ToArray();

                var activeFlagsCount = 0;

                foreach (var flag in flags)
                {
                    if (value.HasFlag(flag))
                    {
                        activeFlagsCount++;
                    }
                }

                if (activeFlagsCount == 0)
                {
                    return new GUIContent("None");
                }
                else if (activeFlagsCount == 1)
                {
                    return new GUIContent(value.ToString().Prettify());
                }
                else if (activeFlagsCount == flags.Length)
                {
                    return new GUIContent("Everything");
                }
                else
                {
                    return new GUIContent("Mixed ...");
                }
            }
            else
            {
                return new GUIContent(value.ToString().Prettify());
            }
        }

        public static int Spinner(Rect position, bool upEnabled = true, bool downEnabled = true)
        {
            var upPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                position.height / 2
                );

            var downPosition = new Rect
                (
                position.x,
                position.y + (position.height / 2),
                position.width,
                position.height / 2
                );

            EditorGUI.BeginDisabledGroup(!upEnabled);

            if (GUI.Button(upPosition, GUIContent.none, LudiqStyles.spinnerButton))
            {
                return 1;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!downEnabled);

            if (GUI.Button(downPosition, GUIContent.none, LudiqStyles.spinnerButton))
            {
                return -1;
            }

            EditorGUI.EndDisabledGroup();

            var arrow = LudiqStyles.spinnerDownArrow;

            var upArrowPosition = new Rect
                (
                upPosition.x + ((upPosition.width - arrow.width) / 2),
                upPosition.y + ((upPosition.height - arrow.height) / 2) + arrow.height - 1,
                arrow.width,
                -arrow.height
                );

            var downArrowPosition = new Rect
                (
                downPosition.x + ((downPosition.width - arrow.width) / 2),
                downPosition.y + ((downPosition.height - arrow.height) / 2) + 1,
                arrow.width,
                arrow.height
                );

            using (color.Override(upEnabled ? GUI.color : GUI.color.WithAlpha(0.3f)))
            {
                GUI.DrawTexture(upArrowPosition, arrow);
            }

            using (color.Override(downEnabled ? GUI.color : GUI.color.WithAlpha(0.3f)))
            {
                GUI.DrawTexture(downArrowPosition, arrow);
            }

            return 0;
        }

        #endregion

        #region Number Dragging

        // Lots of re-implementation from internal EditorGUI methods to allow us custom control trigger

        private static double CalculateDragSensitivityContinuous(double value)
        {
            if (Double.IsInfinity(value) || Double.IsNaN(value))
                return 0.0;
            return Math.Max(1.0, Math.Pow(Math.Abs(value), 0.5)) * 0.0299999993294477;
        }

        private static long CalculateDragSensitivityDiscrete(long value)
        {
            return (long)Math.Max(1.0, Math.Pow(Math.Abs((double)value), 0.5) * 0.0299999993294477);
        }

        private static NumberDragState numberDragState = NumberDragState.NotDragging;
        private static double numberDragStartValueContinuous;
        private static long numberDragStartValueDiscrete;
        private static Vector2 numberDragStartPosition;
        private static double numberDragSensitivity;
        private const float numberDragDeadZone = 16;
        private static readonly int numberDragControlIDHint = "DraggableFieldOverlay".GetHashCode();

        private enum NumberDragState
        {
            NotDragging,
            RequestedDragging,
            Dragging
        }

        public static long DraggableLongField(Rect position, long value, GUIContent label = null)
        {
            var controlId = GUIUtility.GetControlID(numberDragControlIDHint, FocusType.Passive, position);

            if (e.shift)
            {
                value = DragNumber(position, true, controlId, value);
            }

            return label != null ? EditorGUI.LongField(position, label, value) : EditorGUI.LongField(position, value);
        }

        public static float DraggableFloatField(Rect position, float value, GUIContent label = null)
        {
            var controlId = GUIUtility.GetControlID(numberDragControlIDHint, FocusType.Passive, position);

            if (e.shift)
            {
                value = DragNumber(position, true, controlId, value);
            }

            return label != null ? EditorGUI.FloatField(position, label, value) : EditorGUI.FloatField(position, value);
        }

        public static int DraggableIntField(Rect position, int value, GUIContent label = null)
        {
            var controlId = GUIUtility.GetControlID(numberDragControlIDHint, FocusType.Passive, position);

            if (e.shift)
            {
                value = DragNumber(position, true, controlId, value);
            }

            return label != null ? EditorGUI.IntField(position, label, value) : EditorGUI.IntField(position, value);
        }

        public static long DragNumber(Rect hotZone, bool deadZone, int controlId, long value)
        {
            double continuousValue = value;
            long discreteValue = 0;
            DragNumber(hotZone, deadZone, controlId, true, ref continuousValue, ref discreteValue);
            return (long)continuousValue;
        }

        public static float DragNumber(Rect hotZone, bool deadZone, int controlId, float value)
        {
            double continuousValue = value;
            long discreteValue = 0;
            DragNumber(hotZone, deadZone, controlId, true, ref continuousValue, ref discreteValue);
            return (float)continuousValue;
        }

        public static int DragNumber(Rect hotZone, bool deadZone, int controlId, int value)
        {
            double continuousValue = 0;
            long discreteValue = value;
            DragNumber(hotZone, deadZone, controlId, false, ref continuousValue, ref discreteValue);
            return (int)discreteValue;
        }

        private static void DragNumber(Rect hotZone, bool deadZone, int controlId, bool isContinuous, ref double continuousValue, ref long discreteValue)
        {
            var e = GUIEvent.current;

            switch (e.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    {
                        if (!hotZone.Contains(e.mousePosition) || e.button != (int)MouseButton.Left)
                        {
                            break;
                        }

                        EditorGUIUtility.editingTextField = false;
                        GUIUtility.hotControl = controlId;
                        GUIUtility.keyboardControl = controlId;

                        if (deadZone)
                        {
                            numberDragState = NumberDragState.RequestedDragging;
                        }
                        else
                        {
                            numberDragState = NumberDragState.Dragging;
                        }

                        numberDragStartValueContinuous = continuousValue;
                        numberDragStartValueDiscrete = discreteValue;
                        numberDragStartPosition = e.mousePosition;

                        if (isContinuous)
                        {
                            numberDragSensitivity = CalculateDragSensitivityContinuous(continuousValue);
                        }
                        else
                        {
                            numberDragSensitivity = CalculateDragSensitivityDiscrete(discreteValue);
                        }

                        e.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                        break;
                    }

                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl != controlId || numberDragState == NumberDragState.NotDragging)
                        {
                            break;
                        }

                        GUIUtility.hotControl = 0;
                        numberDragState = NumberDragState.NotDragging;
                        e.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        break;
                    }

                case EventType.MouseDrag:
                    {
                        if (GUIUtility.hotControl != controlId)
                        {
                            break;
                        }

                        switch (numberDragState)
                        {
                            case NumberDragState.RequestedDragging:
                                {
                                    if ((e.mousePosition - numberDragStartPosition).sqrMagnitude > numberDragDeadZone)
                                    {
                                        numberDragState = NumberDragState.Dragging;
                                        GUIUtility.keyboardControl = controlId;
                                    }
                                    e.Use();
                                    break;
                                }

                            case NumberDragState.Dragging:
                                {
                                    if (isContinuous)
                                    {
                                        continuousValue = continuousValue + HandleUtility.niceMouseDelta * numberDragSensitivity;
                                        continuousValue = RoundBasedOnMinimumDifference(continuousValue, numberDragSensitivity);
                                    }
                                    else
                                    {
                                        discreteValue = discreteValue + (long)Math.Round(HandleUtility.niceMouseDelta * numberDragSensitivity);
                                    }

                                    GUI.changed = true;
                                    e.Use();
                                    break;
                                }
                        }

                        break;
                    }

                case EventType.KeyDown:
                    {
                        if (GUIUtility.hotControl != controlId || e.keyCode != KeyCode.Escape || numberDragState == NumberDragState.NotDragging)
                        {
                            break;
                        }

                        continuousValue = numberDragStartValueContinuous;
                        discreteValue = numberDragStartValueDiscrete;
                        GUI.changed = true;
                        GUIUtility.hotControl = 0;
                        e.Use();
                        break;
                    }

                case EventType.Repaint:
                    {
                        EditorGUIUtility.AddCursorRect(hotZone, MouseCursor.SlideArrow);
                        break;
                    }
            }
        }

        private static double DiscardLeastSignificantDecimal(double v)
        {
            int digits = Math.Max(0, (int)(5d - Math.Log10(Math.Abs(v))));

            try
            {
                return Math.Round(v, digits);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0d;
            }
        }

        private static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
        {
            return (int)Math.Max(0d, -Math.Floor(Math.Log10(Math.Abs(minDifference))));
        }

        private static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
        {
            if (minDifference == 0d)
            {
                return DiscardLeastSignificantDecimal(valueToRound);
            }

            return Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Headers

        public delegate float GetHeaderTitleHeightDelegate(float innerWidth);

        public delegate float GetHeaderSummaryHeightDelegate(float innerWidth);

        public delegate void OnHeaderTitleGUIDelegate(Rect titlePosition);

        public delegate void OnHeaderSummaryGUIDelegate(Rect summaryPosition);

        public static float GetHeaderHeight
        (
            GetHeaderTitleHeightDelegate getTitleHeight,
            GetHeaderSummaryHeightDelegate getSummaryHeight,
            EditorTexture icon,
            float totalWidth,
            bool bottomMargin = true,
            float spaceBetweenTitleAndSummary = 0
        )
        {
            var innerWidth = GetHeaderInnerWidth(totalWidth, icon);

            var height = 0f;

            height += LudiqStyles.headerBackground.padding.top;

            var innerHeight = 0f;

            innerHeight += getTitleHeight(innerWidth);

            var summaryHeight = getSummaryHeight(innerWidth);

            if (summaryHeight > 0)
            {
                innerHeight += spaceBetweenTitleAndSummary;
                innerHeight += summaryHeight;
            }

            if (icon != null)
            {
                innerHeight = Mathf.Max(innerHeight, LudiqStyles.headerIcon.fixedHeight + LudiqStyles.headerIcon.margin.top);
            }

            height += innerHeight;

            height += LudiqStyles.headerBackground.padding.bottom;

            if (bottomMargin)
            {
                height += LudiqStyles.headerBackground.margin.bottom;
            }

            return height;
        }

        public static void OnHeaderGUI
        (
            GetHeaderTitleHeightDelegate getTitleHeight,
            GetHeaderSummaryHeightDelegate getSummaryHeight,
            OnHeaderTitleGUIDelegate onTitleGUI,
            OnHeaderSummaryGUIDelegate onSummaryGui,
            EditorTexture icon,
            Rect position,
            ref float y,
            bool bottomMargin = true,
            float spaceBetweenTitleAndSummary = 0
        )
        {
            var innerWidth = GetHeaderInnerWidth(position.width, icon);
            var x = position.x;

            var headerPosition = new Rect
                (
                x,
                y,
                position.width,
                GetHeaderHeight(getTitleHeight, getSummaryHeight, icon, position.width, false)
                );

            if (e.type == EventType.Repaint)
            {
                LudiqStyles.headerBackground.Draw(headerPosition, false, false, false, false);
            }

            x += LudiqStyles.headerBackground.padding.left;

            if (icon != null)
            {
                var iconPosition = new Rect
                    (
                    x,
                    y + LudiqStyles.headerBackground.padding.top + LudiqStyles.headerIcon.margin.top,
                    LudiqStyles.headerIcon.fixedWidth,
                    LudiqStyles.headerIcon.fixedHeight
                    );

                OnHeaderIconGUI(icon, iconPosition);

                x += iconPosition.width + LudiqStyles.headerIcon.margin.right;
            }

            var titlePosition = new Rect
                (
                x,
                y + LudiqStyles.headerBackground.padding.top,
                innerWidth,
                getTitleHeight(innerWidth)
                );

            onTitleGUI(titlePosition);

            var summaryHeight = getSummaryHeight(innerWidth);

            if (summaryHeight > 0)
            {
                var summaryPosition = new Rect
                    (
                    x,
                    titlePosition.yMax + spaceBetweenTitleAndSummary,
                    innerWidth,
                    summaryHeight
                    );

                onSummaryGui(summaryPosition);
            }

            y = headerPosition.yMax;

            if (bottomMargin)
            {
                y += LudiqStyles.headerBackground.margin.bottom;
            }
        }

        private static float GetHeaderInnerWidth(float totalWidth, EditorTexture icon)
        {
            var width = totalWidth;

            width -= LudiqStyles.headerBackground.padding.left;
            width -= LudiqStyles.headerBackground.padding.right;

            if (icon != null)
            {
                width -= LudiqStyles.headerIcon.fixedWidth;
                width -= LudiqStyles.headerIcon.margin.right;
            }

            return width;
        }

        private static void OnHeaderIconGUI(EditorTexture icon, Rect iconPosition)
        {
            if (icon != null && icon[IconSize.Medium])
            {
                GUI.DrawTexture(iconPosition, icon?[IconSize.Medium]);
            }
        }

        #region Static

        public static float GetHeaderHeight(GUIContent header, float totalWidth, bool bottomMargin = true)
        {
            var title = new GUIContent(header.text);
            var summary = new GUIContent(header.tooltip);
            var icon = EditorTexture.Single(header.image);

            return GetHeaderHeight
                (
                innerWidth => GetHeaderTitleHeight(title, innerWidth),
                innerWidth => GetHeaderSummaryHeight(summary, innerWidth),
                icon,
                totalWidth,
                bottomMargin,
                0
                );
        }

        public static void OnHeaderGUI(GUIContent header, Rect position, ref float y, bool bottomMargin = true)
        {
            var title = new GUIContent(header.text);
            var summary = new GUIContent(header.tooltip);
            var icon = EditorTexture.Single(header.image);

            OnHeaderGUI
            (
                innerWidth => GetHeaderTitleHeight(title, innerWidth),
                innerWidth => GetHeaderSummaryHeight(summary, innerWidth),
                titlePosition => OnHeaderTitleGUI(title, titlePosition),
                summaryPosition => OnHeaderSummaryGUI(summary, summaryPosition),
                icon,
                position,
                ref y,
                bottomMargin,
                0
            );
        }

        private static float GetHeaderTitleHeight(GUIContent title, float width)
        {
            return LudiqStyles.headerTitle.CalcHeight(title, width);
        }

        private static float GetHeaderSummaryHeight(GUIContent summary, float width)
        {
            if (StringUtility.IsNullOrWhiteSpace(summary.text))
            {
                return 0;
            }

            return LudiqStyles.headerSummary.CalcHeight(summary, width);
        }

        private static void OnHeaderTitleGUI(GUIContent title, Rect titlePosition)
        {
            EditorGUI.LabelField(titlePosition, title, LudiqStyles.headerTitle);
        }

        private static void OnHeaderSummaryGUI(GUIContent summary, Rect summaryPosition)
        {
            EditorGUI.LabelField(summaryPosition, summary, LudiqStyles.headerSummary);
        }

        #endregion

        #region Editable

        public static float GetHeaderHeight(Inspector parentInspector, Metadata titleMetadata, Metadata summaryMetadata, EditorTexture icon, float totalWidth, bool bottomMargin = true)
        {
            return GetHeaderHeight
                (
                innerWidth => GetHeaderTitleHeight(parentInspector, titleMetadata, innerWidth),
                innerWidth => GetHeaderSummaryHeight(parentInspector, summaryMetadata, innerWidth),
                icon,
                totalWidth,
                bottomMargin,
                EditorGUIUtility.standardVerticalSpacing
                );
        }

        public static void OnHeaderGUI(Metadata titleMetadata, Metadata summaryMetadata, EditorTexture icon, Rect position, ref float y, bool bottomMargin = true)
        {
            OnHeaderGUI
            (
                innerWidth => GetHeaderTitleHeight(null, titleMetadata, innerWidth),
                innerWidth => GetHeaderSummaryHeight(null, summaryMetadata, innerWidth),
                titlePosition => OnHeaderTitleGUI(titleMetadata, titlePosition),
                summaryPosition => OnHeaderSummaryGUI(summaryMetadata, summaryPosition),
                icon,
                position,
                ref y,
                bottomMargin,
                EditorGUIUtility.standardVerticalSpacing
            );
        }

        private static float GetHeaderTitleHeight(Inspector parentInspector, Metadata titleMetadata, float width)
        {
            return LudiqStyles.headerTitleField.fixedHeight;
        }

        private static float GetHeaderSummaryHeight(Inspector parentInspector, Metadata summaryMetadata, float width)
        {
            var attribute = summaryMetadata.GetAttribute<InspectorTextAreaAttribute>();

            var height = LudiqStyles.textAreaWordWrapped.CalcHeight(new GUIContent((string)summaryMetadata.value), width);

            if (attribute.hasMinLines)
            {
                var minHeight = EditorStyles.textArea.lineHeight * attribute.minLines + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

                height = Mathf.Max(height, minHeight);
            }

            if (attribute.hasMaxLines)
            {
                var maxHeight = EditorStyles.textArea.lineHeight * attribute.maxLines + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

                height = Mathf.Min(height, maxHeight);
            }

            return height;
        }

        private static void OnHeaderTitleGUI(Metadata titleMetadata, Rect titlePosition)
        {
            VisualScripting.Inspector.BeginLabeledBlock(titleMetadata, titlePosition, GUIContent.none);

            var hidable = !StringUtility.IsNullOrWhiteSpace((string)titleMetadata.value);

            var newTitle = EditorGUI.TextField(titlePosition, (string)titleMetadata.value, hidable ? LudiqStyles.headerTitleFieldHidable : LudiqStyles.headerTitleField);

            if (VisualScripting.Inspector.EndBlock(titleMetadata))
            {
                titleMetadata.RecordUndo();
                titleMetadata.value = newTitle;
            }

            if (String.IsNullOrEmpty((string)titleMetadata.value))
            {
                GUI.Label(titlePosition, $"({titleMetadata.label.text})", LudiqStyles.headerTitlePlaceholder);
            }
        }

        private static void OnHeaderSummaryGUI(Metadata summaryMetadata, Rect summaryPosition)
        {
            VisualScripting.Inspector.BeginLabeledBlock(summaryMetadata, summaryPosition, GUIContent.none);

            var hidable = !StringUtility.IsNullOrWhiteSpace((string)summaryMetadata.value);

            var newTitle = EditorGUI.TextArea(summaryPosition, (string)summaryMetadata.value, hidable ? LudiqStyles.headerSummaryFieldHidable : LudiqStyles.headerSummaryField);

            if (VisualScripting.Inspector.EndBlock(summaryMetadata))
            {
                summaryMetadata.RecordUndo();
                summaryMetadata.value = newTitle;
            }

            if (String.IsNullOrEmpty((string)summaryMetadata.value))
            {
                GUI.Label(summaryPosition, $"({summaryMetadata.label.text})", LudiqStyles.headerSummaryPlaceholder);
            }
        }

        #endregion

        #endregion

        #region Version Mismatch

        private const string VersionMismatchMessage = "Inspectors are disabled when plugin versions mismatch to prevent data corruption. ";

        public static float GetVersionMismatchShieldHeight(float width)
        {
            var height = 0f;

            height += LudiqGUIUtility.GetHelpBoxHeight(VersionMismatchMessage, MessageType.Warning, width);
            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public static void VersionMismatchShield(Rect position)
        {
            var warningPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                LudiqGUIUtility.GetHelpBoxHeight(VersionMismatchMessage, MessageType.Warning, position.width)
                );

            var buttonPosition = new Rect
                (
                position.x,
                warningPosition.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            EditorGUI.HelpBox(warningPosition, VersionMismatchMessage, MessageType.Warning);
        }

        public static void VersionMismatchShieldLayout()
        {
            LudiqGUI.BeginVertical();

            EditorGUILayout.HelpBox(VersionMismatchMessage, MessageType.Warning);

            LudiqGUI.EndVertical();
        }

        #endregion

        #region Lists

        private static float CalculateListWidth(IList<ListOption> options)
        {
            var width = 0f;

            foreach (var option in options)
            {
                width = Mathf.Max(width, LudiqStyles.listRow.CalcSize(option.label).x);
            }

            return width + LudiqGUIUtility.scrollBarWidth;
        }

        public static Vector2 List(Vector2 scroll, IList<ListOption> options, object selected, Action<object> selectionChanged)
        {
            var selectedIndex = options.IndexOf(options.FirstOrDefault(o => Equals(o.value, selected)));

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.DownArrow)
                {
                    selectionChanged(options[Mathf.Min(options.Count - 1, selectedIndex + 1)].value);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.UpArrow)
                {
                    selectionChanged(options[Mathf.Max(0, selectedIndex - 1)].value);
                    e.Use();
                }
            }

            var newScroll = EditorGUILayout.BeginScrollView(scroll, LudiqStyles.listBackground, GUILayout.Width(CalculateListWidth(options)));
            LudiqGUI.BeginVertical();

            foreach (var option in options)
            {
                var wasSelected = Equals(option.value, selected);
                var isSelected = ListOption(option.label, wasSelected);

                if (!wasSelected && isSelected)
                {
                    selectionChanged(option.value);
                }
            }

            LudiqGUI.EndVertical();
            GUILayout.EndScrollView();

            return newScroll;
        }

        private static bool ListOption(GUIContent label, bool selected)
        {
            if (!String.IsNullOrEmpty(label.tooltip))
            {
                label = new GUIContent($"{label.text}\n<size=10>{label.tooltip}</size>", label.image);
            }

            return GUILayout.Toggle(selected, label, LudiqStyles.listRow, GUILayout.ExpandWidth(true)) && !selected;
        }

        #endregion

        #region Standard Dropdowns

        public static void Dropdown
        (
            Vector2 position,
            Action<object> callback,
            IEnumerable<DropdownOption> options,
            object selected
        )
        {
            var hasMultipleDifferentValues = EditorGUI.showMixedValue;

            ICollection<DropdownOption> optionsCache = null;

            bool hasOptions;

            if (options != null)
            {
                optionsCache = options.AsReadOnlyCollection();
                hasOptions = optionsCache.Count > 0;
            }
            else
            {
                hasOptions = false;
            }

            var menu = new GenericMenu();

            GenericMenu.MenuFunction2 menuCallback = (o) =>
            {
                try
                {
                    callback(o);
                }
                catch (ExitGUIException) { }
            };

            if (hasOptions)
            {
                var wasSeparator = false;

                foreach (var option in optionsCache)
                {
                    var isSeparator = option is DropdownSeparator;

                    if (isSeparator)
                    {
                        if (!wasSeparator)
                        {
                            menu.AddSeparator(((DropdownSeparator)option).path);
                        }
                    }
                    else
                    {
                        var on = !hasMultipleDifferentValues && OptionValuesEqual(selected, option.value);

                        menu.AddItem(new GUIContent(option.label), @on, menuCallback, option.value);
                    }

                    wasSeparator = isSeparator;
                }
            }

            using (LudiqGUIUtility.fixedClip)
            {
                menu.DropDown(new Rect(position, Vector2.zero));
            }
        }

        public static void Dropdown
        (
            Vector2 position,
            Action<HashSet<object>> callback,
            IEnumerable<DropdownOption> options,
            HashSet<object> selected,
            bool showNothingEverything = true
        )
        {
            var hasMultipleDifferentValues = EditorGUI.showMixedValue;

            ICollection<DropdownOption> optionsCache = null;

            bool hasOptions;

            if (options != null)
            {
                optionsCache = options.AsReadOnlyCollection();
                hasOptions = optionsCache.Count > 0;
            }
            else
            {
                hasOptions = false;
            }

            var selectedCopy = selected.ToHashSet();

            // Remove options outside range
            selectedCopy.RemoveWhere(so => !optionsCache.Any(o => OptionValuesEqual(o.value, so)));

            var menu = new GenericMenu();

            // The callback when a normal option has been selected
            GenericMenu.MenuFunction2 switchCallback = (o) =>
            {
                var switchValue = o;

                if (selectedCopy.Contains(switchValue))
                {
                    selectedCopy.Remove(switchValue);
                }
                else
                {
                    selectedCopy.Add(switchValue);
                }

                try
                {
                    callback(selectedCopy);
                }
                catch (ExitGUIException) { }
            };

            // The callback when the special "Nothing" option has been selected
            GenericMenu.MenuFunction nothingCallback = () => { callback(new HashSet<object>()); };

            // The callback when the special "Everything" option has been selected
            GenericMenu.MenuFunction everythingCallback = () => { callback(optionsCache.Select((o) => o.value).ToHashSet()); };

            // Add the special "Nothing" / "Everything" options
            if (showNothingEverything)
            {
                menu.AddItem
                    (
                        new GUIContent("Nothing"),
                        !hasMultipleDifferentValues && selectedCopy.Count == 0,
                        nothingCallback
                    );

                if (hasOptions)
                {
                    menu.AddItem
                        (
                            new GUIContent("Everything"),
                            !hasMultipleDifferentValues && selectedCopy.Count == optionsCache.Count && selectedCopy.OrderBy(t => t).SequenceEqual(optionsCache.Select(o => o.value).OrderBy(t => t)),
                            everythingCallback
                        );
                }
            }

            // Add a separator (not in Unity default, but pretty)
            if (showNothingEverything && hasOptions)
            {
                menu.AddSeparator(String.Empty);
            }

            // Add the normal options
            if (hasOptions)
            {
                foreach (var option in optionsCache)
                {
                    menu.AddItem
                        (
                            new GUIContent(option.label),
                            !hasMultipleDifferentValues && (selectedCopy.Any(selectedValue => OptionValuesEqual(selectedValue, option.value))),
                            switchCallback,
                            option.value
                        );
                }
            }

            // Show the dropdown
            using (LudiqGUIUtility.fixedClip)
            {
                menu.DropDown(new Rect(position, Vector2.zero));
            }
        }

        #endregion

        #region Standard Popups

        public static object Popup
        (
            Rect position,
            Func<IEnumerable<DropdownOption>> getOptions,
            object selected,
            GUIContent label = null,
            GUIStyle style = null
        )
        {
            return ImmediatePopup
                (
                position,
                label ?? DefaultPopupLabel(selected),
                style,
                selected,
                () => Dropdown
                (
                    new Vector2(position.xMin, position.yMax),
                    UpdateImmediatePopupValue,
                    getOptions(),
                    selected
                )
                );
        }

        public static HashSet<object> Popup
        (
            Rect position,
            Func<IEnumerable<DropdownOption>> getOptions,
            HashSet<object> selected,
            bool showNothingEverything = true,
            GUIContent label = null,
            GUIStyle style = null
        )
        {
            return ImmediatePopup
                (
                position,
                label ?? DefaultPopupLabel(selected, getOptions().Count()),
                style,
                selected,
                () => Dropdown
                (
                    new Vector2(position.xMin, position.yMax),
                    UpdateImmediatePopupValue,
                    getOptions(),
                    selected,
                    showNothingEverything
                )
                );
        }

        #endregion

        #region Fuzzy Dropdowns

        public static void FuzzyDropdown
        (
            Rect activatorPosition,
            IFuzzyOptionTree optionTree,
            object selected,
            Action<object> callback
        )
        {
            optionTree.selected.Clear();
            optionTree.selected.Add(selected);

            FuzzyWindow.Show(activatorPosition, optionTree, (option) =>
            {
                callback(option.value);
                FuzzyWindow.instance.Close();
                InternalEditorUtility.RepaintAllViews();
            });
        }

        public static void FuzzyDropdown
        (
            Rect activatorPosition,
            IFuzzyOptionTree optionTree,
            HashSet<object> selected,
            Action<HashSet<object>> callback
        )
        {
            optionTree.selected.Clear();
            optionTree.selected.AddRange(selected);

            FuzzyWindow.Show(activatorPosition, optionTree, (option) =>
            {
                callback(optionTree.selected.ToHashSet());
                FuzzyWindow.instance.Close();
                InternalEditorUtility.RepaintAllViews();
            });
        }

        #endregion

        #region Fuzzy Popups

        public static object FuzzyPopup
        (
            Rect position,
            Func<IFuzzyOptionTree> getProvider,
            object selected,
            GUIContent label = null,
            GUIStyle style = null
        )
        {
            Ensure.That(nameof(getProvider)).IsNotNull(getProvider);

            return ImmediatePopup
                (
                position,
                label ?? DefaultPopupLabel(selected),
                style,
                selected,
                () => FuzzyDropdown
                (
                    position,
                    getProvider(),
                    selected,
                    UpdateImmediatePopupValue
                )
                );
        }

        public static HashSet<object> FuzzyPopup
        (
            Rect position,
            Func<IFuzzyOptionTree> getProvider,
            HashSet<object> selected,
            bool showNothingEverything = true,
            GUIContent label = null,
            GUIStyle style = null
        )
        {
            Ensure.That(nameof(getProvider)).IsNotNull(getProvider);

            return ImmediatePopup
                (
                position,
                label ?? DefaultPopupLabel(getProvider().selected),
                style,
                selected,
                () => FuzzyDropdown
                (
                    position,
                    getProvider(),
                    selected,
                    UpdateImmediatePopupValues
                )
                );
        }

        #endregion

        #region Immediate State Handling

        private static int activeActivatorControlID = -1;
        private static bool activeDropdownChanged;
        private static object activeDropdownValue;
        private static HashSet<object> activeDropdownValues;

        public static void UpdateImmediatePopupValue(object value)
        {
            activeDropdownValue = value;
            activeDropdownChanged = true;
        }

        public static void UpdateImmediatePopupValues(HashSet<object> value)
        {
            activeDropdownValues = value;
            activeDropdownChanged = true;
        }

        private static bool PopupActivatorRaw
        (
            int controlID,
            bool activated,
            Action dropdown
        )
        {
            if (activated)
            {
                // Assign the active control ID
                activeActivatorControlID = controlID;

                // Display the dropdown
                dropdown();
            }

            if (controlID == activeActivatorControlID && activeDropdownChanged)
            {
                GUI.changed = true;
                activeActivatorControlID = -1;
                activeDropdownChanged = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static object ImmediatePopup
        (
            int controlID,
            bool activated,
            object selected,
            Action dropdown
        )
        {
            if (PopupActivatorRaw(controlID, activated, dropdown))
            {
                return activeDropdownValue;
            }
            else
            {
                return selected;
            }
        }

        public static HashSet<object> ImmediatePopup
        (
            int controlID,
            bool activated,
            HashSet<object> selected,
            Action dropdown
        )
        {
            if (PopupActivatorRaw(controlID, activated, dropdown))
            {
                return activeDropdownValues;
            }
            else
            {
                return selected;
            }
        }

        private static readonly int PopupHash = "LudiqPopup".GetHashCode();

        private static bool PopupActivator
        (
            Rect position,
            GUIContent label,
            GUIStyle style,
            Action dropdown
        )
        {
            if (style == null)
            {
                style = EditorStyles.popup;
            }

            style = LudiqGUIUtility.BoldedStyle(style);

            // Render a button and get its control ID
            // Note: I'm having a really hard time ensuring control ID
            // continuity across immediate mode GUI. Internally, Unity
            // simply uses a static hash for its popups, but that doesn't
            // seem to be reliable. Using the internal s_LastControlID doesn't
            // seem to be constant across OnGUI calls either. Maybe it's because
            // I open another window in the mean time. To mitigate the effect,
            // we're using the current inspector block metadata to strenghten our
            // hash.
            var activatorControlID = GUIUtility.GetControlID(HashUtility.GetHashCode(PopupHash, VisualScripting.Inspector.currentBlock.metadata), FocusType.Keyboard, position);
            var activatorClicked = GUI.Button(position, label, style);
            //var activatorControlID = LudiqGUIUtility.GetLastControlID();

            if (activatorClicked)
            {
                // Cancel button click
                GUI.changed = false;
            }

            return PopupActivatorRaw(activatorControlID, activatorClicked, dropdown);
        }

        private static object ImmediatePopup
        (
            Rect position,
            GUIContent label,
            GUIStyle style,
            object selected,
            Action dropdown
        )
        {
            if (PopupActivator(position, label, style, dropdown))
            {
                return activeDropdownValue;
            }
            else
            {
                return selected;
            }
        }

        private static HashSet<object> ImmediatePopup
        (
            Rect position,
            GUIContent label,
            GUIStyle style,
            HashSet<object> selected,
            Action dropdown
        )
        {
            if (PopupActivator(position, label, style, dropdown))
            {
                return activeDropdownValues;
            }
            else
            {
                return selected;
            }
        }

        #endregion

        #region Popup Utility

        private static bool OptionValuesEqual(object a, object b)
        {
            return Equals(a, b);
        }

        private static GUIContent DefaultPopupLabel(object selectedValue)
        {
            string text;

            if (EditorGUI.showMixedValue)
            {
                text = "\u2014"; // Em Dash
            }
            else if (selectedValue != null)
            {
                text = selectedValue.ToString();
            }
            else
            {
                text = String.Empty;
            }

            return new GUIContent(text);
        }

        private static GUIContent DefaultPopupLabel(HashSet<object> selectedValues)
        {
            string text;

            if (EditorGUI.showMixedValue)
            {
                text = "\u2014"; // Em Dash
            }
            else
            {
                if (selectedValues.Count == 0)
                {
                    text = "Nothing";
                }
                else if (selectedValues.Count == 1)
                {
                    text = selectedValues.First().ToString();
                }
                else
                {
                    text = "(Multiple)";
                }
            }

            return new GUIContent(text);
        }

        private static GUIContent DefaultPopupLabel(HashSet<object> selectedValues, int totalOptionsCount)
        {
            string text;

            if (EditorGUI.showMixedValue)
            {
                text = "\u2014"; // Em Dash
            }
            else
            {
                if (selectedValues.Count == 0)
                {
                    text = "Nothing";
                }
                else if (selectedValues.Count == 1)
                {
                    text = selectedValues.First().ToString();
                }
                else if (selectedValues.Count == totalOptionsCount)
                {
                    text = "Everything";
                }
                else
                {
                    text = "(Mixed)";
                }
            }

            return new GUIContent(text);
        }

        #endregion

        #region Layout

        // Some editor GUI functions on Mac throw a NRE on Unity < 2018.2. Wrap them in safer methods.
        // https://forum.unity.com/threads/mac-os-various-gui-exceptions-after-os-dialogs-have-been-shown.515421/

        public static void Space(float pixels)
        {
            try
            {
                GUILayout.Space(pixels);
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        public static void FlexibleSpace()
        {
            try
            {
                GUILayout.FlexibleSpace();
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        public static void BeginHorizontal(params GUILayoutOption[] options)
        {
            try
            {
                GUILayout.BeginHorizontal(options);
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            try
            {
                GUILayout.BeginHorizontal(style, options);
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        public static void EndHorizontal()
        {
            try
            {
                GUILayout.EndHorizontal();
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        public static void BeginVertical(params GUILayoutOption[] options)
        {
            try
            {
                GUILayout.BeginVertical(options);
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        public static void EndVertical()
        {
            try
            {
                GUILayout.EndVertical();
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }

        #endregion

        public static float GetTextFieldAdaptiveWidth(object content, float min = 16)
        {
            return Mathf.Max(min, EditorStyles.textField.CalcSize(new GUIContent(content?.ToString())).x + 1);
        }
    }
}
