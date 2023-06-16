using System.Collections.Generic;

namespace Unity.PlasticSCM.Editor.Help
{
    internal static class TestingHelpData
    {
        internal static HelpData GetSample1()
        {
            HelpData result = new HelpData();

            result.CleanText = "There are some .private files" + System.Environment.NewLine + System.Environment.NewLine +

"Do not panic, these are copies Plastic creates to preserve files it can't overwrite." + System.Environment.NewLine + System.Environment.NewLine +

"Suppose you have a private file \"src / foo.c\", then switch your workspace to a branch where someone added \"src / foo.c\". Plastic downloads the new file because it is under source control and yours is not. But, it can't delete yours, so it renames it as .private.0." + System.Environment.NewLine + System.Environment.NewLine +

"Makes sense?" + System.Environment.NewLine + System.Environment.NewLine +

"Learn more:" + System.Environment.NewLine +
"* You have some files ready to be added to version control" + System.Environment.NewLine +
"* Are you missing any changes?" + System.Environment.NewLine +
"* Tips to work with Visual Studio projects." + System.Environment.NewLine + System.Environment.NewLine +
"This is just text after links (like this help link -> content2) to verify that the format is preserved." + System.Environment.NewLine + System.Environment.NewLine +
"And then that another link at the end works, with some bold text at the end, final!!";

            // IMPORTANT! We need single EOL chars to calculate the positions,
            // otherwise positions are wrong calculated
            result.CleanText = result.CleanText.Replace("\r\n", "\n");

            result.FormattedBlocks = new List<HelpFormat>();
            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Title,
                Position = 0,
                Length = 29
            });

            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Bold,
                Position = result.CleanText.IndexOf("not panic"),
                Length = "not panic".Length
            });

            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Underline,
                Position = result.CleanText.IndexOf("overwrite"),
                Length = "overwrite".Length
            });

            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Bold,
                Position = result.CleanText.IndexOf("Makes sense?"),
                Length = "Makes sense?".Length
            });

            result.Links = new List<HelpLink>();
            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("You have some files ready to be added to version control"),
                Length = "You have some files ready to be added to version control".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Action, "plasticscm-pendingchanges-filestoadd")
            });
            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("Are you missing any changes?"),
                Length = "Are you missing any changes?".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Action, "plasticscm-pendingchanges-missingchanges")
            });
            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("Tips to work with Visual Studio projects."),
                Length = "Tips to work with Visual Studio projects.".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Action, "plasticscm-pendingchanges-visualstudio")
            });

            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Bold,
                Position = result.CleanText.IndexOf("verify that the format is preserved"),
                Length = "verify that the format is preserved".Length
            });

            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("this help link"),
                Length = "this help link".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Help, "sample2")
            });

            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("another link at the end"),
                Length = "another link at the end".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Link, "https://www.google.com")
            });

            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Bold,
                Position = result.CleanText.IndexOf("bold text at the end"),
                Length = "bold text at the end".Length
            });

            return result;
        }

        internal static HelpData GetSample2()
        {
            HelpData result = new HelpData();

            result.CleanText = "Alternative title to confirm that all is working" + System.Environment.NewLine + System.Environment.NewLine +

"This is just another help example to ensure that the panel replaces the helps dynamically." + System.Environment.NewLine + System.Environment.NewLine +

"If you're reading this text, means that the help changed its content dynamically, so we can navigate between help tips by clicking hyperlinks" + System.Environment.NewLine + System.Environment.NewLine +

"Makes sense?" + System.Environment.NewLine + System.Environment.NewLine +

"Learn more:" + System.Environment.NewLine +
"* You have some files ready to be added to version control" + System.Environment.NewLine +
"* Are you missing any changes?" + System.Environment.NewLine +
"* Tips to work with Visual Studio projects." + System.Environment.NewLine + System.Environment.NewLine +
"This is just text after links (like this help link -> content1) to verify that the format is preserved.";

            // IMPORTANT! We need single EOL chars to calculate the positions,
            // otherwise positions are wrong calculated
            result.CleanText = result.CleanText.Replace("\r\n", "\n");

            result.FormattedBlocks = new List<HelpFormat>();
            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Title,
                Position = 0,
                Length = "Alternative title to confirm that all is working".Length
            });

            result.FormattedBlocks.Add(new HelpFormat()
            {
                Type = HelpFormat.FormatType.Bold,
                Position = result.CleanText.IndexOf("replaces the helps dynamically"),
                Length = "replaces the helps dynamically".Length
            });

            result.Links = new List<HelpLink>();
            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("You have some files ready to be added to version control"),
                Length = "You have some files ready to be added to version control".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Action, "plasticscm-pendingchanges-filestoadd")
            });
            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("Are you missing any changes?"),
                Length = "Are you missing any changes?".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Action, "plasticscm-pendingchanges-missingchanges")
            });
            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("Tips to work with Visual Studio projects."),
                Length = "Tips to work with Visual Studio projects.".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Action, "plasticscm-pendingchanges-visualstudio")
            });

            result.Links.Add(new HelpLink()
            {
                Position = result.CleanText.IndexOf("this help link"),
                Length = "this help link".Length,
                Link = HelpLinkData.AsString(HelpLink.LinkType.Help, "sample1")
            });

            return result;
        }

        class HelpLinkData
        {
            internal static string AsString(HelpLink.LinkType linkType, string linkContent)
            {
                string linkTypeString = string.Empty;

                switch (linkType)
                {
                    case HelpLink.LinkType.Action:
                        linkTypeString = ACTION;
                        break;
                    case HelpLink.LinkType.Help:
                        linkTypeString = HELP;
                        break;
                    case HelpLink.LinkType.Link:
                        linkTypeString = LINK;
                        break;
                }

                return string.Concat(linkTypeString, SEPARATOR, linkContent);
            }

            const string ACTION = "action";
            const string HELP = "help";
            const string LINK = "link";
            const string SEPARATOR = ":";
        }
    }
}