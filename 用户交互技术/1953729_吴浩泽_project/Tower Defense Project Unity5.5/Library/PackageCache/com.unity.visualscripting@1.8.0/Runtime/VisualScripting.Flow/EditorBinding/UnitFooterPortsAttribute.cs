using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class UnitFooterPortsAttribute : Attribute
    {
        public bool ControlInputs { get; set; } = false;
        public bool ControlOutputs { get; set; } = false;
        public bool ValueInputs { get; set; } = true;
        public bool ValueOutputs { get; set; } = true;
    }
}
