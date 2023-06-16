using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Selects a value from a set by matching it with an input flow.
    /// </summary>
    [UnitCategory("Control")]
    [UnitTitle("Select On Flow")]
    [UnitShortTitle("Select")]
    [UnitSubtitle("On Flow")]
    [UnitOrder(8)]
    [TypeIcon(typeof(ISelectUnit))]
    public sealed class SelectOnFlow : Unit, ISelectUnit
    {
        [SerializeAs(nameof(branchCount))]
        private int _branchCount = 2;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Branches")]
        public int branchCount
        {
            get => _branchCount;
            set => _branchCount = Mathf.Clamp(value, 2, 10);
        }

        [DoNotSerialize]
        public Dictionary<ControlInput, ValueInput> branches { get; private set; }

        /// <summary>
        /// Triggered when any selector is entered.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        /// <summary>
        /// The selected value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput selection { get; private set; }

        protected override void Definition()
        {
            branches = new Dictionary<ControlInput, ValueInput>();

            selection = ValueOutput<object>(nameof(selection));
            exit = ControlOutput(nameof(exit));

            for (int i = 0; i < branchCount; i++)
            {
                var branchValue = ValueInput<object>("value_" + i);
                var branchControl = ControlInput("enter_" + i, (flow) => Select(flow, branchValue));

                Requirement(branchValue, branchControl);
                Assignment(branchControl, selection);
                Succession(branchControl, exit);

                branches.Add(branchControl, branchValue);
            }
        }

        public ControlOutput Select(Flow flow, ValueInput branchValue)
        {
            flow.SetValue(selection, flow.GetValue(branchValue));

            return exit;
        }
    }
}
