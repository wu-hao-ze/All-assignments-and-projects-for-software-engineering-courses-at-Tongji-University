using UnityEngine;

namespace Unity.VisualScripting
{
    public class ListOption
    {
        public ListOption(object value)
        {
            this.value = value;
            label = new GUIContent(value?.ToString() ?? "(null)");
        }

        public ListOption(object value, GUIContent label)
        {
            this.value = value;
            this.label = label;
        }

        public ListOption(object value, string label) : this(value, new GUIContent(label)) { }

        public object value { get; set; }
        public GUIContent label { get; set; }
    }
}
