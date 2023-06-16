namespace Unity.VisualScripting
{
    public abstract class GlobalEventUnit<TArgs> : EventUnit<TArgs>
    {
        protected override bool register => true;

        protected virtual string hookName => throw new InvalidImplementationException();

        public override EventHook GetHook(GraphReference reference)
        {
            return hookName;
        }
    }
}
