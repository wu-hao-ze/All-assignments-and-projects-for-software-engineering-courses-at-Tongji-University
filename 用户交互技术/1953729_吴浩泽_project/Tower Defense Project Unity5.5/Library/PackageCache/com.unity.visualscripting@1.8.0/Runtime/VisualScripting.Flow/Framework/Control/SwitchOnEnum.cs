using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Branches flow by switching over an enum.
    /// </summary>
    [UnitCategory("Control")]
    [UnitTitle("Switch On Enum")]
    [UnitShortTitle("Switch")]
    [UnitSubtitle("On Enum")]
    [UnitOrder(3)]
    [TypeIcon(typeof(IBranchUnit))]
    public sealed class SwitchOnEnum : Unit, IBranchUnit
    {
        [DoNotSerialize]
        public Dictionary<Enum, ControlOutput> branches { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable]
        [TypeFilter(Enums = true, Classes = false, Interfaces = false, Structs = false, Primitives = false)]
        public Type enumType { get; set; }

        /// <summary>
        /// The entry point for the switch.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The enum value on which to switch.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput @enum { get; private set; }

        public override bool canDefine => enumType != null && enumType.IsEnum;

        protected override void Definition()
        {
            branches = new Dictionary<Enum, ControlOutput>();

            enter = ControlInput(nameof(enter), Enter);

            @enum = ValueInput(enumType, nameof(@enum));

            Requirement(@enum, enter);

            foreach (var valueByName in EnumUtility.ValuesByNames(enumType))
            {
                var enumName = valueByName.Key;
                var enumValue = valueByName.Value;

                // Just like in C#, duplicate switch labels for the same underlying value is prohibited
                if (!branches.ContainsKey(enumValue))
                {
                    var branch = ControlOutput("%" + enumName);

                    branches.Add(enumValue, branch);

                    Succession(enter, branch);
                }
            }
        }

        public ControlOutput Enter(Flow flow)
        {
            var @enum = (Enum)flow.GetValue(this.@enum, enumType);

            if (branches.ContainsKey(@enum))
            {
                return branches[@enum];
            }
            else
            {
                return null;
            }
        }
    }
}
