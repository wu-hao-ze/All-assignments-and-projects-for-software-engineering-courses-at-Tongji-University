using System;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [Obsolete("Use BackgroundWorker.Schedule() directly instead of this attribute")]
    public class BackgroundWorkerAttribute : Attribute
    {
        public BackgroundWorkerAttribute(string methodName = "BackgroundWork")
        {
            this.methodName = methodName;
        }

        public string methodName { get; }
    }
}
