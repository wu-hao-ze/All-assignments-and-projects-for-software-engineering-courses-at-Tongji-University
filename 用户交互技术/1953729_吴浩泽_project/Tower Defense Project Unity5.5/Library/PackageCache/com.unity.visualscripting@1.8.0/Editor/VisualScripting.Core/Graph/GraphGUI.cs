using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class GraphGUI
    {
        public const float MinZoomForControls = 0.7f;

        public const float MinZoom = 0.1f;

        public const float MaxZoom = 1;

        public const float ZoomSteps = 0.05f;

        private static readonly RectOffset sizeProjectionOffset = new RectOffset(1, 1, 1, 1);

        public static Event e => Event.current;

        private static readonly List<KeyValuePair<NodeColor, float>> currentColorMix = new List<KeyValuePair<NodeColor, float>>();

        public static GUIStyle GetNodeStyle(NodeShape shape, NodeColor color)
        {
            switch (shape)
            {
                case NodeShape.Square:
                    return Styles.squares[color];
                case NodeShape.Hex:
                    return Styles.hexes[color];
                default:
                    throw new UnexpectedEnumValueException<NodeShape>(shape);
            }
        }

        public static void Node(Rect position, NodeShape shape, NodeColor color, bool selected)
        {
            if (e.type == EventType.Repaint)
            {
                var outerPosition = GetNodeEdgeToOuterPosition(position, shape);

                GetNodeStyle(shape, color).Draw(FixNodePosition(outerPosition, shape, color, selected), false, false, false, selected);
            }
        }

        public static void Node(Rect position, NodeShape shape, NodeColorMix mix, bool selected)
        {
            if (e.type == EventType.Repaint)
            {
                mix.PopulateColorsList(currentColorMix);

                foreach (var color in currentColorMix)
                {
                    var outerPosition = GetNodeEdgeToOuterPosition(position, shape);

                    using (LudiqGUI.color.Override(GUI.color.WithAlphaMultiplied(color.Value)))
                    {
                        GetNodeStyle(shape, color.Key).Draw(FixNodePosition(outerPosition, shape, color.Key, selected), false, false, false, selected);
                    }
                }
            }
        }

        private static Rect FixNodePosition(Rect position, NodeShape shape, NodeColor color, bool selected)
        {
            // Some background images have weird offsets
            // Fix it on a case-by-case basis

            var offset = Vector2.zero;

            position.position += offset;

            return position;
        }

        public static Rect GetNodeEdgeToOuterPosition(Rect edgePosition, NodeShape shape)
        {
            return GetNodeStyle(shape, NodeColor.Gray).margin.Add(edgePosition);
        }

        public static Rect GetNodeEdgeToInnerPosition(Rect edgePosition, NodeShape shape)
        {
            return GetNodeStyle(shape, NodeColor.Gray).padding.Remove(edgePosition);
        }

        public static Rect GetNodeOuterToEdgePosition(Rect outerPosition, NodeShape shape)
        {
            return GetNodeStyle(shape, NodeColor.Gray).margin.Remove(outerPosition);
        }

        public static Rect GetNodeInnerToEdgePosition(Rect innerPosition, NodeShape shape)
        {
            return GetNodeStyle(shape, NodeColor.Gray).padding.Add(innerPosition);
        }

        public static void DrawBackground(Rect position)
        {
            if (e.type == EventType.Repaint)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    EditorGUI.DrawRect(position, new Color(0.125f, 0.125f, 0.125f));
                }
                else
                {
                    EditorGUI.DrawRect(position, new Color(0.45f, 0.45f, 0.45f));
                }
            }
        }

        public static float SnapToGrid(float position)
        {
            return MathfEx.NearestMultiple(position, Styles.minorGridSpacing);
        }

        public static Vector2 SnapToGrid(Vector2 position)
        {
            return new Vector2(SnapToGrid(position.x), SnapToGrid(position.y));
        }

        public static Rect SnapToGrid(Rect position, bool resize)
        {
            return new Rect(SnapToGrid(position.position), resize ? SnapToGrid(position.size) : position.size);
        }

        public static void DrawGrid(Vector2 scroll, Rect position, float zoom = 1)
        {
            if (e.type != EventType.Repaint)
            {
                return;
            }

            var i = 0;

            var drawMinor = zoom >= MinZoomForControls;

            var width = MathfEx.HigherMultiple(position.width, Styles.minorGridSpacing * Styles.majorGridGroup);

            for (var x = position.x; x < position.x + width; x += Styles.minorGridSpacing)
            {
                var xWrap = MathfEx.Wrap(x - scroll.x, width);

                if (i == 0)
                {
                    EditorGUI.DrawRect(new Rect
                        (
                            xWrap,
                            0,
                            Styles.majorGridThickness / zoom,
                            position.height
                        ), Styles.majorGridColor);
                }
                else if (drawMinor)
                {
                    EditorGUI.DrawRect(new Rect
                        (
                            xWrap,
                            0,
                            Styles.minorGridThickness / zoom,
                            position.height
                        ), Styles.minorGridColor);
                }

                i = (i + 1) % Styles.majorGridGroup;
            }

            var j = 0;

            var height = MathfEx.HigherMultiple(position.height, Styles.minorGridSpacing * Styles.majorGridGroup);

            for (var y = position.y; y < position.y + height; y += Styles.minorGridSpacing)
            {
                var yWrap = MathfEx.Wrap(y - scroll.y, height);

                if (j == 0)
                {
                    EditorGUI.DrawRect(new Rect
                        (
                            0,
                            yWrap,
                            position.width,
                            Styles.majorGridThickness / zoom
                        ), Styles.majorGridColor);
                }
                else if (drawMinor)
                {
                    EditorGUI.DrawRect(new Rect
                        (
                            0,
                            yWrap,
                            position.width,
                            Styles.minorGridThickness / zoom
                        ), Styles.minorGridColor);
                }

                j = (j + 1) % Styles.majorGridGroup;
            }

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                GUI.Label(new Rect(position.position, new Vector2(500, 16)), "Scroll: " + scroll, EditorStyles.whiteLabel);
                GUI.Label(new Rect(position.position + new Vector2(0, 16), new Vector2(500, 16)), "Position: " + position, EditorStyles.whiteLabel);
                GUI.Label(new Rect(position.position + new Vector2(0, 32), new Vector2(500, 16)), "Hot Controls: " + GUIUtility.hotControl + " / " + GUIUtility.keyboardControl, EditorStyles.whiteLabel);
            }
        }

        private static float GetAngleRelative(Vector2 start, Vector2 end)
        {
            var difference = (end - start).normalized;
            var angle = Mathf.Atan2(difference.y, difference.x) / (2 * Mathf.PI);
            if (angle < 0)
            {
                angle++;
            }
            return angle;
        }

        public static void GetConnectionEdge(Vector2 start, Vector2 end, out Edge startEdge, out Edge endEdge)
        {
            var angle = GetAngleRelative(start, end);

            if (angle >= (1 / 8f) && angle < (3 / 8f))
            {
                startEdge = Edge.Bottom;
                endEdge = Edge.Top;
            }
            else if (angle >= (3 / 8f) && angle < (5 / 8f))
            {
                startEdge = Edge.Left;
                endEdge = Edge.Right;
            }
            else if (angle >= (5 / 8f) && angle < (7 / 8f))
            {
                startEdge = Edge.Top;
                endEdge = Edge.Bottom;
            }
            else
            {
                startEdge = Edge.Right;
                endEdge = Edge.Left;
            }
        }

        public static void GetHorizontalConnectionEdge(Vector2 start, Vector2 end, out Edge startEdge, out Edge endEdge)
        {
            var angle = GetAngleRelative(start, end);

            if (angle >= (1 / 4f) && angle < (3 / 4f))
            {
                startEdge = Edge.Left;
                endEdge = Edge.Right;
            }
            else
            {
                startEdge = Edge.Right;
                endEdge = Edge.Left;
            }
        }

        public static EditorTexture ArrowTexture(Edge destinationEdge)
        {
            switch (destinationEdge)
            {
                case Edge.Left:
                    return Styles.arrowRight;
                case Edge.Right:
                    return Styles.arrowLeft;
                case Edge.Top:
                    return Styles.arrowDown;
                case Edge.Bottom:
                    return Styles.arrowUp;
                default:
                    throw new UnexpectedEnumValueException<Edge>(destinationEdge);
            }
        }

        public static void DrawConnectionArrow(Color color, Vector2 start, Vector2 end, Edge startEdge, Edge endEdge, float relativeBend = 1 / 4f, float minBend = 0)
        {
            DrawConnection(color, start, end, startEdge, endEdge, ArrowTexture(endEdge)?[24], new Vector2(8, 8), relativeBend, minBend);
        }

        public static Vector2 GetPointOnConnection(float t, Vector2 start, Vector2 end, Edge startEdge, Edge? endEdge, float relativeBend = 1 / 4f, float minBend = 0)
        {
            var startTangent = GetStartTangent(start, end, startEdge, endEdge, relativeBend, minBend);
            var endTangent = GetEndTangent(start, end, startEdge, endEdge, relativeBend, minBend);

            return MathfEx.Bezier(start, end, startTangent, endTangent, t);
        }

        public static void DrawConnection(Color color, Vector2 start, Vector2 end, Edge startEdge, Edge? endEdge, Texture cap = null, Vector2 capSize = default(Vector2), float relativeBend = 1 / 4f, float minBend = 0, float thickness = 3)
        {
            if (cap)
            {
                var capPosition = new Rect
                    (
                    end,
                    capSize
                    );

                Vector2 capOffset;
                Vector2 endOffset;

                if (endEdge.HasValue)
                {
                    switch (endEdge)
                    {
                        case Edge.Left:
                            capOffset = new Vector2(-capSize.x, -capSize.y / 2);
                            endOffset = new Vector2(capSize.x, 0);
                            break;

                        case Edge.Right:
                            capOffset = new Vector2(0, -capSize.y / 2);
                            endOffset = new Vector2(-capSize.x, 0);
                            break;

                        case Edge.Top:
                            capOffset = new Vector2(-capSize.x / 2, -capSize.y);
                            endOffset = new Vector2(0, capSize.y);
                            break;

                        case Edge.Bottom:
                            capOffset = new Vector2(-capSize.x / 2, 0);
                            endOffset = new Vector2(0, -capSize.y);
                            break;

                        default:
                            throw new UnexpectedEnumValueException<Edge>(endEdge.Value);
                    }
                }
                else
                {
                    capOffset = new Vector2(-capSize.x / 2, -capSize.y / 2);
                    endOffset = Vector2.zero;
                }

                capPosition.position += capOffset;
                end -= endOffset;

                if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
                {
                    EditorGUI.DrawRect(capPosition, new Color(0, 0, 1, 0.25f));
                }

                using (LudiqGUI.color.Override(LudiqGUI.color.value * color))
                {
                    GUI.DrawTexture(capPosition, cap);
                }
            }

            var startTangent = GetStartTangent(start, end, startEdge, endEdge, relativeBend, minBend);
            var endTangent = GetEndTangent(start, end, startEdge, endEdge, relativeBend, minBend);

            Handles.DrawBezier(start, end, startTangent, endTangent, LudiqGUI.color.value * color, AliasedBezierTexture(thickness), thickness);

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                Handles.color = Color.yellow;
                Handles.DrawLine(start, startTangent);
                Handles.DrawLine(end, endTangent);
            }
        }

        private static Vector2 GetStartTangent(Vector2 start, Vector2 end, Edge startEdge, Edge? endEdge, float relativeBend, float minBend)
        {
            var startDirection = startEdge.Normal();

            var startBend = Mathf.Abs(Vector2.Dot(end - start, startDirection)) * relativeBend;

            if (startDirection.y != 0)
            {
                startBend *= -1;
            }

            if (Mathf.Abs(startBend) < Mathf.Abs(minBend))
            {
                startBend = Mathf.Sign(startBend) * minBend;
            }

            var startTangent = start + startDirection * startBend;

            return startTangent;
        }

        private static Vector2 GetEndTangent(Vector2 start, Vector2 end, Edge startEdge, Edge? endEdge, float relativeBend, float minBend)
        {
            var endDirection = endEdge?.Normal() ?? startEdge.Opposite().Normal();

            var endBend = Mathf.Abs(Vector2.Dot(start - end, endDirection)) * relativeBend;

            if (endDirection.y != 0)
            {
                endBend *= -1;
            }

            if (Mathf.Abs(endBend) < Mathf.Abs(minBend))
            {
                endBend = Mathf.Sign(endBend) * minBend;
            }

            var endTangent = end + endDirection * endBend;

            return endTangent;
        }

        public static bool PositionOverlaps(ICanvas canvas, IGraphElementWidget widget, float threshold = 3)
        {
            var position = widget.position;

            return canvas.graph.elements.Any(otherElement =>
            {
                // Skip itself, which would by definition always overlap
                if (otherElement == widget.element)
                {
                    return false;
                }

                var positionA = canvas.Widget(otherElement).position;
                var positionB = position;

                return Mathf.Abs(positionA.xMin - positionB.xMin) < threshold &&
                Mathf.Abs(positionA.yMin - positionB.yMin) < threshold;
            });
        }

        public static Vector2? LineIntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            var A1 = end1.y - start1.y;
            var B1 = start1.x - end1.x;
            var C1 = A1 * start1.x + B1 * start1.y;

            var A2 = end2.y - start2.y;
            var B2 = start2.x - end2.x;
            var C2 = A2 * start2.x + B2 * start2.y;

            var delta = A1 * B2 - A2 * B1;

            if (delta == 0)
            {
                return null;
            }

            return new Vector2
            (
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }

        public static float SizeProjection(Vector2 size, Vector2 spreadOrigin, Vector2 spreadAxis)
        {
            var rect = new Rect(spreadOrigin - size / 2, size);

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                EditorGUI.DrawRect(rect, new Color(0, 1, 0, 0.1f));
            }

            var topLeft = new Vector2(rect.xMin, rect.yMin);
            var bottomLeft = new Vector2(rect.xMin, rect.yMax);
            var topRight = new Vector2(rect.xMax, rect.yMin);
            var bottomRight = new Vector2(rect.xMax, rect.yMax);

            var perp1 = spreadOrigin + spreadAxis;
            var perp2 = spreadOrigin - spreadAxis;

            // Vertical
            var vert1 = LineIntersectionPoint(topLeft, bottomLeft, perp1, perp2);
            var vert2 = LineIntersectionPoint(topRight, bottomRight, perp1, perp2);

            if (!vert1.HasValue || !vert2.HasValue)
            {
                return Vector2.Distance(topLeft, bottomLeft);
            }

            if (sizeProjectionOffset.Add(rect).Contains(vert1.Value))
            {
                return Vector2.Distance(vert1.Value, vert2.Value);
            }

            // Horizontal
            var horiz1 = LineIntersectionPoint(topLeft, topRight, perp1, perp2);
            var horiz2 = LineIntersectionPoint(bottomLeft, bottomRight, perp1, perp2);

            if (!horiz1.HasValue || !horiz2.HasValue)
            {
                return Vector2.Distance(topLeft, topRight);
            }

            if (sizeProjectionOffset.Add(rect).Contains(horiz1.Value))
            {
                return Vector2.Distance(horiz1.Value, horiz2.Value);
            }

            throw new ArithmeticException("Centered rect is not in spread axis.");
        }

        public static Rect CalculateArea(IEnumerable<IGraphElementWidget> widgets)
        {
            var assigned = false;
            var area = Rect.zero;

            foreach (var widget in widgets)
            {
                if (!assigned)
                {
                    area = widget.position;
                    assigned = true;
                }
                else
                {
                    area = area.Encompass(widget.position);
                }
            }

            return area;
        }

        public static void DrawDragAndDropPreviewLabel(Vector2 position, GUIContent content)
        {
            var padding = Styles.dragAndDropPreviewBackground.padding;

            var textSize = Styles.dragAndDropPreviewText.CalcSize(content);

            var backgroundPosition = new Rect
                (
                position.x,
                position.y,
                textSize.x + padding.left + padding.right,
                textSize.y + padding.top + padding.bottom
                );

            var textPosition = new Rect
                (
                backgroundPosition.x + padding.left,
                backgroundPosition.y + padding.top,
                textSize.x,
                textSize.y
                );

            Rect iconPosition = default(Rect);

            if (content.image != null)
            {
                iconPosition = new Rect
                    (
                    backgroundPosition.x + padding.left,
                    backgroundPosition.y + padding.top,
                    IconSize.Small,
                    IconSize.Small
                    );

                var spacing = 5;

                textPosition.x += iconPosition.width + spacing;
                textPosition.y += 1;
                textPosition.width += iconPosition.width + spacing;
                backgroundPosition.width += iconPosition.width + spacing;
            }

            GUI.Label(backgroundPosition, GUIContent.none, Styles.dragAndDropPreviewBackground);

            GUI.Label(textPosition, content, Styles.dragAndDropPreviewText);

            if (content.image != null)
            {
                GUI.DrawTexture(iconPosition, content.image);
            }
        }

        public static void DrawDragAndDropPreviewLabel(Vector2 position, string content)
        {
            DrawDragAndDropPreviewLabel(position, new GUIContent(content));
        }

        public static void DrawDragAndDropPreviewLabel(Vector2 position, string content, EditorTexture icon)
        {
            DrawDragAndDropPreviewLabel(position, new GUIContent(content, icon?[IconSize.Small]));
        }

        private static Texture2D AliasedBezierTexture(float width)
        {
            if (!bezierTextures.ContainsKey(width))
            {
                var height = Mathf.Max(2, Mathf.CeilToInt(width / 2));

                var texture = new Texture2D(1, height, TextureFormat.ARGB32, false, LudiqGUIUtility.createLinearTextures);

                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(0, y, Color.white.WithAlpha(y == 0 ? 0 : 1));
                }

                texture.Apply();

                bezierTextures.Add(width, texture);
            }

            return bezierTextures[width];
        }

        private static readonly Dictionary<float, Texture2D> bezierTextures = new Dictionary<float, Texture2D>();

        public static void UpdateDroplets(ICanvas canvas, List<float> droplets, int lastEntryFrame, ref float lastEntryTime, ref float dropTime, float discreteThreshold = 0.1f, float continuousDelay = 0.33f, float trickleDuration = 0.5f)
        {
            if (EditorApplication.isPaused)
            {
                return;
            }

            var time = EditorTimeBinding.time;
            var frame = EditorTimeBinding.frame;
            var deltaTime = canvas.eventDeltaTime;

            // Create new droplets

            if (lastEntryFrame == frame)
            {
                if (time - lastEntryTime > discreteThreshold)
                {
                    droplets.Add(0);
                    dropTime = time;
                }
                else if (time > dropTime + continuousDelay)
                {
                    droplets.Add(0);
                    dropTime = time;
                }

                lastEntryTime = time;
            }

            // Move droplets along the path

            for (int i = 0; i < droplets.Count; i++)
            {
                droplets[i] += deltaTime * (1 / trickleDuration);

                if (droplets[i] > 1)
                {
                    droplets.RemoveAt(i);
                }
            }
        }

        public static class Styles
        {
            static Styles()
            {
                LoadStyles();
            }

            internal static void LoadStyles()
            {
                coordinatesLabel = new GUIStyle(EditorStyles.label);
                coordinatesLabel.normal.textColor = majorGridColor;
                coordinatesLabel.fontSize = 9;
                coordinatesLabel.normal.background = new Color(0.36f, 0.36f, 0.36f).GetPixel();
                coordinatesLabel.padding = new RectOffset(4, 4, 4, 4);

                var nodeColorComparer = new NodeColorComparer();
                squares = new Dictionary<NodeColor, GUIStyle>(nodeColorComparer);
                hexes = new Dictionary<NodeColor, GUIStyle>(nodeColorComparer);

                foreach (var nodeColor in nodeColors)
                {
                    var squareOff = (GUIStyle)($"flow node {(int)nodeColor}");
                    var squareOn = (GUIStyle)($"flow node {(int)nodeColor} on");
                    var hexOff = (GUIStyle)($"flow node hex {(int)nodeColor}");
                    var hexOn = (GUIStyle)($"flow node hex {(int)nodeColor} on");

                    // For node styles:
                    //  - Border: 9-slice coordinates
                    //  - Padding: inner spacing from edge
                    //  - Margin: shadow / glow outside edge
                    TextureResolution[] textureResolution = { 2 };
                    var createTextureOptions = CreateTextureOptions.Scalable;

                    string path = "NodeFill";

                    EditorTexture normalTexture = BoltCore.Resources.LoadTexture($"{path}{nodeColor}.png", textureResolution, createTextureOptions);
                    EditorTexture activeTexture = BoltCore.Resources.LoadTexture($"{path}{nodeColor}Active.png", textureResolution, createTextureOptions);
                    EditorTexture hoverTexture = BoltCore.Resources.LoadTexture($"{path}{nodeColor}Hover.png", textureResolution, createTextureOptions);
                    EditorTexture focusedTexture = BoltCore.Resources.LoadTexture($"{path}{nodeColor}Focused.png", textureResolution, createTextureOptions);

                    var square = new GUIStyle
                    {
                        border = squareOff.border.Clone(),
                        margin = new RectOffset(3, 3, 6, 9),
                        padding = new RectOffset(5, 5, 6, 6),

                        normal = { background = normalTexture.Single() },
                        active = { background = activeTexture.Single() },
                        hover = { background = hoverTexture.Single() },
                        focused = { background = focusedTexture.Single() }
                    };
                    squares.Add(nodeColor, square);

                    var hex = new GUIStyle
                    {
                        border = new RectOffset(25, 25, 23, 23),
                        margin = new RectOffset(6, 6, 5, 7),
                        padding = new RectOffset(17, 17, 10, 10),
                        normal = { background = hexOff.normal.background },
                        active = { background = hexOff.normal.background },
                        hover = { background = hexOff.normal.background },
                        focused = { background = hexOn.normal.background }
                    };
                    hexes.Add(nodeColor, hex);
                }

                var arrowResolutions = new TextureResolution[] { 32 };
                var arrowOptions = CreateTextureOptions.Scalable;

                arrowUp = BoltCore.Resources.LoadTexture("ArrowUp_Pro.png", arrowResolutions, arrowOptions, false);
                arrowDown = BoltCore.Resources.LoadTexture("ArrowDown_Pro.png", arrowResolutions, arrowOptions, false);
                arrowLeft = BoltCore.Resources.LoadTexture("ArrowLeft_Pro.png", arrowResolutions, arrowOptions, false);
                arrowRight = BoltCore.Resources.LoadTexture("ArrowRight_Pro.png", arrowResolutions, arrowOptions, false);

                lockIcon = new GUIContent(LudiqGUIUtility.newSkin ? ((GUIStyle)"IN ThumbnailSelection").onActive.background : ((GUIStyle)"Icon.Locked").onNormal.background);

                if (EditorGUIUtility.isProSkin)
                {
                    majorGridColor = new Color(0, 0, 0, majorGridColor.a * 1.5f);
                    minorGridColor = new Color(0, 0, 0, minorGridColor.a * 1.5f);
                }

                dragAndDropPreviewBackground = new GUIStyle("TE NodeBox");
                dragAndDropPreviewBackground.margin = new RectOffset(0, 0, 0, 0);
                dragAndDropPreviewBackground.padding = new RectOffset(6, 8, 4, 8);

                dragAndDropPreviewText = new GUIStyle();
                dragAndDropPreviewText.fontSize = 11;
                dragAndDropPreviewText.normal.textColor = ColorPalette.unityForeground;
                dragAndDropPreviewText.imagePosition = ImagePosition.TextOnly;
            }

            public static readonly GUIStyle background = new GUIStyle("flow background");
            public static Color majorGridColor = new Color(1, 1, 1, 0.1f);
            public static Color minorGridColor = new Color(1, 1, 1, 0.03f);
            public static readonly int majorGridGroup = 10;
            public static readonly float minorGridSpacing = 12;
            public static readonly float majorGridThickness = 1;
            public static readonly float minorGridThickness = 1;
            public static Dictionary<NodeColor, GUIStyle> squares;
            public static Dictionary<NodeColor, GUIStyle> hexes;
            public static GUIStyle coordinatesLabel;
            public static GUIStyle dragAndDropPreviewBackground;
            public static GUIStyle dragAndDropPreviewText;

            public static EditorTexture arrowUp;
            public static EditorTexture arrowRight;
            public static EditorTexture arrowDown;
            public static EditorTexture arrowLeft;

            public static readonly float dimAlpha = EditorGUIUtility.isProSkin ? 0.3f : 0.4f;

            public static GUIContent lockIcon;

            // Mono allocates memory on its default comparer for enums
            // because of boxing. Creating a specific comparer to avoid this.
            // http://stackoverflow.com/a/26281533
            public struct NodeColorComparer : IEqualityComparer<NodeColor>
            {
                public bool Equals(NodeColor x, NodeColor y)
                {
                    return x == y;
                }

                public int GetHashCode(NodeColor obj)
                {
                    return (int)obj;
                }
            }
        }

        public static readonly NodeColor[] nodeColors = (NodeColor[])Enum.GetValues(typeof(NodeColor));
    }
}
