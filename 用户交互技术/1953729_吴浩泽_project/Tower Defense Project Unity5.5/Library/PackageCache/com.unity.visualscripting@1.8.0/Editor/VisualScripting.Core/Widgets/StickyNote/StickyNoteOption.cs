namespace Unity.VisualScripting
{
    public sealed class StickyNoteOption : FuzzyOption<object>
    {
        public StickyNoteOption()
        {
            label = "Sticky Note";
            value = typeof(StickyNote);
            UnityAPI.Async(() => icon = typeof(StickyNote).Icon());
        }
    }
}
