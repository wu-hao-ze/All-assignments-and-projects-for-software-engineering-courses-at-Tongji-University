using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    public abstract class GenericGuiEventUnit : GameObjectEventUnit<BaseEventData>
    {
        /// <summary>
        /// The event data.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput data { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            data = ValueOutput<BaseEventData>(nameof(data));
        }

        protected override void AssignArguments(Flow flow, BaseEventData data)
        {
            flow.SetValue(this.data, data);
        }
    }
}
