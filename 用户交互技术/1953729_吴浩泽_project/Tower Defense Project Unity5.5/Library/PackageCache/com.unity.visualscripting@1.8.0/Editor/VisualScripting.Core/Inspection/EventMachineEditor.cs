namespace Unity.VisualScripting
{
    [Editor(typeof(IEventMachine))]
    public class EventMachineEditor : MachineEditor
    {
        public EventMachineEditor(Metadata metadata) : base(metadata) { }
    }
}
