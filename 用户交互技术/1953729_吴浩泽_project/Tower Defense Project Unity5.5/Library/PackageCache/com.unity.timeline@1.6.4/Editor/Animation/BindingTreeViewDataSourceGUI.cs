using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    class BindingTreeViewGUI : TreeViewGUI
    {
        const float k_RowRightOffset = 10;
        const float k_CurveColorIndicatorIconSize = 11;
        const float k_ColorIndicatorTopMargin = 3;

        static readonly Color s_KeyColorForNonCurves = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        static readonly Color s_ChildrenCurveLabelColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        static readonly Color s_PhantomPropertyLabelColor = new Color(0.0f, 0.8f, 0.8f, 1f);
        static readonly Texture2D s_DefaultScriptTexture = EditorGUIUtility.LoadIcon("cs Script Icon");
        static readonly Texture2D s_TrackDefault = EditorGUIUtility.LoadIcon("UnityEngine/ScriptableObject Icon");
        public float parentWidth { get; set; }

        public BindingTreeViewGUI(TreeViewController treeView)
            : base(treeView, true)
        {
            k_IconWidth = 13.0f;
            iconOverlayGUI += OnItemIconOverlay;
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            Color originalColor = GUI.color;
            bool leafNode = node.parent != null && node.parent.id != BindingTreeViewDataSource.RootID && node.parent.id != BindingTreeViewDataSource.GroupID;
            GUI.color = Color.white;
            if (leafNode)
            {
                CurveTreeViewNode curveNode = node as CurveTreeViewNode;
                if (curveNode != null && curveNode.bindings.Any() && curveNode.bindings.First().isPhantom)
                    GUI.color = s_PhantomPropertyLabelColor;
                else
                    GUI.color = s_ChildrenCurveLabelColor;
            }

            base.OnRowGUI(rowRect, node, row, selected, focused);

            GUI.color = originalColor;
            DoCurveColorIndicator(rowRect, node as CurveTreeViewNode);
        }

        protected override bool IsRenaming(int id)
        {
            return false;
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            return false;
        }

        static void DoCurveColorIndicator(Rect rect, CurveTreeViewNode node)
        {
            if (node == null)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            Color originalColor = GUI.color;

            if (node.bindings.Length == 1 && !node.bindings[0].isPPtrCurve)
                GUI.color = CurveUtility.GetPropertyColor(node.bindings[0].propertyName);
            else
                GUI.color = s_KeyColorForNonCurves;

            Texture icon = CurveUtility.GetIconCurve();

            rect = new Rect(rect.xMax - k_RowRightOffset - (k_CurveColorIndicatorIconSize / 2) - 5,
                rect.yMin + k_ColorIndicatorTopMargin + (rect.height - EditorGUIUtility.singleLineHeight) / 2,
                k_CurveColorIndicatorIconSize,
                k_CurveColorIndicatorIconSize);

            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true, 1);

            GUI.color = originalColor;
        }

        protected override Texture GetIconForItem(TreeViewItem item)
        {
            var node = item as CurveTreeViewNode;
            if (node == null)
                return null;

            var type = node.iconType;
            if (type == null)
                return null;

            // track type icon
            if (typeof(TrackAsset).IsAssignableFrom(type))
            {
                var icon = TrackResourceCache.GetTrackIconForType(type);
                return icon == s_TrackDefault ? s_DefaultScriptTexture : icon;
            }

            // custom clip icons always use the script texture
            if (typeof(PlayableAsset).IsAssignableFrom(type))
                return s_DefaultScriptTexture;

            // this will return null for MonoBehaviours without a custom icon.
            // use the scripting icon instead
            return AssetPreview.GetMiniTypeThumbnail(type) ?? s_DefaultScriptTexture;
        }

        static void OnItemIconOverlay(TreeViewItem item, Rect rect)
        {
            var curveNodeItem = item as CurveTreeViewNode;
            if (curveNodeItem != null && curveNodeItem.iconOverlay != null)
                GUI.Label(rect, curveNodeItem.iconOverlay);
        }

        public override Vector2 GetTotalSize()
        {
            var originalSize = base.GetTotalSize();
            originalSize.x = Mathf.Max(parentWidth, originalSize.x);
            return originalSize;
        }
    }
}
