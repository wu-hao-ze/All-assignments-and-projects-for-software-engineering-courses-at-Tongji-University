using System;
using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Loops over each element of a collection.
    /// </summary>
    [UnitTitle("For Each Loop")]
    [UnitCategory("Control")]
    [UnitOrder(10)]
    public class ForEach : LoopUnit
    {
        /// <summary>
        /// The collection over which to loop.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput collection { get; private set; }

        /// <summary>
        /// The current index of the loop.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Index")]
        public ValueOutput currentIndex { get; private set; }

        /// <summary>
        /// The key of the current item of the loop.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Key")]
        public ValueOutput currentKey { get; private set; }

        /// <summary>
        /// The current item of the loop.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Item")]
        public ValueOutput currentItem { get; private set; }

        [Serialize]
        [Inspectable, UnitHeaderInspectable("Dictionary")]
        [InspectorToggleLeft]
        public bool dictionary { get; set; }

        protected override void Definition()
        {
            base.Definition();

            if (dictionary)
            {
                collection = ValueInput<IDictionary>(nameof(collection));
            }
            else
            {
                collection = ValueInput<IEnumerable>(nameof(collection));
            }

            currentIndex = ValueOutput<int>(nameof(currentIndex));

            if (dictionary)
            {
                currentKey = ValueOutput<object>(nameof(currentKey));
            }

            currentItem = ValueOutput<object>(nameof(currentItem));

            Requirement(collection, enter);
            Assignment(enter, currentIndex);
            Assignment(enter, currentItem);

            if (dictionary)
            {
                Assignment(enter, currentKey);
            }
        }

        private int Start(Flow flow, out IEnumerator enumerator, out IDictionaryEnumerator dictionaryEnumerator, out int currentIndex)
        {
            if (dictionary)
            {
                dictionaryEnumerator = flow.GetValue<IDictionary>(collection).GetEnumerator();
                enumerator = dictionaryEnumerator;
            }
            else
            {
                enumerator = flow.GetValue<IEnumerable>(collection).GetEnumerator();
                dictionaryEnumerator = null;
            }

            currentIndex = -1;

            return flow.EnterLoop();
        }

        private bool MoveNext(Flow flow, IEnumerator enumerator, IDictionaryEnumerator dictionaryEnumerator, ref int currentIndex)
        {
            var result = enumerator.MoveNext();

            if (result)
            {
                if (dictionary)
                {
                    flow.SetValue(currentKey, dictionaryEnumerator.Key);
                    flow.SetValue(currentItem, dictionaryEnumerator.Value);
                }
                else
                {
                    flow.SetValue(currentItem, enumerator.Current);
                }

                currentIndex++;

                flow.SetValue(this.currentIndex, currentIndex);
            }

            return result;
        }

        protected override ControlOutput Loop(Flow flow)
        {
            var loop = Start(flow, out var enumerator, out var dictionaryEnumerator, out var currentIndex);

            var stack = flow.PreserveStack();

            try
            {
                while (flow.LoopIsNotBroken(loop) && MoveNext(flow, enumerator, dictionaryEnumerator, ref currentIndex))
                {
                    flow.Invoke(body);

                    flow.RestoreStack(stack);
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            flow.DisposePreservedStack(stack);

            flow.ExitLoop(loop);

            return exit;
        }

        protected override IEnumerator LoopCoroutine(Flow flow)
        {
            var loop = Start(flow, out var enumerator, out var dictionaryEnumerator, out var currentIndex);

            var stack = flow.PreserveStack();

            try
            {
                while (flow.LoopIsNotBroken(loop) && MoveNext(flow, enumerator, dictionaryEnumerator, ref currentIndex))
                {
                    yield return body;

                    flow.RestoreStack(stack);
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            flow.DisposePreservedStack(stack);

            flow.ExitLoop(loop);

            yield return exit;
        }
    }
}
