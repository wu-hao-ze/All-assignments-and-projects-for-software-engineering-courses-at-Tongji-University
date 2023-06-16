using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity.VisualScripting
{
    /// <summary>
    /// A list of widgets that can be safely iterated over even if the collection changes during iteration.
    /// </summary>
    public class WidgetList<TWidget> : Collection<TWidget>, IEnumerable<TWidget> where TWidget : class, IWidget
    {
        private uint version;

        private readonly ICanvas canvas;

        public WidgetList(ICanvas canvas)
        {
            Ensure.That(nameof(canvas)).IsNotNull(canvas);

            this.canvas = canvas;
        }

        protected override void InsertItem(int index, TWidget item)
        {
            base.InsertItem(index, item);
            version++;
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            version++;
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            version++;
        }

        public new Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TWidget> IEnumerable<TWidget>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<TWidget>
        {
            private readonly WidgetList<TWidget> list;

            private int index;

            private TWidget current;

            private bool invalid;

            private readonly uint version;

            private IGraph graph;

            public Enumerator(WidgetList<TWidget> list) : this()
            {
                this.list = list;
                version = list.version;

                // Micro optim
                graph = list.canvas.graph;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                // Stop if the list changed
                if (version != list.version)
                {
                    current = null;
                    invalid = true;
                    return false;
                }

                // Micro optim
                var count = list.Count;

                // Find the next element that is within the graph
                while (index < count)
                {
                    current = list[index];
                    index++;

                    if (current.item.graph == graph)
                    {
                        return true;
                    }
                }

                // Expleted the list
                current = null;
                invalid = true;
                return false;
            }

            public TWidget Current => current;

            object IEnumerator.Current
            {
                get
                {
                    if (invalid)
                    {
                        throw new InvalidOperationException();
                    }

                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
