using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class Assignment
    {
        public Assignment(Member assigner, Type assigneeType)
        {
            Ensure.That(nameof(assigneeType)).IsNotNull(assigneeType);

            this.assigner = assigner;

            var assignsAttribute = assigner.info.GetAttribute<AssignsAttribute>();
            assignee = new Member(assigneeType, assignsAttribute?.memberName ?? assigner.name.FirstCharacterToLower());
            requiresAPI = assigner.info.HasAttribute<RequiresUnityAPIAttribute>();
            cache = assignsAttribute?.cache ?? true;

            assigner.Prewarm();
            assignee.Prewarm();
        }

        public Member assigner { get; }
        public Member assignee { get; }
        public bool requiresAPI { get; }
        public bool cache { get; }

        public void Run(object assigner, object assignee)
        {
            if (requiresAPI)
            {
                UnityAPI.Async(() => _Run(assigner, assignee));
            }
            else
            {
                _Run(assigner, assignee);
            }
        }

        private void _Run(object assigner, object assignee)
        {
            var oldValue = this.assignee.Get(assignee);
            var newValue = ConversionUtility.Convert(this.assigner.Invoke(assigner), this.assignee.type);

            this.assignee.Set(assignee, newValue);

            if (!Equals(oldValue, newValue))
            {
                if (assigner is IAssigner _assigner)
                {
                    _assigner.ValueChanged();
                }
            }
        }

        public static IEnumerable<Assignment> Fetch(Type descriptorType, Type descriptionType)
        {
            var bindingFlags = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic;

            return descriptorType.GetMethods(bindingFlags)
                .Where(m => m.HasAttribute<AssignsAttribute>())
                .Select(m => new Assignment(m.ToManipulator(descriptorType), descriptionType));
        }
    }
}
