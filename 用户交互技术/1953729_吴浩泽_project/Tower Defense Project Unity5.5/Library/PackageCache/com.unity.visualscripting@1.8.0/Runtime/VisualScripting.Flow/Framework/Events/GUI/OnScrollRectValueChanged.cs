using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the current value of the scrollbar has changed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(ScrollRect))]
    [UnitOrder(7)]
    public sealed class OnScrollRectValueChanged : GameObjectEventUnit<Vector2>
    {
        public override Type MessageListenerType => typeof(UnityOnScrollRectValueChangedMessageListener);
        protected override string hookName => EventHooks.OnScrollRectValueChanged;

        /// <summary>
        /// The new scroll position of the scroll rect.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput<Vector2>(nameof(value));
        }

        protected override void AssignArguments(Flow flow, Vector2 value)
        {
            flow.SetValue(this.value, value);
        }
    }
}
