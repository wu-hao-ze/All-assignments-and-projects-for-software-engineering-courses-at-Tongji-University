namespace Unity.PlasticSCM.Editor.UI
{
    internal static class UnityMenuItem
    {
        internal static string GetText(string menuName, string subMenuName)
        {
            return string.Format("{0}{1}{2}", menuName, SEPARATOR, subMenuName);
        }

        internal static string EscapedText(string menuName)
        {
            return menuName.Replace(SEPARATOR, ESCAPED_SEPARATOR);
        }

        const string SEPARATOR = "/";
        const string ESCAPED_SEPARATOR = "\u200A\u2215\u200A";
    }
}
