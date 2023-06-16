using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Unity.VisualScripting
{
    public static class ConsoleProfiler
    {
        private static readonly Color[] levelColors =
        {
            new Color(.14f, .65f, 1.00f),
            new Color(.89f, .12f, .12f),
            new Color(.09f, .85f, .43f),
            new Color(.80f, .24f, 1.00f),
            new Color(1.00f, .79f, .0f),
            new Color(.09f, .80f, .85f),
            new Color(.66f, .85f, .09f),
        };

        [Conditional("ENABLE_PROFILER")]
        public static void Dump(TimeSpan threshold)
        {
            if (ProfilingUtility.rootSegment.children.Count == 0)
            {
                return;
            }

            var sb = new StringBuilder();

            Append(sb, 0, ProfilingUtility.rootSegment, threshold);

            ProfilingUtility.Clear();

            Debug.Log(sb);
        }

        [Conditional("ENABLE_PROFILER")]
        public static void Dump()
        {
            Dump(TimeSpan.Zero);
        }

        private static void Append(StringBuilder sb, int indent, ProfiledSegment segmentParent, TimeSpan threshold)
        {
            var totalElapsed = TimeSpan.Zero;

            foreach (var segment in segmentParent.children.OrderByDescending(s => s.stopwatch.Elapsed))
            {
                if (segment.stopwatch.Elapsed < threshold)
                {
                    continue;
                }

                totalElapsed += segment.stopwatch.Elapsed;
                var elapsedProportion = segmentParent.parent == null ? 1 : segment.stopwatch.Elapsed.TotalMilliseconds / segmentParent.stopwatch.Elapsed.TotalMilliseconds;
                var color = levelColors[indent % levelColors.Length];

                sb.AppendLineFormat
                    (
                        "<color=#{0}>{1}<b>{2}:</b> {3:0.0}ms ({4:0}%) ({5}x, ~{6:0.000}ms/x)</color>",
                        color.ToHexString(),
                        new string(' ', indent * 4),
                        segment.name,
                        segment.stopwatch.Elapsed.TotalMilliseconds,
                        elapsedProportion * 100,
                        segment.calls,
                        segment.stopwatch.Elapsed.TotalMilliseconds / segment.calls
                    );

                indent++;
                Append(sb, indent, segment, threshold);
                indent--;
            }

            if (segmentParent.children.Count > 0)
            {
                var remainingElapsed = (segmentParent.stopwatch.Elapsed - totalElapsed);

                if (remainingElapsed > threshold)
                {
                    var remainingElapsedProportion = remainingElapsed.TotalMilliseconds / segmentParent.stopwatch.Elapsed.TotalMilliseconds;
                    var color = levelColors[indent % levelColors.Length];

                    sb.AppendLineFormat
                        (
                            "<color=#{4}>{0}<b>{1}:</b> {2:0.0}ms ({3:0}%)</color>",
                            new string(' ', indent * 4),
                            "Remaining",
                            remainingElapsed.TotalMilliseconds,
                            remainingElapsedProportion * 100,
                            color.ToHexString()
                        );
                }
            }
        }
    }
}
