using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting.FullSerializer.Internal;

namespace Unity.VisualScripting
{
    public sealed class EnumerableCloner : Cloner<IEnumerable>
    {
        public override bool Handles(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && GetAddMethod(type) != null;
        }

        public override void FillClone(Type type, ref IEnumerable clone, IEnumerable original, CloningContext context)
        {
            var addMethod = GetAddMethod(type);

            if (addMethod == null)
            {
                throw new InvalidOperationException($"Cannot instantiate enumerable type '{type}' because it does not provide an add method.");
            }

            // No way to preserve instances here

            foreach (var item in original)
            {
                addMethod.Invoke(item, Cloning.Clone(context, item));
            }
        }

        private readonly Dictionary<Type, IOptimizedInvoker> addMethods = new Dictionary<Type, IOptimizedInvoker>();

        private IOptimizedInvoker GetAddMethod(Type type)
        {
            if (!addMethods.ContainsKey(type))
            {
                var addMethod = fsReflectionUtility.GetInterface(type, typeof(ICollection<>))?.GetDeclaredMethod("Add") ??
                    type.GetFlattenedMethod("Add") ??
                    type.GetFlattenedMethod("Push") ??
                    type.GetFlattenedMethod("Enqueue");

                addMethods.Add(type, addMethod?.Prewarm());
            }

            return addMethods[type];
        }
    }
}
