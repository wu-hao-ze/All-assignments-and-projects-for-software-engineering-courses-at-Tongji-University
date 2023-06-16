using System;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(Enum))]
    public class EnumOption : DocumentedOption<Enum>
    {
        public EnumOption(Enum @enum)
        {
            value = @enum;
            label = @enum.HumanName();
            UnityAPI.Async(() => icon = @enum.Icon());
            documentation = @enum.Documentation();
            zoom = true;
        }
    }
}
