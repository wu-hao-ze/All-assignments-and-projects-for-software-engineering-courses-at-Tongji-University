namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(FuzzyGroup))]
    public class FuzzyGroupOption : FuzzyOption<object>
    {
        public FuzzyGroupOption(FuzzyGroup group)
        {
            value = group;
            label = group.label;
            icon = group.icon;
            parentOnly = true;
        }
    }
}
