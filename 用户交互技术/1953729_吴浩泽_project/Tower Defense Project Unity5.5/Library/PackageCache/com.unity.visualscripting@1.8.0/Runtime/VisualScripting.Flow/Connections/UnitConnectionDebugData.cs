using System;

namespace Unity.VisualScripting
{
    public class UnitConnectionDebugData : IUnitConnectionDebugData
    {
        public int lastInvokeFrame { get; set; }

        public float lastInvokeTime { get; set; }

        public Exception runtimeException { get; set; }
    }
}
