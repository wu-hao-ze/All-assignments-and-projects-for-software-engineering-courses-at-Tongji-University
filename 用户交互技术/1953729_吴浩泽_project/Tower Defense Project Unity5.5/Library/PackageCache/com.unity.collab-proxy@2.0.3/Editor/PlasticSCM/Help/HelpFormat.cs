namespace Unity.PlasticSCM.Editor.Help
{
    internal class HelpFormat
    {
        internal enum FormatType
        {
            Title,
            Bold,
            Underline
        }

        internal int Position;
        internal int Length;
        internal FormatType Type;
    }
}