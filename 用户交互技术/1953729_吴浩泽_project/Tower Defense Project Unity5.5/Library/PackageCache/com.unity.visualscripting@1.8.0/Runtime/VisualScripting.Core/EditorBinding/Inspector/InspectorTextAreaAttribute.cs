using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InspectorTextAreaAttribute : Attribute
    {
        private float? _minLines;
        private float? _maxLines;

        public float minLines
        {
            get => _minLines.GetValueOrDefault();
            set => _minLines = value;
        }

        public bool hasMinLines => _minLines.HasValue;

        public float maxLines
        {
            get => _maxLines.GetValueOrDefault();
            set => _maxLines = value;
        }

        public bool hasMaxLines => _maxLines.HasValue;
    }
}
