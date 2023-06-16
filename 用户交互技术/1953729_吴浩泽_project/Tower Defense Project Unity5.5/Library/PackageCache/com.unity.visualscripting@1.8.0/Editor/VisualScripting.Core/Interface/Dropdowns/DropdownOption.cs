using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Unity.VisualScripting
{
    public class DropdownOption : IDropdownOption
    {
        public DropdownOption(object value)
        {
            this.value = value;
            label = value != null ? value.ToString() : "(null)";
        }

        public DropdownOption(object value, string label)
        {
            this.value = value;
            this.label = label;
        }

        public string label { get; set; }

        public virtual string popupLabel => label;

        public object value { get; set; }

        private static IEnumerable<DropdownOption> GetEnumOptions<T>(bool nicify)
        {
            foreach (var enumValue in Enum.GetValues(typeof(T)).Cast<T>())
            {
                yield return new DropdownOption(enumValue, nicify ? ObjectNames.NicifyVariableName(enumValue.ToString()) : enumValue.ToString());
            }
        }

        public static IEnumerable<DropdownOption> GetEnumOptions<T>()
        {
            return GetEnumOptions<T>(false);
        }

        public static IEnumerable<DropdownOption> GetEnumOptionsNicified<T>()
        {
            return GetEnumOptions<T>(true);
        }
    }
}
