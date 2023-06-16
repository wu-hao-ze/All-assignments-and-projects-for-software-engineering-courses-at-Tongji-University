namespace Unity.VisualScripting
{
    public struct CustomEventArgs
    {
        public readonly string name;

        public readonly object[] arguments;

        public CustomEventArgs(string name, params object[] arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }
    }
}
