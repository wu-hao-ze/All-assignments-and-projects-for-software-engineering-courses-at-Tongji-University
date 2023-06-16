using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(INesterUnit))]
    public class NesterUnitDescriptor<TNesterUnit> : UnitDescriptor<TNesterUnit> where TNesterUnit : class, INesterUnit
    {
        public NesterUnitDescriptor(TNesterUnit unit) : base(unit) { }

        [RequiresUnityAPI]
        protected override string DefinedTitle()
        {
            return GraphNesterDescriptor.Title(unit, BoltFlowNameUtility.UnitTitle(unitType, false, false));
        }

        [RequiresUnityAPI]
        protected override string DefinedSummary()
        {
            return GraphNesterDescriptor.Summary(unit);
        }

        [RequiresUnityAPI]
        protected override string DefinedShortTitle()
        {
            return DefinedTitle();
        }

        [RequiresUnityAPI]
        protected override string DefinedSurtitle()
        {
            var hasCurrentTitle = !StringUtility.IsNullOrWhiteSpace(unit.nest.graph?.title);
            var hasMacroTitle = unit.nest.source == GraphSource.Macro && (UnityObject)unit.nest.macro != null;

            return hasCurrentTitle || hasMacroTitle
                ? BoltFlowNameUtility.UnitTitle(unitType, false, false)
                : null;
        }
    }
}
