using System;

namespace Unity.VisualScripting
{
    public struct ProfilingScope : IDisposable
    {
        public ProfilingScope(string name)
        {
            ProfilingUtility.BeginSample(name);
        }

        public void Dispose()
        {
            ProfilingUtility.EndSample();
        }
    }
}
