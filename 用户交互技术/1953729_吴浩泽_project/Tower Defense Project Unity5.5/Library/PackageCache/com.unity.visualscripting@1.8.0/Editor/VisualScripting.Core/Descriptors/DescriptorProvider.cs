using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class DescriptorProvider : SingleDecoratorProvider<object, IDescriptor, DescriptorAttribute>, IDisposable
    {
        private readonly Dictionary<object, HashSet<Action>> listeners = new Dictionary<object, HashSet<Action>>();

        protected override bool cache => true;

        private DescriptorProvider()
        {
            PluginContainer.delayCall += () => // The provider gets created at runtime on Start for the debug data
            {
                BoltCore.Configuration.namingSchemeChanged += DescribeAll;
                XmlDocumentation.loadComplete += DescribeAll;
            };
        }

        public override bool IsValid(object described)
        {
            return !described.IsUnityNull();
        }

        public void Dispose()
        {
            BoltCore.Configuration.namingSchemeChanged -= DescribeAll;
            XmlDocumentation.loadComplete -= DescribeAll;
            ClearListeners();
        }

        public void AddListener(object describable, Action onDescriptionChange)
        {
            if (!listeners.ContainsKey(describable))
            {
                listeners.Add(describable, new HashSet<Action>());
            }

            listeners[describable].Add(onDescriptionChange);
        }

        public void RemoveListener(object describable, Action onDescriptionChange)
        {
            if (!listeners.ContainsKey(describable))
            {
                Debug.LogWarning($"Trying to remove unknown description change listener for '{describable}'.");

                return;
            }

            listeners[describable].Remove(onDescriptionChange);

            if (listeners[describable].Count == 0)
            {
                listeners.Remove(describable);
            }
        }

        public void ClearListeners()
        {
            listeners.Clear();
        }

        public void TriggerDescriptionChange(object describable)
        {
            if (!listeners.ContainsKey(describable))
            {
                return;
            }

            foreach (var onDescriptionChange in listeners[describable])
            {
                onDescriptionChange?.Invoke();
            }
        }

        public void Describe(object describable)
        {
            GetDecorator(describable).isDirty = true;
        }

        public void DescribeAll()
        {
            foreach (var descriptor in decorators.Values)
            {
                descriptor.isDirty = true;
            }
        }

        public IDescriptor Descriptor(object target)
        {
            return GetDecorator(target);
        }

        public TDescriptor Descriptor<TDescriptor>(object target) where TDescriptor : IDescriptor
        {
            return GetDecorator<TDescriptor>(target);
        }

        public IDescription Description(object target)
        {
            var descriptor = Descriptor(target);
            descriptor.Validate();
            return descriptor.description;
        }

        public TDescription Description<TDescription>(object target) where TDescription : IDescription
        {
            var description = Description(target);

            if (!(description is TDescription))
            {
                throw new InvalidCastException($"Description type mismatch for '{target}': found {description.GetType()}, expected {typeof(TDescription)}.");
            }

            return (TDescription)description;
        }

        static DescriptorProvider()
        {
            instance = new DescriptorProvider();
        }

        public static DescriptorProvider instance { get; }
    }

    public static class XDescriptorProvider
    {
        public static void Describe(this object target)
        {
            DescriptorProvider.instance.Describe(target);
        }

        public static bool HasDescriptor(this object target)
        {
            Ensure.That(nameof(target)).IsNotNull(target);

            return DescriptorProvider.instance.HasDecorator(target.GetType());
        }

        public static IDescriptor Descriptor(this object target)
        {
            return DescriptorProvider.instance.Descriptor(target);
        }

        public static TDescriptor Descriptor<TDescriptor>(this object target) where TDescriptor : IDescriptor
        {
            return DescriptorProvider.instance.Descriptor<TDescriptor>(target);
        }

        public static IDescription Description(this object target)
        {
            return DescriptorProvider.instance.Description(target);
        }

        public static TDescription Description<TDescription>(this object target) where TDescription : IDescription
        {
            return DescriptorProvider.instance.Description<TDescription>(target);
        }
    }
}
