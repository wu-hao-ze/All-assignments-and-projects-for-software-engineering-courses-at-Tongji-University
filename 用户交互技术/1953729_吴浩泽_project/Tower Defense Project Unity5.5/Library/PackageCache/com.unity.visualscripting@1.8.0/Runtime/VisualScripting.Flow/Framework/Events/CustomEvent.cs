using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// A special named event with any amount of parameters called manually with the 'Trigger Custom Event' unit.
    /// </summary>
    [UnitCategory("Events")]
    [UnitOrder(0)]
    public sealed class CustomEvent : GameObjectEventUnit<CustomEventArgs>
    {
        public override Type MessageListenerType => null;
        protected override string hookName => EventHooks.Custom;

        [SerializeAs(nameof(argumentCount))]
        private int _argumentCount;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Arguments")]
        public int argumentCount
        {
            get => _argumentCount;
            set => _argumentCount = Mathf.Clamp(value, 0, 10);
        }

        /// <summary>
        /// The name of the event.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput name { get; private set; }

        [DoNotSerialize]
        public List<ValueOutput> argumentPorts { get; } = new List<ValueOutput>();

        protected override void Definition()
        {
            base.Definition();

            name = ValueInput(nameof(name), string.Empty);

            argumentPorts.Clear();

            for (var i = 0; i < argumentCount; i++)
            {
                argumentPorts.Add(ValueOutput<object>("argument_" + i));
            }
        }

        protected override bool ShouldTrigger(Flow flow, CustomEventArgs args)
        {
            return CompareNames(flow, name, args.name);
        }

        protected override void AssignArguments(Flow flow, CustomEventArgs args)
        {
            for (var i = 0; i < argumentCount; i++)
            {
                flow.SetValue(argumentPorts[i], args.arguments[i]);
            }
        }

        public static void Trigger(GameObject target, string name, params object[] args)
        {
            EventBus.Trigger(EventHooks.Custom, target, new CustomEventArgs(name, args));
        }
    }
}
