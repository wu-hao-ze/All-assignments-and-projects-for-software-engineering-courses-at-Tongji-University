using System.Diagnostics;
using System.Threading;
using UnityEngine.Profiling;

namespace Unity.VisualScripting
{
    public static class ProfilingUtility
    {
        static ProfilingUtility()
        {
            currentSegment = rootSegment = new ProfiledSegment(null, "Root");
        }

        private static readonly object @lock = new object();

        public static ProfiledSegment rootSegment { get; private set; }
        public static ProfiledSegment currentSegment { get; set; }

        [Conditional("ENABLE_PROFILER")]
        public static void Clear()
        {
            currentSegment = rootSegment = new ProfiledSegment(null, "Root");
        }

        public static ProfilingScope SampleBlock(string name)
        {
            return new ProfilingScope(name);
        }

        [Conditional("ENABLE_PROFILER")]
        public static void BeginSample(string name)
        {
            Monitor.Enter(@lock);

            if (!currentSegment.children.Contains(name))
            {
                currentSegment.children.Add(new ProfiledSegment(currentSegment, name));
            }

            currentSegment = currentSegment.children[name];
            currentSegment.calls++;
            currentSegment.stopwatch.Start();

            if (UnityThread.allowsAPI)
            {
                Profiler.BeginSample(name);
            }
        }

        [Conditional("ENABLE_PROFILER")]
        public static void EndSample()
        {
            currentSegment.stopwatch.Stop();

            if (currentSegment.parent != null)
            {
                currentSegment = currentSegment.parent;
            }

            if (UnityThread.allowsAPI)
            {
                Profiler.EndSample();
            }

            Monitor.Exit(@lock);
        }
    }
}
