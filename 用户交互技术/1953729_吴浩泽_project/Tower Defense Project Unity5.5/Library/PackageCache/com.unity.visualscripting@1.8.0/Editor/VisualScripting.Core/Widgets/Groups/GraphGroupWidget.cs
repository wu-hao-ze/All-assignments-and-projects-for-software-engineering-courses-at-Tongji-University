using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(GraphGroup))]
    public class GraphGroupWidget : GraphElementWidget<ICanvas, GraphGroup>
    {
        public GraphGroupWidget(ICanvas canvas, GraphGroup group) : base(canvas, group)
        {
            minResizeSize = new Vector2(64, 64);
            resizeInnerOffset.top = 0;
        }

        #region Model

        private GraphGroup group => element;

        private IEnumerable<IGraphElement> elements
        {
            get
            {
                foreach (var element in graph.elements.OfType<IGraphElement>())
                {
                    if (position.Encompasses(canvas.Widget(element).position))
                    {
                        yield return element;
                    }
                }
            }
        }

        public override void CacheItem()
        {
            base.CacheItem();

            labelContent.text = group.label;
        }

        #endregion


        #region Lifecycle

        public override bool foregroundRequiresInput => true;

        #endregion


        #region Contents

        private static readonly GUIContent maxHeadLabelSizeContent = new GUIContent("M");

        private readonly GUIContent labelContent = new GUIContent();

        #endregion


        #region Positioning

        protected override bool snapToGrid => BoltCore.Configuration.snapToGrid;

        public override float zIndex
        {
            get
            {
                return float.MinValue;
            }
            set { }
        }

        public override Rect position
        {
            get
            {
                return group.position;
            }
            set
            {
                group.position = value;
            }
        }

        public Rect labelPosition { get; private set; }

        public override Rect hotArea => headerPosition;

        public override void CachePosition()
        {
            AdjustLabelFontSize();

            headerPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                Styles.headerHeight
                );

            labelPosition = new Rect
                (
                position.x + Styles.label.margin.left,
                position.y + Styles.label.margin.top,
                Styles.label.CalcSize(labelContent).x + Styles.label.CalcSize(maxHeadLabelSizeContent).x,
                Styles.group.border.top
                );
        }

        public override void OnViewportChange()
        {
            base.OnViewportChange();

            Reposition(); // The label position is dependant on the zoom factor
        }

        private void AdjustLabelFontSize()
        {
            Styles.label.fontSize = Styles.labelSelected.fontSize = Mathf.RoundToInt(Styles.headerFontSize / graph.zoom);
        }

        #endregion


        #region Drawing


        public override void DrawForeground()
        {
            AdjustLabelFontSize();
            window.AddCursorRect(labelPosition, MouseCursor.Text);


            if (isMouseOverHeaderArea)
            {
                window.AddCursorRect(hotArea, MouseCursor.MoveArrow);
            }

            //GUI.SetNextControlName(group.guid + ".LabelField");
            //var labelFieldId = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
            group.label = EditorGUI.TextField(labelPosition, GUIContent.none, group.label, selection.Contains(element) ? Styles.labelSelected : Styles.label);

            if (shouldFocusLabel)
            {
                // I give up, nothing seems to work consistently.
                // If future me wants to give this yet another try, don't forget to
                // avoid reproducing this issue:
                // https://support.ludiq.io/communities/5/topics/2530-group-box-rename-bug
                //GUIUtility.hotControl = labelFieldId;
                //EditorGUIUtility.editingTextField = true;
                //Debug.Log("Focusing on " + group.guid + ".LabelField");
                //GUIUtility.keyboardControl = 0;
                //GUI.FocusControl(group.guid + ".LabelField");
                shouldFocusLabel = false;
            }
        }

        public override void DrawBackground()
        {
            var selected = selection.Contains(group);

            using (LudiqGUI.color.Override(Styles.AdjustColor(group.color, selected)))
            {
                Styles.group.Draw(position, false, false, selected, false);
            }
        }

        #endregion


        #region Deleting

        public override bool canDelete => true;

        #endregion


        #region Layouting

        public override bool canAlignAndDistribute => false;

        #endregion


        #region Resizing

        public override bool canResizeHorizontal => true;

        public override bool canResizeVertical => true;

        #endregion


        #region Selecting

        private bool shouldFocusLabel;

        public override bool canSelect => true;

        public void FocusLabel()
        {
            shouldFocusLabel = true;
        }

        private void SelectElements()
        {
            selection.Select(elements.Where(element => canvas.Widget(element).canSelect));
        }

        protected override void OnDoubleClick()
        {
            if (group.graph.zoom == 1)
            {
                SelectElements();
                e.Use();
            }
            else
            {
                base.OnDoubleClick();
            }
        }

        #endregion


        #region Dragging

        public override bool canDrag => true;

        public override void ExpandDragGroup(HashSet<IGraphElement> dragGroup)
        {
            if ((BoltCore.Configuration.controlScheme == CanvasControlScheme.Default && e.alt) ||
                (BoltCore.Configuration.controlScheme == CanvasControlScheme.Alternate && e.ctrlOrCmd))
            {
                return;
            }

            if (!elements.Any(e => selection.Contains(e)))
            {
                dragGroup.UnionWith(elements);
            }
        }

        #endregion


        public static class Styles
        {
            static Styles()
            {
                @group = new GUIStyle();
                @group.normal.background = BoltCore.Resources.LoadTexture("Group.png", new TextureResolution[] { 64 }, CreateTextureOptions.PixelPerfect)?.Single();
                @group.onNormal.background = @group.normal.background;
                @group.border = new RectOffset(16, 16, 25, 16);

                label = new GUIStyle();
                label.normal.textColor = new Color(1, 1, 1, 0.75f);
                label.alignment = TextAnchor.MiddleLeft;
                label.padding = new RectOffset(0, 5, 0, 0);
                label.margin = new RectOffset(10, 0, 0, 0);

                labelSelected = new GUIStyle(label);
                //labelSelected.normal.textColor = new Color(0, 0, 0, 0.75f);
            }

            public static readonly float headerFontSize = 14;

            public static readonly GUIStyle group;

            public static readonly GUIStyle label;

            public static readonly GUIStyle labelSelected;

            public static float headerHeight => @group.border.top;

            public static Color AdjustColor(Color color, bool selected)
            {
                float hue, saturation, value;
                var alpha = color.a;
                Color.RGBToHSV(color, out hue, out saturation, out value);

                var saturationBoost = 0.25f;
                var valueBoost = 0.25f;
                var alphaAttenuation = -0.4f;
                var minAlpha = 0.25f;

                if (selected)
                {
                    if (saturation == 0)
                    {
                        hue = 0.59f; // Tealish, same as selection rectangle
                    }

                    if (value == 0)
                    {
                        value = 1;
                    }

                    saturation = Mathf.Clamp(saturation + saturationBoost, 0.6f, 1);
                    value = Mathf.Clamp(value + valueBoost, 0.6f, 1);
                    alpha = Mathf.Clamp(alpha, minAlpha - alphaAttenuation, 1);
                }
                else
                {
                    if (saturation > 1 - saturationBoost &&
                        value > 1 - valueBoost)
                    {
                        alpha += alphaAttenuation;
                    }
                }

                alpha = Mathf.Clamp(alpha, minAlpha, 1);

                return Color.HSVToRGB(hue, saturation, value).WithAlpha(alpha);
            }
        }
    }
}
