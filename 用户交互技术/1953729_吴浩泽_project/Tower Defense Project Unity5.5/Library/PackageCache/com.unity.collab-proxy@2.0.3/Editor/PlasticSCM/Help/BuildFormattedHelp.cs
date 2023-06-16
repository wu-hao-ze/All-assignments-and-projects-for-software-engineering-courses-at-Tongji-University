using System.Collections.Generic;
using System.Text;
using System.Linq;

using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Help
{
    internal class FormattedHelpLink
    {
        internal HelpLink Source;
        internal int Position;
        internal int Length;
    }

    internal static class BuildFormattedHelp
    {
        internal static void ForData(
            string plainText,
            HelpFormat[] formattedBlocks,
            HelpLink[] links,
            out string formattedHelpText,
            out List<FormattedHelpLink> formattedLinks)
        {
            formattedHelpText = string.Empty;
            formattedLinks = new List<FormattedHelpLink>();

            List<object> segments = new List<object>();
            segments.AddRange(formattedBlocks);
            segments.AddRange(links);
            var sortedSegments = segments.OrderBy(
                n => n is HelpFormat ?
                ((HelpFormat)n).Position :
                ((HelpLink)n).Position);

            StringBuilder sb = new StringBuilder();

            int lastIndex = 0;
            foreach (var segment in sortedSegments)
            {
                Segment.Data segmentData = GetSegmentData(segment);

                if (segmentData.Begin > lastIndex)
                    sb.Append(plainText.Substring(lastIndex, segmentData.Begin - lastIndex));

                string plainSegment = plainText.Substring(
                    segmentData.Begin, segmentData.Length);

                if (segment is HelpLink)
                {
                    formattedLinks.Add(new FormattedHelpLink()
                    {
                        Source = (HelpLink)segment,
                        Position = sb.Length,
                        Length = segmentData.Prefix.Length +
                                 plainSegment.Length +
                                 segmentData.Suffix.Length
                    });
                }

                sb.Append(segmentData.Prefix);
                sb.Append(plainSegment);
                sb.Append(segmentData.Suffix);

                lastIndex = segmentData.Begin + segmentData.Length;
            }

            sb.Append(plainText.Substring(lastIndex));

            formattedHelpText = sb.ToString();
        }

        internal static bool IsLinkMetaChar(
            FormattedHelpLink formattedLink,
            int charIndex)
        {
            int prefixEndIndex =
                formattedLink.Position +
                Segment.LINK_PREFIX.Length - 1;

            if (formattedLink.Position <= charIndex &&
                charIndex <= prefixEndIndex)
                return true;

            int suffixStartIndex =
                formattedLink.Position +
                formattedLink.Length - Segment.LINK_SUFFIX.Length;

            if (suffixStartIndex <= charIndex &&
                charIndex <= (formattedLink.Position + formattedLink.Length - 1))
                return true;

            return false;
        }

        static Segment.Data GetSegmentData(object segment)
        {
            if (segment is HelpLink)
            {
                HelpLink link = (HelpLink)segment;
                return Segment.BuildForLink(link);
            }

            HelpFormat format = (HelpFormat)segment;
            return Segment.BuildForFormat(format);
        }

        static class Segment
        {
            internal class Data
            {
                internal int Begin;
                internal int Length;
                internal string Prefix;
                internal string Suffix;
            }

            internal static Data BuildForLink(HelpLink link)
            {
                return new Data()
                {
                    Begin = link.Position,
                    Length = link.Length,
                    Prefix = LINK_PREFIX,
                    Suffix = LINK_SUFFIX
                };
            }

            internal static Data BuildForFormat(HelpFormat format)
            {
                switch (format.Type)
                {
                    case HelpFormat.FormatType.Title:
                        return new Data()
                        {
                            Begin = format.Position,
                            Length = format.Length,
                            Prefix = TITLE_PREFIX,
                            Suffix = TITLE_SUFFIX
                        };
                    case HelpFormat.FormatType.Bold:
                        return new Data()
                        {
                            Begin = format.Position,
                            Length = format.Length,
                            Prefix = BOLD_PREFIX,
                            Suffix = BOLD_SUFFIX
                        };
                    case HelpFormat.FormatType.Underline:
                        // NOTE(rafa): No support yet for underline, we use italic instead
                        return new Data()
                        {
                            Begin = format.Position,
                            Length = format.Length,
                            Prefix = ITALIC_PREFIX,
                            Suffix = ITALIC_SUFFIX
                        };
                    default:
                        return null;
                }
            }

            internal const string LINK_PREFIX = "<color=\"" + UnityStyles.HexColors.LINK_COLOR + "\">";
            internal const string LINK_SUFFIX = "</color>";
            const string TITLE_PREFIX = "<size=16>";
            const string TITLE_SUFFIX = "</size>";
            const string BOLD_PREFIX = "<b>";
            const string BOLD_SUFFIX = "</b>";
            const string ITALIC_PREFIX = "<i>";
            const string ITALIC_SUFFIX = "</i>";
        }
    }
}
