using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public abstract class Assigner<TTarget, TAssignee> : IAssigner
        where TAssignee : class
    {
        protected Assigner(TTarget target, TAssignee assignee)
        {
            Ensure.That(nameof(target)).IsNotNull(target);
            Ensure.That(nameof(assignee)).IsNotNull(assignee);

            this.target = target;
            this.assignee = assignee;

            var assignerType = GetType();

            if (!_assignments.ContainsKey(assignerType))
            {
                _assignments.Add(assignerType, Assignment.Fetch(assignerType, typeof(TAssignee)).ToArray());
                _transientAssignments.Add(assignerType, _assignments[assignerType].Where(a => !a.cache).ToArray());
            }
        }

        public TTarget target { get; }

        public TAssignee assignee { get; }

        public bool isDirty { get; set; } = true;

        public void Validate()
        {
            if (isDirty)
            {
                AssignAll();
            }
            else
            {
                AssignTransient();
            }
        }

        protected void AssignAll()
        {
            isDirty = false;

            foreach (var assignment in assignments)
            {
                assignment.Run(this, assignee);
            }
        }

        protected void AssignTransient()
        {
            foreach (var assignment in transientAssignments)
            {
                assignment.Run(this, assignee);
            }
        }

        public virtual void ValueChanged() { }

        public Assignment[] assignments => _assignments[GetType()];
        public Assignment[] transientAssignments => _transientAssignments[GetType()];

        private static readonly Dictionary<Type, Assignment[]> _assignments = new Dictionary<Type, Assignment[]>();

        private static readonly Dictionary<Type, Assignment[]> _transientAssignments = new Dictionary<Type, Assignment[]>();
    }
}
