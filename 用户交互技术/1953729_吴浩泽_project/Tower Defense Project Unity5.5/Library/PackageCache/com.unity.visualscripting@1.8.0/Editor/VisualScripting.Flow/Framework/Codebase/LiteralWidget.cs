using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(Literal))]
    public sealed class LiteralWidget : UnitWidget<Literal>
    {
        public LiteralWidget(FlowCanvas canvas, Literal unit) : base(canvas, unit) { }

        protected override bool showHeaderAddon => unit.isDefined;

        public override bool foregroundRequiresInput => true;

        protected override float GetHeaderAddonWidth()
        {
            var adaptiveWidthAttribute = unit.type.GetAttribute<InspectorAdaptiveWidthAttribute>();

            return Mathf.Min(metadata.Inspector().GetAdaptiveWidth(), adaptiveWidthAttribute?.width ?? Styles.maxSettingsWidth);
        }

        protected override float GetHeaderAddonHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(null, metadata, width, GUIContent.none);
        }

        public override void BeforeFrame()
        {
            base.BeforeFrame();

            if (showHeaderAddon &&
                GetHeaderAddonWidth() != headerAddonPosition.width ||
                GetHeaderAddonHeight(headerAddonPosition.width) != headerAddonPosition.height)
            {
                Reposition();
            }
        }

        protected override void DrawHeaderAddon()
        {
            using (LudiqGUIUtility.labelWidth.Override(75)) // For reflected inspectors / custom property drawers
            using (Inspector.adaptiveWidth.Override(true))
            {
                EditorGUI.BeginChangeCheck();

                LudiqGUI.Inspector(metadata, headerAddonPosition, GUIContent.none);

                if (EditorGUI.EndChangeCheck())
                {
                    unit.EnsureDefined();
                    Reposition();
                }
            }
        }
    }
}
