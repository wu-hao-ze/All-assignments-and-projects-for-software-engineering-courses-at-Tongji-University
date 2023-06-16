using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(INesterUnit))]
    public class NesterUnitOption<TNesterUnit> : UnitOption<TNesterUnit> where TNesterUnit : INesterUnit
    {
        string m_Name;

        public override string formerHaystack => string.IsNullOrEmpty(m_Name) ? base.formerHaystack : m_Name;

        public NesterUnitOption() : base() { }

        public NesterUnitOption(TNesterUnit unit)
            : base(unit)
        {
            // Store the name here as formerHaystack is called from a thread and UnitObject.Name can't be
            // called from a thread
            var macro = (UnityObject)unit.nest.macro;
            m_Name = macro != null ? macro.name : string.Empty;
        }

        // TODO: Favoritable
        public override bool favoritable => false;

        protected override string Label(bool human)
        {
            return UnityAPI.Await(() =>
            {
                var macro = (UnityObject)unit.nest.macro;
                return macro != null ? macro.name : BoltFlowNameUtility.UnitTitle(unit.GetType(), false, false);
            });
        }
    }
}
