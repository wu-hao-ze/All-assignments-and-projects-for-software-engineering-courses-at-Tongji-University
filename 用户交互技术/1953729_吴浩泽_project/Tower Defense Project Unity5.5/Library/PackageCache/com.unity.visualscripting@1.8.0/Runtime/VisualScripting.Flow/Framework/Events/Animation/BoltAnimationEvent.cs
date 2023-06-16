using System.ComponentModel;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
#if MODULE_ANIMATION_EXISTS 
    /// <summary>
    /// Called when an animation event points to TriggerAnimationEvent.
    /// </summary>
    [UnitCategory("Events/Animation")]
    [UnitShortTitle("Animation Event")]
    [UnitTitle("Animation Event")]
    [TypeIcon(typeof(Animation))]
    [DisplayName("Visual Scripting Animation Event")]
    public sealed class BoltAnimationEvent : MachineEventUnit<AnimationEvent>
    {
        protected override string hookName => EventHooks.AnimationEvent;

        /// <summary>
        /// The string parameter passed to the event.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("String")]
        public ValueOutput stringParameter { get; private set; }

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

            stringParameter = ValueOutput<string>(nameof(stringParameter));
            floatParameter = ValueOutput<float>(nameof(floatParameter));
            intParameter = ValueOutput<int>(nameof(intParameter));
            objectReferenceParameter = ValueOutput<UnityObject>(nameof(objectReferenceParameter));
        }

        protected override void AssignArguments(Flow flow, AnimationEvent args)
        {
            flow.SetValue(stringParameter, args.stringParameter);
            flow.SetValue(floatParameter, args.floatParameter);
            flow.SetValue(intParameter, args.intParameter);
            flow.SetValue(objectReferenceParameter, args.objectReferenceParameter);
        }
    }
#endif
}
