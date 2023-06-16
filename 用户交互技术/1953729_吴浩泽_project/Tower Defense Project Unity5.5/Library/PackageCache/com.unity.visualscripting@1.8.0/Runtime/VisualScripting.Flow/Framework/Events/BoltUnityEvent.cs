using System.ComponentModel;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when a UnityEvent points to TriggerUnityEvent.
    /// </summary>
    [UnitCategory("Events")]
    [UnitTitle("UnityEvent")]
    [UnitOrder(2)]
    [DisplayName("Visual Scripting Unity Event")]
    public sealed class BoltUnityEvent : MachineEventUnit<string>
    {
        protected override string hookName => EventHooks.UnityEvent;

        /// <summary>
        /// The name of the event. The event will only trigger if this value
        /// is equal to the string parameter passed in the UnityEvent.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput name { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            name = ValueInput(nameof(name), string.Empty);
        }

        protected override bool ShouldTrigger(Flow flow, string name)
        {
            return CompareNames(flow, this.name, name);
        }
    }
}
