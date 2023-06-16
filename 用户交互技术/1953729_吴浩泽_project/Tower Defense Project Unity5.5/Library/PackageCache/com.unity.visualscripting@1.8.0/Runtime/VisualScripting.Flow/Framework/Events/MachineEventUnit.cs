namespace Unity.VisualScripting
{
    public abstract class MachineEventUnit<TArgs> : EventUnit<TArgs>
    {
        protected sealed override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(hookName, reference.machine);
        }

        protected virtual string hookName => throw new InvalidImplementationException($"Missing event hook for '{this}'.");
    }
}
