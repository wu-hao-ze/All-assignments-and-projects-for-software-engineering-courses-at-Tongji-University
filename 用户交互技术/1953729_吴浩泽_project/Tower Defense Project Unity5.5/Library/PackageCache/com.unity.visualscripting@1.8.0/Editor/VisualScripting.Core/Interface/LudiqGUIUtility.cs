using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityResources = UnityEngine.Resources;

namespace Unity.VisualScripting
{
    public static class LudiqGUIUtility
    {
        static LudiqGUIUtility()
        {
            try
            {
                var binding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

                GUIClipType = typeof(GUIUtility).Assembly.GetType("UnityEngine.GUIClip", true);
                GUIClip_Unclip_Vector2 = GUIClipType.GetMethod("Unclip", binding, null, new[] { typeof(Vector2) }, null);
                GUIClip_Unclip_Rect = GUIClipType.GetMethod("Unclip", binding, null, new[] { typeof(Rect) }, null);
                GUIClip_Clip_Vector2 = GUIClipType.GetMethod("Clip", binding, null, new[] { typeof(Vector2) }, null);
                GUIClip_Clip_Rect = GUIClipType.GetMethod("Clip", binding, null, new[] { typeof(Rect) }, null);
                GUIClip_topmostRect = GUIClipType.GetProperty("topmostRect", binding);
                GUIClip_visibleRect = GUIClipType.GetProperty("visibleRect", binding);
                GUIClip_GetTopRect = GUIClipType.GetMethod("GetTopRect", binding, null, Empty<Type>.array, null);
                GUIClip_GetMatrix = GUIClipType.GetMethod("GetMatrix", binding, null, Empty<Type>.array, null);
                GUIClip_SetMatrix = GUIClipType.GetMethod("SetMatrix", binding, null, new[] { typeof(Matrix4x4) }, null);
                GUIClip_enabled = GUIClipType.GetProperty("enabled", binding);
                GUIClip_GetCount = GUIClipType.GetMethod("Internal_GetCount", binding, null, Empty<Type>.array, null);

                if (GUIClip_Unclip_Vector2 == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "Unclip");
                }

                if (GUIClip_Unclip_Rect == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "Unclip");
                }

                if (GUIClip_Clip_Vector2 == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "Clip");
                }

                if (GUIClip_Clip_Rect == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "Clip");
                }

                if (GUIClip_topmostRect == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "topmostRect");
                }

                if (GUIClip_visibleRect == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "visibleRect");
                }

                if (GUIClip_GetTopRect == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "GetTopRect");
                }

                if (GUIClip_GetMatrix == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "GetMatrix");
                }

                if (GUIClip_SetMatrix == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "SetMatrix");
                }

                if (GUIClip_enabled == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "enabled");
                }

                if (GUIClip_GetCount == null)
                {
                    throw new MissingMemberException(GUIClipType.FullName, "Internal_GetCount");
                }

                GUIStyle_CalcSizeWithConstraints = typeof(GUIStyle).GetMethod("CalcSizeWithConstraints", BindingFlags.Instance | BindingFlags.NonPublic);

                if (GUIStyle_CalcSizeWithConstraints == null)
                {
                    throw new MissingMemberException(typeof(GUIStyle).FullName, "CalcSizeWithConstraints");
                }

                EditorGUIUtility_GetHelpIcon = typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.Static | BindingFlags.NonPublic);
                EditorGUIUtility_GetBoldDefaultFont = typeof(EditorGUIUtility).GetMethod("GetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
                EditorGUIUtility_SetBoldDefaultFont = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
                EditorGUIUtility_s_LastControlID = typeof(EditorGUIUtility).GetField("s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic);

                if (EditorGUIUtility_GetHelpIcon == null)
                {
                    throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "GetHelpIcon");
                }

                if (EditorGUIUtility_GetBoldDefaultFont == null)
                {
                    throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "GetBoldDefaultFont");
                }

                if (EditorGUIUtility_SetBoldDefaultFont == null)
                {
                    throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "SetBoldDefaultFont");
                }

                if (EditorGUIUtility_s_LastControlID == null)
                {
                    throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "s_LastControlID");
                }

                InspectorWindowType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow", true);
                InspectorWindow_RepaintAllInspectors = InspectorWindowType.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);

                if (InspectorWindow_RepaintAllInspectors == null)
                {
                    throw new MissingMemberException("UnityEditor.InspectorWindow", "RepaintAllInspectors");
                }

                ContainerWindowType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ContainerWindow", true);
                ContainerWindow_m_ShowMode = ContainerWindowType.GetField("m_ShowMode", BindingFlags.Instance | BindingFlags.NonPublic);
                ContainerWindow_position = ContainerWindowType.GetProperty("position", BindingFlags.Instance | BindingFlags.Public);

                if (ContainerWindow_m_ShowMode == null)
                {
                    throw new MissingMemberException("UnityEditor.ContainerWindow", "m_ShowMode");
                }

                if (ContainerWindow_position == null)
                {
                    throw new MissingMemberException("UnityEditor.ContainerWindow", "position");
                }

                EditorWindow_ShowModal = typeof(EditorWindow).GetMethod("ShowModal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (EditorWindow_ShowModal == null)
                {
                    throw new MissingMemberException("UnityEditor.EditorWindow", "ShowModal");
                }

                ShowModeType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ShowMode", true);

                EditorWindow_ShowAsDropDown = typeof(EditorWindow).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == "ShowAsDropDown" && m.GetParameters().Length == 4);

                if (EditorWindow_ShowAsDropDown == null)
                {
                    throw new MissingMemberException("UnityEditor.EditorWindow", "ShowAsDropDown");
                }

                // Different signature in different Unity versions
                GUIUtility_guiDepth = typeof(GUIUtility).GetProperty("guiDepth", BindingFlags.Static | BindingFlags.NonPublic);
                GUIUtility_Internal_GetGUIDepth = typeof(GUIUtility).GetMethod("Internal_GetGUIDepth", BindingFlags.Static | BindingFlags.NonPublic);

                if (GUIUtility_guiDepth == null && GUIUtility_Internal_GetGUIDepth == null)
                {
                    throw new MissingMemberException("GUIUtility", "guiDepth");
                }
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        private static readonly Type GUIClipType;

        private static readonly MethodInfo GUIClip_Unclip_Vector2; // public static Vector2 Unclip(Vector2 pos)

        private static readonly MethodInfo GUIClip_Unclip_Rect; // public static Rect Unclip(Rect rect)

        private static readonly MethodInfo GUIClip_Clip_Vector2; // public static Vector2 Clip(Vector2 absolutePos)

        private static readonly MethodInfo GUIClip_Clip_Rect; // public static Rect Clip(Rect absoluteRect)

        private static readonly PropertyInfo GUIClip_topmostRect; // public static Rect topmostRect { get; }

        private static readonly PropertyInfo GUIClip_visibleRect; // public static Rect topmostRect { get; }

        private static readonly MethodInfo GUIClip_GetTopRect; // internal static Rect GetTopRect()

        private static readonly MethodInfo GUIClip_GetMatrix; // internal static Matrix4x4 GetMatrix()

        private static readonly MethodInfo GUIClip_SetMatrix; // internal static Matrix4x4 GetMatrix(Matrix4x4 matrix)

        private static readonly MethodInfo GUIStyle_CalcSizeWithConstraints; // internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)

        private static readonly PropertyInfo GUIClip_enabled; // public static extern bool enabled { get; }

        private static readonly MethodInfo GUIClip_GetCount; // internal static extern int Internal_GetCount();

        // 2018+
        private static readonly PropertyInfo GUIUtility_guiDepth; // internal static extern int guiDepth { get; }

        // 2017
        private static readonly MethodInfo GUIUtility_Internal_GetGUIDepth; // extern internal static  int Internal_GetGUIDepth () ;

        public static bool newSkin => EditorApplicationUtility.unityVersion >= "2019.3.0";

        private static readonly Vector2[] corners1 = new Vector2[4];

        private static readonly Vector2[] corners2 = new Vector2[4];

        public static Rect GUIToScreenRect(this Rect rect)
        {
            return new Rect(GUIUtility.GUIToScreenPoint(rect.position), rect.size);
        }

        public static Rect VerticalSection(this Rect rect, ref float y, float height)
        {
            var section = new Rect
                (
                rect.x,
                y,
                rect.width,
                height
                );

            y += height;

            return section;
        }

        public static RectOffset Clone(this RectOffset rectOffset)
        {
            return new RectOffset
            (
                rectOffset.left,
                rectOffset.right,
                rectOffset.top,
                rectOffset.bottom
            );
        }

        public static Rect ExpandBy(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x -= offset.left;
            rect.y -= offset.top;
            rect.width += offset.left + offset.right;
            rect.height += offset.top + offset.bottom;
            return rect;
        }

        public static Rect ShrinkBy(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x += offset.left;
            rect.y += offset.top;
            rect.width -= offset.left + offset.right;
            rect.height -= offset.top + offset.bottom;
            return rect;
        }

        public static Rect ExpandByX(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x -= offset.left;
            rect.width += offset.left + offset.right;
            return rect;
        }

        public static Rect ShrinkByX(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.x += offset.left;
            rect.width -= offset.left + offset.right;
            return rect;
        }

        public static Rect ExpandByY(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.y -= offset.top;
            rect.height += offset.top + offset.bottom;
            return rect;
        }

        public static Rect ShrinkByY(this Rect rect, RectOffset offset)
        {
            if (offset == null)
            {
                return rect;
            }

            rect.y += offset.top;
            rect.height -= offset.top + offset.bottom;
            return rect;
        }

        public static Rect Encompass(this Rect rect, Vector2 point)
        {
            if (rect.xMin > point.x)
            {
                rect.xMin = point.x;
            }

            if (rect.yMin > point.y)
            {
                rect.yMin = point.y;
            }

            if (rect.xMax < point.x)
            {
                rect.xMax = point.x;
            }

            if (rect.yMax < point.y)
            {
                rect.yMax = point.y;
            }

            return rect;
        }

        public static Rect Encompass(this Rect rect, Rect other)
        {
            rect = rect.Encompass(new Vector2(other.xMin, other.yMin));
            rect = rect.Encompass(new Vector2(other.xMin, other.yMax));
            rect = rect.Encompass(new Vector2(other.xMax, other.yMin));
            rect = rect.Encompass(new Vector2(other.xMax, other.yMax));

            return rect;
        }

        public static bool Encompasses(this Rect rect, Rect other)
        {
            return
                rect.Contains(new Vector2(other.xMin, other.yMin)) &&
                rect.Contains(new Vector2(other.xMin, other.yMax)) &&
                rect.Contains(new Vector2(other.xMax, other.yMin)) &&
                rect.Contains(new Vector2(other.xMax, other.yMax));
        }

        public static void ClosestPoints(Rect rect1, Rect rect2, out Vector2 point1, out Vector2 point2)
        {
            corners1[0] = new Vector2(rect1.xMin, rect1.yMin);
            corners1[1] = new Vector2(rect1.xMin, rect1.yMax);
            corners1[2] = new Vector2(rect1.xMax, rect1.yMin);
            corners1[3] = new Vector2(rect1.xMax, rect1.yMax);

            corners2[0] = new Vector2(rect2.xMin, rect2.yMin);
            corners2[1] = new Vector2(rect2.xMin, rect2.yMax);
            corners2[2] = new Vector2(rect2.xMax, rect2.yMin);
            corners2[3] = new Vector2(rect2.xMax, rect2.yMax);

            var minDistance = float.MaxValue;

            point1 = rect1.center;
            point2 = rect2.center;

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    var corner1 = corners1[i];
                    var corner2 = corners2[j];
                    var distance = Vector2.Distance(corner1, corner2);

                    if (Vector2.Distance(corner1, corner2) < minDistance)
                    {
                        point1 = corner1;
                        point2 = corner2;
                        minDistance = distance;
                    }
                }
            }
        }

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector2 Perpendicular1(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }

        public static Vector2 Perpendicular2(this Vector2 vector)
        {
            return new Vector2(-vector.y, vector.x);
        }

        public static Vector2 PixelPerfect(this Vector2 vector)
        {
            return new Vector2(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Rect PixelPerfect(this Rect rect)
        {
            return new Rect(rect.position.PixelPerfect(), rect.size.PixelPerfect());
        }

        public static Vector2 Normal(this Edge edge)
        {
            switch (edge)
            {
                case Edge.Left:
                    return Vector2.left;
                case Edge.Right:
                    return Vector2.right;
                case Edge.Top:
                    return Vector2.up;
                case Edge.Bottom:
                    return Vector2.down;
                default:
                    throw new UnexpectedEnumValueException<Edge>(edge);
            }
        }

        public static Edge Opposite(this Edge edge)
        {
            switch (edge)
            {
                case Edge.Left:
                    return Edge.Right;
                case Edge.Right:
                    return Edge.Left;
                case Edge.Top:
                    return Edge.Bottom;
                case Edge.Bottom:
                    return Edge.Top;
                default:
                    throw new UnexpectedEnumValueException<Edge>(edge);
            }
        }

        public static Vector2 GetEdgeCenter(this Rect rect, Edge edge)
        {
            switch (edge)
            {
                case Edge.Left:
                    return new Vector2(rect.xMin, rect.center.y);
                case Edge.Right:
                    return new Vector2(rect.xMax, rect.center.y);
                case Edge.Top:
                    return new Vector2(rect.center.x, rect.yMin);
                case Edge.Bottom:
                    return new Vector2(rect.center.x, rect.yMax);
                default:
                    throw new UnexpectedEnumValueException<Edge>(edge);
            }
        }

        public static bool CtrlOrCmd(this Event e)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return e.command;
            }

            return e.control;
        }

        public static bool IsRightMouseButton(this Event e)
        {
            if (Application.platform == RuntimePlatform.OSXEditor && e.control && e.button == (int)MouseButton.Left)
            {
                return true;
            }

            return e.button == (int)MouseButton.Right;
        }

        public static void NineSlice(this Rect r,
            RectOffset o,
            out Rect topLeft, out Rect topCenter, out Rect topRight,
            out Rect middleLeft, out Rect middleCenter, out Rect middleRight,
            out Rect bottomLeft, out Rect bottomCenter, out Rect bottomRight)
        {
            topLeft = new Rect
                (
                r.x,
                r.y,
                o.left,
                o.top
                );

            topCenter = new Rect
                (
                r.x + o.left,
                r.y,
                r.width - o.left - o.right,
                o.top
                );

            topRight = new Rect
                (
                r.xMax - o.right,
                r.y,
                o.right,
                o.top
                );

            middleLeft = new Rect
                (
                r.x,
                r.y + o.top,
                o.left,
                r.height - o.top - o.bottom
                );

            middleCenter = new Rect
                (
                r.x + o.left,
                r.y + o.top,
                r.width - o.left - o.right,
                r.height - o.top - o.bottom
                );

            middleRight = new Rect
                (
                r.xMax - o.right,
                r.y + o.top,
                o.right,
                r.height - o.top - o.bottom
                );

            bottomLeft = new Rect
                (
                r.x,
                r.yMax - o.bottom,
                o.left,
                o.bottom
                );

            bottomCenter = new Rect
                (
                r.x + o.left,
                r.yMax - o.bottom,
                r.width - o.left - o.bottom,
                o.bottom
                );

            bottomRight = new Rect
                (
                r.xMax - o.right,
                r.yMax - o.bottom,
                o.right,
                o.bottom
                );
        }

        private static int guiDepth
        {
            get
            {
                try
                {
                    if (GUIUtility_guiDepth != null)
                    {
                        return (int)GUIUtility_guiDepth.GetValue(null, null);
                    }
                    else if (GUIUtility_Internal_GetGUIDepth != null)
                    {
                        return (int)GUIUtility_Internal_GetGUIDepth.Invoke(null, Empty<object>.array);
                    }
                    else
                    {
                        throw new MissingMemberException("GUIUtility", "guiDepth");
                    }
                }
                catch (Exception ex)
                {
                    // Safeguard because I'm not 100% sure how reliable that property is e.g. in other threads.
                    // In the worst case, returning 0 should be fine because we're probably not in a GUI call.
                    // Warn to get reports if it happens though.
                    Debug.LogWarning("Fetching GUI depth failed, returning zero.\n" + ex);
                    return 0;
                }
            }
        }

        // In certain cases, mainly editor application callbacks like
        // undo/redo/selection change, we're not actually on GUI but
        // Unity keeps the GUI depth. This makes throwing ExitGUI
        // exceptions dangerous.
        public static void BeginNotActuallyOnGUI()
        {
            notOnGuiDepth++;
        }

        public static void EndNotActuallyOnGUI()
        {
            if (notOnGuiDepth == 0)
            {
                throw new InvalidOperationException();
            }

            notOnGuiDepth--;
        }

        private static int notOnGuiDepth = 0;

        public static bool isWithinGUI => notOnGuiDepth == 0 && guiDepth > 0;

        public static string EscapeRichText(string s)
        {
            // Unity rich text supports <material=...> and <color=...> tags, which
            // mess up rendering with generic types such as List<Material>.
            // Escape these edge cases with a zero-width non-breaking space character.
            return s.Replace("<Material>", "<\uFEFFMaterial>")
                .Replace("<Color>", "<\uFEFFColor>");
        }

        #region Textures

        public static Vector2 Size(this Texture2D texture)
        {
            return new Vector2(texture.width, texture.height);
        }

        #endregion


        #region Styles

        public static Vector2 CalcSizeWithConstraints(this GUIStyle style, GUIContent content, Vector2 constraints)
        {
            Ensure.That(nameof(style)).IsNotNull(style);

            return (Vector2)GUIStyle_CalcSizeWithConstraints.InvokeOptimized(style, content, constraints);
        }

        public static string DimString(string s)
        {
            return $"<color=#{ColorPalette.unityForegroundDim.ToHexString()}>{s}</color>";
        }

        #endregion


        #region Clipping

        public static Vector2 Unclip(Vector2 pos)
        {
            return (Vector2)GUIClip_Unclip_Vector2.InvokeOptimized(null, pos);
        }

        public static Rect Unclip(Rect rect)
        {
            return (Rect)GUIClip_Unclip_Rect.InvokeOptimized(null, rect);
        }

        public static Vector2 Clip(Vector2 absolutePos)
        {
            return (Vector2)GUIClip_Clip_Vector2.InvokeOptimized(null, absolutePos);
        }

        public static Rect Clip(Rect absoluteRect)
        {
            return (Rect)GUIClip_Clip_Rect.InvokeOptimized(null, absoluteRect);
        }

        public static Rect topmostClipRect => (Rect)GUIClip_topmostRect.GetValueOptimized(null);

        public static Rect topClipRect => (Rect)GUIClip_GetTopRect.InvokeOptimized(null);

        public static Rect visibleClipRect => (Rect)GUIClip_visibleRect.GetValueOptimized(null);

        public static int clipDepth => (int)GUIClip_GetCount.InvokeOptimized(null);

        public static bool clipEnabled => (bool)GUIClip_enabled.GetValueOptimized(null);

        public static Matrix4x4 clipMatrix
        {
            get => (Matrix4x4)GUIClip_GetMatrix.InvokeOptimized(null);
            set => GUIClip_SetMatrix.InvokeOptimized(null, value);
        }

        public struct ClipFixContext : IDisposable
        {
            public void Dispose()
            {
                EndClipFix();
            }
        }

        private static Matrix4x4 _oldClipMatrix;

        public static ClipFixContext fixedClip
        {
            get
            {
                BeginClipFix();
                return new ClipFixContext();
            }
        }

        public static void BeginClipFix()
        {
            //GUI.BeginClip(new Rect(Vector2.zero, visibleClipRect.size), Vector2.zero, Vector2.zero, true);
            //_oldClipMatrix = clipMatrix;
            //clipMatrix = Matrix4x4.identity;
        }

        public static void EndClipFix()
        {
            //clipMatrix = _oldClipMatrix;
            //GUI.EndClip();
        }

        #endregion


        #region Layout

        public const float scrollBarWidth = 15f;

        public static bool currentInspectorHasScrollbar { get; set; }

        public static float currentInspectorWidthWithoutScrollbar
        {
            get
            {
                var width = currentInspectorWidth.value;

                if (currentInspectorHasScrollbar)
                {
                    width -= scrollBarWidth;
                }

                return width;
            }
        }

        public static float GetLayoutWidth(RectOffset offset = null)
        {
            var width = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(0)).width;

            if (offset != null)
            {
                width -= offset.left;
                width -= offset.right;
            }

            return width;
        }

        public static Rect GetLayoutRect(float height, RectOffset offset = null)
        {
            if (offset != null)
            {
                height -= offset.top;
                height -= offset.bottom;
            }

            var position = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(height), GUILayout.ExpandWidth(true));

            if (offset != null)
            {
                position.width -= offset.left;
                position.width -= offset.right;
            }

            return position;
        }

        private static RectOffset windowOverdraw;

        public static void BeginScrollablePanel(Rect outerPosition, Func<float, float> getInnerHeight, out Rect innerPosition, ref Vector2 scroll, RectOffset overdraw = null)
        {
            Ensure.That(nameof(getInnerHeight)).IsNotNull(getInnerHeight);

            innerPosition = new Rect(Vector2.zero, outerPosition.size);

            innerPosition = innerPosition.ExpandByX(overdraw);

            innerPosition.height = getInnerHeight(innerPosition.width);

            currentInspectorWidth.BeginOverride(innerPosition.width);

            if (innerPosition.height > outerPosition.height)
            {
                innerPosition.width -= scrollBarWidth;
                innerPosition.height = getInnerHeight(innerPosition.width);
                currentInspectorHasScrollbar = true;
            }
            else
            {
                currentInspectorHasScrollbar = false;
            }

            innerPosition = innerPosition.ExpandByY(overdraw);

            scroll = GUI.BeginScrollView(outerPosition, scroll, innerPosition.ShrinkBy(overdraw));

            GUI.BeginClip(innerPosition, Vector2.zero, Vector2.zero, false);
        }

        public static void EndScrollablePanel()
        {
            GUI.EndClip();
            GUI.EndScrollView();
            currentInspectorWidth.EndOverride();
        }

        public static void BeginScrollableWindow(Rect windowPosition, Func<float, float> getInnerHeight, out Rect innerPosition, ref Vector2 scroll)
        {
            // Needs to be called from the main thread
            if (windowOverdraw == null)
            {
                windowOverdraw = new RectOffset(1, 1, 1, 1);
            }

            var outerPosition = new Rect(Vector2.zero, windowPosition.size);

            BeginScrollablePanel(outerPosition, getInnerHeight, out innerPosition, ref scroll, windowOverdraw);
        }

        public static void EndScrollableWindow()
        {
            EndScrollablePanel();
        }

        public static OverrideStack<float> labelWidth { get; } = new OverrideStack<float>
            (
            () => EditorGUIUtility.labelWidth,
            value => EditorGUIUtility.labelWidth = value
            );

        public static OverrideStack<int> iconSize { get; } = new OverrideStack<int>
            (
            () => (int)EditorGUIUtility.GetIconSize().x,
            value => EditorGUIUtility.SetIconSize(new Vector2(value, value))
            );

        public static void TryUse(this Event e)
        {
            if (e != null && e.type != EventType.Repaint && e.type != EventType.Layout)
            {
                e.Use();
            }
        }

        private static float? _currentInspectorWidthOverride;

        public static OverrideStack<float> currentInspectorWidth { get; } = new OverrideStack<float>
            (
            () => _currentInspectorWidthOverride ?? EditorGUIUtility.currentViewWidth,
            value => _currentInspectorWidthOverride = value,
            () => _currentInspectorWidthOverride = null
            );

        #endregion


        #region Controls

        private static readonly FieldInfo EditorGUIUtility_s_LastControlID; // internal static int EditorGUIUtility.s_LastControlID

        public static int GetLastControlID()
        {
            return (int)EditorGUIUtility_s_LastControlID.GetValue(null);
        }

        #endregion


        #region Inspector

        private static readonly Type InspectorWindowType; // internal class InspectorWindow : EditorWindow, IHasCustomMenu

        private static readonly MethodInfo InspectorWindow_RepaintAllInspectors; // internal static void InspectorWindow.RepaintAllInspectors()

        public static void RepaintAllInspectors()
        {
            InspectorWindow_RepaintAllInspectors.Invoke(null, new object[0]);
        }

        public static void FocusInspector()
        {
            EditorWindow.FocusWindowIfItsOpen(InspectorWindowType);
        }

        #endregion


        #region Windows

        private static readonly Type ContainerWindowType; // internal sealed class ContainerWindow : ScriptableObject

        private static readonly FieldInfo ContainerWindow_m_ShowMode; // private int ContainerWindow.m_ShowMode;

        private static readonly PropertyInfo ContainerWindow_position; // public Rect ContainerWindow.position;

        private static readonly MethodInfo EditorWindow_ShowModal; // internal void ShowModal()

        private static readonly MethodInfo EditorWindow_ShowAsDropDown; // internal void ShowAsDropDown(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder, ShowMode mode)

        private static readonly Type ShowModeType; // internal enum ShowMode

        public static Rect mainEditorWindowPosition
        {
            get
            {
                try
                {
                    var containerWindow = UnityResources.FindObjectsOfTypeAll(ContainerWindowType).FirstOrDefault(window => (int)ContainerWindow_m_ShowMode.GetValue(window) == 4);

                    if (containerWindow == null)
                    {
                        return new Rect(0, 0, Screen.width, Screen.height);
                    }

                    return (Rect)ContainerWindow_position.GetValue(containerWindow, null);
                }
                catch (Exception ex)
                {
                    throw new UnityEditorInternalException(ex);
                }
            }
        }

        public static void Center(this EditorWindow window)
        {
            var mainEditorWindowPosition = LudiqGUIUtility.mainEditorWindowPosition;

            window.position = new Rect
                (
                mainEditorWindowPosition.position + mainEditorWindowPosition.size / 2 - window.position.size / 2,
                window.position.size
                );
        }

        public static bool IsFocused(this EditorWindow window)
        {
            return EditorWindow.focusedWindow == window;
        }

        public static void ShowModal(this EditorWindow window)
        {
            EditorWindow_ShowModal.InvokeOptimized(window);
        }

        public static void ShowAsDropDownWithKeyboardFocus(this EditorWindow window, Rect buttonRect, Vector2 windowSize)
        {
            window.ShowAsDropDown(buttonRect, windowSize);

            GUIUtility.ExitGUI();
        }

        #endregion


        #region Help Box

        private static readonly MethodInfo EditorGUIUtility_GetHelpIcon; // internal static Texture2D GetHelpIcon(MessageType type)

        public const float HelpBoxHeight = 40;

        public static Texture2D GetHelpIcon(MessageType type)
        {
            try
            {
                return (Texture2D)EditorGUIUtility_GetHelpIcon.Invoke(null, new object[] { type });
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        public static float GetHelpBoxHeight(string message, MessageType messageType, float width)
        {
            return EditorStyles.helpBox.CalcHeight(new GUIContent(message, GetHelpIcon(messageType)), width);
        }

        #endregion


        #region Font Bolding

        private static readonly MethodInfo EditorGUIUtility_GetBoldDefaultFont; // internal static bool EditorGUIUtility.GetBoldDefaultFont()

        private static readonly MethodInfo EditorGUIUtility_SetBoldDefaultFont; // internal static void EditorGUIUtility.SetBoldDefaultFont(bool isBold)

        private static readonly Dictionary<GUIStyle, GUIStyle> boldedStyles = new Dictionary<GUIStyle, GUIStyle>();

        public static bool editorHasBoldFont
        {
            get
            {
                try
                {
                    return (bool)EditorGUIUtility_GetBoldDefaultFont.InvokeOptimized(null);
                }
                catch (Exception ex)
                {
                    throw new UnityEditorInternalException(ex);
                }
            }
            set
            {
                try
                {
                    EditorGUIUtility_SetBoldDefaultFont.InvokeOptimized(null, value);
                }
                catch (Exception ex)
                {
                    throw new UnityEditorInternalException(ex);
                }
            }
        }

        public static GUIStyle BoldedStyle(GUIStyle style)
        {
            if (!editorHasBoldFont)
            {
                return style;
            }

            if (!boldedStyles.ContainsKey(style))
            {
                var boldedStyle = new GUIStyle(style);
                boldedStyle.fontStyle = FontStyle.Bold;
                boldedStyles.Add(style, boldedStyle);
            }

            return boldedStyles[style];
        }

        #endregion


        #region Multiline

        public static float GetMultilineHeight(params float[] widths)
        {
            return GetMultilineHeightConfigurable(EditorGUIUtility.singleLineHeight, 2, widths);
        }

        public static Rect[] GetMultilinePositions(Rect totalPosition, params float[] widths)
        {
            return GetMultilinePositionsConfigurable(totalPosition, 2, 3, widths);
        }

        public static float GetMultilineHeightConfigurable(float lineHeight, float verticalSpacing, params float[] widths)
        {
            var total = 0f;

            for (var i = 0; i < widths.Length; i++)
            {
                total += widths[i];
            }

            var lines = Mathf.CeilToInt(total);

            return lineHeight * lines + verticalSpacing * (lines - 1);
        }

        public static Rect[] GetMultilinePositionsConfigurable(Rect totalPosition, float verticalSpacing, float horizontalSpacing, params float[] widths)
        {
            var totalWidth = 0f;

            for (var i = 0; i < widths.Length; i++)
            {
                totalWidth += widths[i];
            }

            var lines = Mathf.CeilToInt(totalWidth);

            var availableHeight = totalPosition.height - verticalSpacing * (lines - 1);

            var lineHeight = availableHeight / lines;

            var positions = new Rect[widths.Length];

            var currentY = 0f;

            var currentItem = 0;

            for (var line = 0; line < lines; line++)
            {
                var lineTotal = 0f;
                var itemsOnLine = 0;

                for (var i = currentItem; i < widths.Length; i++)
                {
                    lineTotal += widths[i];

                    if (lineTotal > 1)
                    {
                        break;
                    }

                    itemsOnLine++;
                }

                var currentX = 0f;

                var availableWidth = totalPosition.width - horizontalSpacing * (itemsOnLine - 1);

                for (var i = 0; i < itemsOnLine; i++)
                {
                    positions[currentItem] = new Rect
                        (
                        totalPosition.x + currentX,
                        totalPosition.y + currentY,
                        availableWidth * widths[currentItem],
                        lineHeight
                        );

                    currentX += positions[currentItem].width + horizontalSpacing;
                    currentItem++;
                }

                currentY += lineHeight + verticalSpacing;
            }

            return positions;
        }

        #endregion


        #region Events

        // This is basically a simple way of skipping expensive OnGUI calls early
        // https://support.ludiq.io/communities/5/topics/2187-performance-drop

        public static bool ShouldSkip(this Event e)
        {
            // So apparently skipping Ignore is bad, because Unity uses the event in a bunch
            // of controls, so skipping it changes the CID order.
            // https://answers.unity.com/questions/504017/not-redrawing-controls-on-eventtypeignore-results.html
            // https://books.google.ca/books?id=dS-YCgAAQBAJ&pg=PA138&lpg=PA138&dq=ignore+eventtype+unity&source=bl&ots=VoarspYsdc&sig=XSKr1hJqTwctj6zevZzsDmb6RGQ&hl=en&sa=X&ved=2ahUKEwiV2Kf7_djdAhVGPN8KHaBbCoUQ6AEwCHoECAIQAQ#v=onepage&q=ignore%20eventtype%20unity&f=false
            return e.type == EventType.Used; // || e.type == EventType.Ignore;
        }

        public static bool ShouldSkip(this Event e, Rect position)
        {
            // Unfortunately this optim will mess up CID ordering too...
            return e.ShouldSkip(); // || (e.type == EventType.MouseDrag && !position.Contains(e.mousePosition));
        }

        #endregion


        // Unity 2018.1 changelog: https://unity3d.com/unity/beta/unity2018.1.0b12
        // Editor: Plug-in code that creates textures used in rendering with IMGUI
        // should now avoid specifying them in linear space (i.e. should set the linear
        // parameter to false in the Texture2D constructor). Otherwise, GUI elements drawn
        // with such textures may look washed out when the project is working in Linear space
        // (Player Settings > Color space: Linear). (908904)
        public static bool createLinearTextures => EditorApplicationUtility.unityVersion.major < 2018 && PlayerSettings.colorSpace == ColorSpace.Linear;
    }
}
