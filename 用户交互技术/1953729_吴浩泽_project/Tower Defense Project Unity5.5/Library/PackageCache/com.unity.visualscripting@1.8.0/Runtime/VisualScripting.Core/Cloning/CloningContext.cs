using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public sealed class CloningContext : IPoolable, IDisposable
    {
        public Dictionary<object, object> clonings { get; } = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);

        public ICloner fallbackCloner { get; private set; }

        public bool tryPreserveInstances { get; private set; }

        private bool disposed;

        void IPoolable.New()
        {
            disposed = false;
        }

        void IPoolable.Free()
        {
            disposed = true;
            clonings.Clear();
        }

        public void Dispose()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(ToString());
            }

            GenericPool<CloningContext>.Free(this);
        }

        public static CloningContext New(ICloner fallbackCloner, bool tryPreserveInstances)
        {
            var context = GenericPool<CloningContext>.New(() => new CloningContext());
            context.fallbackCloner = fallbackCloner;
            context.tryPreserveInstances = tryPreserveInstances;
            return context;
        }
    }
}
