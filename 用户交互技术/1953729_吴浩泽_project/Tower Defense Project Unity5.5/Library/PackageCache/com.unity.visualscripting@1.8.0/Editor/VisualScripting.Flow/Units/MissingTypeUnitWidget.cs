using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(MissingType))]
    public class MissingTypeUnitWidget : UnitWidget<MissingType>
    {
        public MissingTypeUnitWidget(FlowCanvas canvas, MissingType unit)
            : base(canvas, unit)
        { }

        protected override bool showSubtitle => !string.IsNullOrEmpty(unit.formerType);

        protected override void CacheDescription()
        {
            base.CacheDescription();

            if (showSubtitle)
            {
                titleContent.text = unit.formerType;
                subtitleContent.text = "Script Missing!";
            }
            else
            {
                titleContent.text = "Script Missing!";
            }
            titleContent.tooltip = $"No definition for type '{unit.formerType}' can be found. Did you perhaps remove a '{unit.formerType}.cs' script file?";
        }
    }
}
