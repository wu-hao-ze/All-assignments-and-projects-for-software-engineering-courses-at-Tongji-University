using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Handles an exception if it occurs.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(17)]
    [UnitFooterPorts(ControlOutputs = true)]
    public sealed class TryCatch : Unit
    {
        /// <summary>
        /// The entry point for the try-catch block.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The action to attempt.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput @try { get; private set; }

        /// <summary>
        /// The action to execute if an exception is thrown.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput @catch { get; private set; }

        /// <summary>
        /// The action to execute afterwards, regardless of whether there was an exception.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput @finally { get; private set; }

        /// <summary>
        /// The exception that was thrown in the try block.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput exception { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable]
        [TypeFilter(typeof(Exception), Matching = TypesMatching.AssignableToAll)]
        [TypeSet(TypeSet.SettingsAssembliesTypes)]
        public Type exceptionType { get; set; } = typeof(Exception);

        public override bool canDefine => exceptionType != null && typeof(Exception).IsAssignableFrom(exceptionType);

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            @try = ControlOutput(nameof(@try));
            @catch = ControlOutput(nameof(@catch));
            @finally = ControlOutput(nameof(@finally));
            exception = ValueOutput(exceptionType, nameof(exception));

            Assignment(enter, exception);
            Succession(enter, @try);
            Succession(enter, @catch);
            Succession(enter, @finally);
        }

        public ControlOutput Enter(Flow flow)
        {
            if (flow.isCoroutine)
            {
                throw new NotSupportedException("Coroutines cannot catch exceptions.");
            }

            try
            {
                flow.Invoke(@try);
            }
            catch (Exception ex)
            {
                if (exceptionType.IsInstanceOfType(ex))
                {
                    flow.SetValue(exception, ex);
                    flow.Invoke(@catch);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                flow.Invoke(@finally);
            }

            return null;
        }
    }
}
