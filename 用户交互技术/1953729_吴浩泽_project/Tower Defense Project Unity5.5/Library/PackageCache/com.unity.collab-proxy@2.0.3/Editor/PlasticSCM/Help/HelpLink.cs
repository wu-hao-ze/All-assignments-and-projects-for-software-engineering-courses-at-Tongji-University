namespace Unity.PlasticSCM.Editor.Help
{
    internal class HelpLink
    {
        internal enum LinkType
        {
            Action,
            Help,
            Link,
        }

        internal int Position;
        internal int Length;
        internal string Link;

        internal LinkType Type = LinkType.Action;
    }
}