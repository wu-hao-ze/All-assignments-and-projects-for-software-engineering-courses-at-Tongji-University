using System.Collections.Generic;

namespace Unity.PlasticSCM.Editor.Help
{
    internal class HelpData
    {
        internal List<HelpFormat> FormattedBlocks = new List<HelpFormat>();
        internal List<HelpLink> Links = new List<HelpLink>();
        internal string CleanText;
        internal bool ShouldShowAgain;
    }
}