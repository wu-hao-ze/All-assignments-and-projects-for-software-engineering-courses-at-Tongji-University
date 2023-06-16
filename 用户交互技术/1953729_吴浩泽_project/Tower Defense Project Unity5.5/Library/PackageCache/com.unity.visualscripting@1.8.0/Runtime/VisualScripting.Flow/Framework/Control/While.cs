using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Loops as long as a given condition is true.
    /// </summary>
    [UnitTitle("While Loop")]
    [UnitCategory("Control")]
    [UnitOrder(11)]
    public class While : LoopUnit
    {
        /// <summary>
        /// The condition to check at each iteration to determine whether the loop should continue.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput condition { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            condition = ValueInput<bool>(nameof(condition));

            Requirement(condition, enter);
        }

        private int Start(Flow flow)
        {
            return flow.EnterLoop();
        }

        private bool CanMoveNext(Flow flow)
        {
            return flow.GetValue<bool>(condition);
        }

        protected override ControlOutput Loop(Flow flow)
        {
            var loop = Start(flow);

            var stack = flow.PreserveStack();

            while (flow.LoopIsNotBroken(loop) && CanMoveNext(flow))
            {
                flow.Invoke(body);

                flow.RestoreStack(stack);
            }

            flow.DisposePreservedStack(stack);

            flow.ExitLoop(loop);

            return exit;
        }

        protected override IEnumerator LoopCoroutine(Flow flow)
        {
            var loop = Start(flow);

            var stack = flow.PreserveStack();

            while (flow.LoopIsNotBroken(loop) && CanMoveNext(flow))
            {
                yield return body;

                flow.RestoreStack(stack);
            }

            flow.DisposePreservedStack(stack);

            flow.ExitLoop(loop);

            yield return exit;
        }
    }
}
