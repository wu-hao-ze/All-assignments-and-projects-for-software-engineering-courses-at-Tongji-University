using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public sealed class GraphStack : GraphPointer, IPoolable, IDisposable
    {
        #region Lifecycle

        private GraphStack() { }

        private void InitializeNoAlloc(IGraphRoot root, List<IGraphParentElement> parentElements, bool ensureValid)
        {
            Initialize(root);

            Ensure.That(nameof(parentElements)).IsNotNull(parentElements);

            foreach (var parentElement in parentElements)
            {
                if (!TryEnterParentElement(parentElement, out var error))
                {
                    if (ensureValid)
                    {
                        throw new GraphPointerException(error, this);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        internal static GraphStack New(IGraphRoot root, List<IGraphParentElement> parentElements)
        {
            var stack = GenericPool<GraphStack>.New(() => new GraphStack());
            stack.InitializeNoAlloc(root, parentElements, true);
            return stack;
        }

        internal static GraphStack New(GraphPointer model)
        {
            var stack = GenericPool<GraphStack>.New(() => new GraphStack());
            stack.CopyFrom(model);
            return stack;
        }

        public GraphStack Clone()
        {
            return New(this);
        }

        public void Dispose()
        {
            GenericPool<GraphStack>.Free(this);
        }

        void IPoolable.New()
        {
        }

        void IPoolable.Free()
        {
            root = null;
            parentStack.Clear();
            parentElementStack.Clear();
            graphStack.Clear();
            dataStack.Clear();
            debugDataStack.Clear();
        }

        #endregion

        #region Conversion

        public override GraphReference AsReference()
        {
            return ToReference();
        }

        public GraphReference ToReference()
        {
            return GraphReference.Intern(this);
        }

        #endregion

        #region Traversal

        public new void EnterParentElement(IGraphParentElement parentElement)
        {
            base.EnterParentElement(parentElement);
        }

        public bool TryEnterParentElement(IGraphParentElement parentElement)
        {
            return TryEnterParentElement(parentElement, out var error);
        }

        public bool TryEnterParentElementUnsafe(IGraphParentElement parentElement)
        {
            return TryEnterParentElement(parentElement, out var error, null, true);
        }

        public new void ExitParentElement()
        {
            base.ExitParentElement();
        }

        #endregion
    }
}
