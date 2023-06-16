namespace Unity.VisualScripting
{
    public sealed class NullOption : FuzzyOption<object>
    {
        public NullOption()
        {
            label = "(None)";
            value = null;
        }
    }
}
