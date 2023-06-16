namespace Unity.VisualScripting
{
    public abstract class OperatorHandler
    {
        protected OperatorHandler(string name, string verb, string symbol, string customMethodName)
        {
            Ensure.That(nameof(name)).IsNotNull(name);
            Ensure.That(nameof(verb)).IsNotNull(verb);
            Ensure.That(nameof(symbol)).IsNotNull(symbol);

            this.name = name;
            this.verb = verb;
            this.symbol = symbol;
            this.customMethodName = customMethodName;
        }

        public string name { get; }
        public string verb { get; }
        public string symbol { get; }
        public string customMethodName { get; }
    }
}
