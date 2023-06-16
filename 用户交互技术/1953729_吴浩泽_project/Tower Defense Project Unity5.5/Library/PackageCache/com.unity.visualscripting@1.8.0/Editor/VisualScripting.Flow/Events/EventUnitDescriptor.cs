using System.Collections.Generic;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(IEventUnit))]
    public class EventUnitDescriptor<TEvent> : UnitDescriptor<TEvent>
        where TEvent : class, IEventUnit
    {
        public EventUnitDescriptor(TEvent @event) : base(@event) { }

        protected override string DefinedSubtitle()
        {
            return "Event";
        }

        protected override IEnumerable<EditorTexture> DefinedIcons()
        {
            if (unit.coroutine)
            {
                yield return BoltFlow.Icons.coroutine;
            }
        }
    }
}
