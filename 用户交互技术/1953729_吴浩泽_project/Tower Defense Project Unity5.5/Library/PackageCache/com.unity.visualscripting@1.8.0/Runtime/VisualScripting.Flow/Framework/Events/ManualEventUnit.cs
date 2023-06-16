namespace Unity.VisualScripting
{
    public abstract class ManualEventUnit<TArgs> : EventUnit<TArgs>
    {
        protected sealed override bool register => false;

        protected abstract string hookName { get; }

        public sealed override EventHook GetHook(GraphReference reference)
        {
            return hookName;
        }
    }
}
