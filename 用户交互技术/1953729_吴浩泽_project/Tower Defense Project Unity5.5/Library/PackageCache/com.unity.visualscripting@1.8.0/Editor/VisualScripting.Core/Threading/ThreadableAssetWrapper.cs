using System;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class ThreadableAssetWrapper<T> where T : UnityObject
    {
        public ThreadableAssetWrapper(T asset)
        {
            Ensure.That(nameof(asset)).IsNotNull(asset);

            if (!UnityThread.allowsAPI)
            {
                throw new InvalidOperationException("Threadable asset wrappers must be created on the main thread.");
            }

            this.asset = asset;
            name = asset.name;
        }

        public T asset { get; }
        public string name { get; }

        public static implicit operator T(ThreadableAssetWrapper<T> wrapper)
        {
            return wrapper.asset;
        }
    }
}
