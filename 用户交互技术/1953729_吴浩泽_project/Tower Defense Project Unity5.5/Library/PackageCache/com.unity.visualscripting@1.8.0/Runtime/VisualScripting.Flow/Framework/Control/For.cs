using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Loops between a first and last index at a specified step.
    /// </summary>
    [UnitTitle("For Loop")]
    [UnitCategory("Control")]
    [UnitOrder(9)]
    public sealed class For : LoopUnit
    {
        /// <summary>
        /// The index at which to start the loop (inclusive).
        /// </summary>
        [PortLabel("First")]
        [DoNotSerialize]
        public ValueInput firstIndex { get; private set; }

        /// <summary>
        /// The index at which to end the loop (exclusive).
        /// </summary>
        [PortLabel("Last")]
        [DoNotSerialize]
        public ValueInput lastIndex { get; private set; }

        /// <summary>
        /// The value by which the index will be incremented (or decremented, if negative) after each loop.
        /// </summary>
        [DoNotSerialize]
        public ValueInput step { get; private set; }

        /// <summary>
        /// The current index of the loop.
        /// </summary>
        [PortLabel("Index")]
        [DoNotSerialize]
        public ValueOutput currentIndex { get; private set; }

        protected override void Definition()
        {
            firstIndex = ValueInput(nameof(firstIndex), 0);
            lastIndex = ValueInput(nameof(lastIndex), 10);
            step = ValueInput(nameof(step), 1);
            currentIndex = ValueOutput<int>(nameof(currentIndex));
            base.Definition();

            Requirement(firstIndex, enter);
            Requirement(lastIndex, enter);
            Requirement(step, enter);
            Assignment(enter, currentIndex);
        }

        private int Start(Flow flow, out int currentIndex, out int lastIndex, out bool ascending)
        {
            var firstIndex = flow.GetValue<int>(this.firstIndex);
            lastIndex = flow.GetValue<int>(this.lastIndex);
            ascending = firstIndex <= lastIndex;
            currentIndex = firstIndex;
            flow.SetValue(this.currentIndex, currentIndex);

            return flow.EnterLoop();
        }

        private bool CanMoveNext(int currentIndex, int lastIndex, bool ascending)
        {
            if (ascending)
            {
                return currentIndex < lastIndex;
            }
            else
            {
                return currentIndex > lastIndex;
            }
        }

        private void MoveNext(Flow flow, ref int currentIndex)
        {
            currentIndex += flow.GetValue<int>(step);
            flow.SetValue(this.currentIndex, currentIndex);
        }

        protected override ControlOutput Loop(Flow flow)
        {
            var loop = Start(flow, out int currentIndex, out int lastIndex, out bool ascending);

            if (!IsStepValueZero())
            {
                var stack = flow.PreserveStack();

                while (flow.LoopIsNotBroken(loop) && CanMoveNext(currentIndex, lastIndex, ascending))
                {
                    flow.Invoke(body);

                    flow.RestoreStack(stack);

                    MoveNext(flow, ref currentIndex);
                }

                flow.DisposePreservedStack(stack);
            }

            flow.ExitLoop(loop);

            return exit;
        }

        protected override IEnumerator LoopCoroutine(Flow flow)
        {
            var loop = Start(flow, out int currentIndex, out int lastIndex, out bool ascending);

            var stack = flow.PreserveStack();

            while (flow.LoopIsNotBroken(loop) && CanMoveNext(currentIndex, lastIndex, ascending))
            {
                yield return body;

                flow.RestoreStack(stack);

                MoveNext(flow, ref currentIndex);
            }

            flow.DisposePreservedStack(stack);

            flow.ExitLoop(loop);

            yield return exit;
        }

        public bool IsStepValueZero()
        {
            var isDefaultZero = !step.hasValidConnection && (int)defaultValues[step.key] == 0;
            var isConnectedToLiteralZero = false;

            if (step.hasValidConnection && step.connection.source.unit is Literal literal)
            {
                if (Convert.ToInt32(literal.value) == 0)
                {
                    isConnectedToLiteralZero = true;
                }
            }

            return isDefaultZero || isConnectedToLiteralZero;
        }
    }
}
