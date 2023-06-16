using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Selects a value from a set by switching over a string.
    /// </summary>
    [UnitCategory("Control")]
    [UnitTitle("Select On String")]
    [UnitShortTitle("Select")]
    [UnitSubtitle("On String")]
    [UnitOrder(7)]
    public class SelectOnString : SelectUnit<string>
    {
        [Serialize]
        [Inspectable, UnitHeaderInspectable("Ignore Case")]
        [InspectorToggleLeft]
        public bool ignoreCase { get; set; }

        protected override bool Matches(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return true;
            }

            return string.Equals(a, b, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
    }
}
