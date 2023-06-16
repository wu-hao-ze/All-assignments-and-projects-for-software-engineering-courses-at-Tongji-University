using System;

using Codice.LogWrapper;

namespace Unity.PlasticSCM.Editor.Help
{
    internal static class HelpLinkData
    {
        internal static bool TryGet(
            string link, out HelpLink.LinkType type, out string content)
        {
            type = HelpLink.LinkType.Link;
            content = string.Empty;

            int separatorIdx = link.IndexOf(':');
            if (separatorIdx == -1)
                return false;

            string key = link.Substring(0, separatorIdx);

            try
            {
                type = (HelpLink.LinkType)Enum.Parse(
                    typeof(HelpLink.LinkType), key, true);
            }
            catch (Exception ex)
            {
                mLog.ErrorFormat("Unable to get help link data: '{0}': {1}",
                    key, ex.Message);
                mLog.DebugFormat("StackTrace: {0}", ex.StackTrace);

                return false;
            }

            content = link.Substring(separatorIdx + 1);

            return true;
        }

        static readonly ILog mLog = LogManager.GetLogger("HelpLinkData");
    }
}
