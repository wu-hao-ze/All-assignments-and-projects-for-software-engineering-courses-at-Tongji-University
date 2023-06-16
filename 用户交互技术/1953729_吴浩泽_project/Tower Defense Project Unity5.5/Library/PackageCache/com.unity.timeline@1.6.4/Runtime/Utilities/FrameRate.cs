#if !TIMELINE_FRAMEACCURATE
using System;

namespace UnityEngine.Timeline
{
    internal readonly struct FrameRate : IEquatable<FrameRate>
    {
        public readonly double rate;

        public static readonly FrameRate k_23_976Fps = new FrameRate(23.976023976024);
        public static readonly FrameRate k_24Fps = new FrameRate(24);
        public static readonly FrameRate k_25Fps = new FrameRate(25);
        public static readonly FrameRate k_30Fps = new FrameRate(30);
        public static readonly FrameRate k_29_97Fps = new FrameRate(29.97002997003);
        public static readonly FrameRate k_50Fps = new FrameRate(50);
        public static readonly FrameRate k_59_94Fps = new FrameRate(59.9400599400599);
        public static readonly FrameRate k_60Fps = new FrameRate(60);

        FrameRate(double framerate) { rate = framerate; }
        public bool IsValid() => rate > TimeUtility.kTimeEpsilon;
        public bool Equals(FrameRate other) => Math.Abs(rate - other.rate) < TimeUtility.kFrameRateEpsilon;
        public override bool Equals(object obj) => obj is FrameRate other && Equals(other);
        public override int GetHashCode() => rate.GetHashCode();
        public static bool operator ==(FrameRate a, FrameRate b) => a.Equals(b);
        public static bool operator !=(FrameRate a, FrameRate b) => !a.Equals(b);

        public static FrameRate DoubleToFrameRate(double rate) => new FrameRate(Math.Ceiling(rate) - rate < TimeUtility.kFrameRateEpsilon ? rate : Math.Ceiling(rate) * 1000.0 / 1001.0);
    }
}
#endif
