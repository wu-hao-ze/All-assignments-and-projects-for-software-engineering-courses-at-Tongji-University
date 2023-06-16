using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Exposes all members of the type.
    /// </summary>
    [SpecialUnit]
    public sealed class Expose : Unit, IAotStubbable
    {
        public Expose() : base() { }

        public Expose(Type type) : base()
        {
            this.type = type;
        }

        [Serialize, Inspectable, TypeFilter(Enums = false)]
        public Type type { get; set; }

        [Serialize, Inspectable, UnitHeaderInspectable("Instance"), InspectorToggleLeft]
        public bool instance { get; set; } = true;

        [Serialize, Inspectable, UnitHeaderInspectable("Static"), InspectorToggleLeft]
        public bool @static { get; set; } = true;

        /// <summary>
        /// The instance of the exposed type.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput target { get; private set; }

        [DoNotSerialize]
        public Dictionary<ValueOutput, Member> members { get; private set; }

        public override bool canDefine => type != null;

        public override IEnumerable<object> GetAotStubs(HashSet<object> visited)
        {
            if (members != null)
            {
                foreach (var member in members.Values)
                {
                    if (member != null && member.isReflected)
                    {
                        yield return member.info;
                    }
                }
            }
        }

        protected override void Definition()
        {
            members = new Dictionary<ValueOutput, Member>();

            var requiresTarget = false;

            foreach (var member in type.GetMembers()
                     .Where(m => m is FieldInfo || m is PropertyInfo)
                     .Select(m => m.ToManipulator(type))
                     .DistinctBy(m => m.name)                   // To account for "new" duplicates
                     .Where(Include)
                     .OrderBy(m => m.requiresTarget ? 0 : 1)
                     .ThenBy(m => m.order))
            {
                var memberPort = ValueOutput(member.type, member.name, (flow) => GetValue(flow, member));

                if (member.isPredictable)
                {
                    memberPort.Predictable();
                }

                members.Add(memberPort, member);

                if (member.requiresTarget)
                {
                    requiresTarget = true;
                }
            }

            if (requiresTarget)
            {
                target = ValueInput(type, nameof(target)).NullMeansSelf();

                target.SetDefaultValue(type.PseudoDefault());

                foreach (var member in members.Keys)
                {
                    if (members[member].requiresTarget)
                    {
                        Requirement(target, member);
                    }
                }
            }
        }

        private bool Include(Member member)
        {
            if (!instance && member.requiresTarget)
            {
                return false;
            }

            if (!@static && !member.requiresTarget)
            {
                return false;
            }

            if (!member.isPubliclyGettable)
            {
                return false;
            }

            if (member.info.HasAttribute<ObsoleteAttribute>())
            {
                return false;
            }

            if (member.isIndexer)
            {
                return false;
            }

            // Pesky edit-mode only accessor that is only available in the editor,
            // yet isn't marked by any special attribute to indicate it.
            if (member.name == "runInEditMode" && member.declaringType == typeof(MonoBehaviour))
            {
                return false;
            }

            return true;
        }

        private object GetValue(Flow flow, Member member)
        {
            var target = member.requiresTarget ? flow.GetValue(this.target, member.targetType) : null;

            return member.Get(target);
        }
    }
}
