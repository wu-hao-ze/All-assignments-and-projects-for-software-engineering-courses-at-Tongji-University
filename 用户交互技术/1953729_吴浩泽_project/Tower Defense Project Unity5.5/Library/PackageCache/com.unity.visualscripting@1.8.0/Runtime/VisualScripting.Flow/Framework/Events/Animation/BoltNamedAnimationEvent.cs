using System.ComponentModel;
using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_ANIMATION_EXISTS
    /// <summary>
    /// Called when an animation event points to TriggerAnimationEvent.
    /// This version allows you to use the string parameter as the event name.
    /// </summary>
    [UnitCategory("Events/Animation")]
    [UnitShortTitle("Animation Event")]
    [UnitTitle("Named Animation Event")]
    [TypeIcon(typeof(Animation))]
    [DisplayName("Visual Scripting Named Animation Event")]
    public sealed class BoltNamedAnimationEvent : MachineEventUnit<AnimationEvent>
    {
        protected override string hookName => EventHooks.AnimationEvent;

        /// <summary>
        /// The name of the event. The event will only trigger if this value
        /// is equal to the string parameter passed in the animation event.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput name { get; private set; }

        /// <summary>
        /// The float parameter passed to the event.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Float")]
        public ValueOutput floatParameter { get; private set; }

        /// <summary>
        /// The integer parameter passed to the function.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Integer")]
        public ValueOutput intParameter { get; private set; }

        /// <summary>
        /// The Unity object parameter passed to the function.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Object")]
        public ValueOutput objectReferenceParameter { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            name = ValueInput(nameof(name), string.Empty);

            floatParameter = ValueOutput<float>(nameof(floatParameter));
            intParameter = ValueOutput<int>(nameof(intParameter));
            objectReferenceParameter = ValueOutput<GameObject>(nameof(objectReferenceParameter));
        }

        protected override bool ShouldTrigger(Flow flow, AnimationEvent animationEvent)
        {
            return CompareNames(flow, name, animationEvent.stringParameter);
        }

        protected override void AssignArguments(Flow flow, AnimationEvent animationEvent)
        {
            flow.SetValue(floatParameter, animationEvent.floatParameter);
            flow.SetValue(intParameter, animationEvent.intParameter);
            flow.SetValue(objectReferenceParameter, animationEvent.objectReferenceParameter);
        }
    }
#endif
}
