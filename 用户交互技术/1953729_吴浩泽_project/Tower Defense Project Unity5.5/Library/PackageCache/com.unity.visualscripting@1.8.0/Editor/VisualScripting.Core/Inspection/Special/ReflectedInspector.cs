using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class ReflectedInspector : Inspector
    {
        public ReflectedInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            bindingFlags = MemberMetadata.DefaultBindingFlags;

            metadata.valueTypeChanged += previousType => ReflectMetadata();
        }

        public BindingFlags bindingFlags { get; private set; }

        private float _adaptiveWidth;

        protected virtual bool Include(MemberInfo m)
        {
            if (m.HasAttribute<InspectableAttribute>())
            {
                return true;
            }

            var conditionalAttribute = m.GetAttribute<InspectableIfAttribute>();

            if (conditionalAttribute != null)
            {
                return AttributeUtility.CheckCondition(metadata.valueType, metadata.value, conditionalAttribute.conditionMember, false);
            }

            return false;
        }

        private readonly List<string> inspectedMemberNames = new List<string>();

        protected IEnumerable<MemberMetadata> inspectedMembers
        {
            get
            {
                return inspectedMemberNames.Select(name => metadata.Member(name, bindingFlags));
            }
        }

        public virtual void ReflectMetadata()
        {
            var adaptiveWidthAttribute = metadata.valueType.GetAttribute<InspectorAdaptiveWidthAttribute>();
            _adaptiveWidth = adaptiveWidthAttribute?.width ?? 200;

            inspectedMemberNames.Clear();

            inspectedMemberNames.AddRange(metadata.valueType
                .GetMembers(bindingFlags)
                .Where(Include)
                .Select(mi => mi.ToManipulator())
                .Where(m => m.isAccessor)
                .OrderBy(m => m.info.GetAttribute<InspectableAttribute>()?.order ?? int.MaxValue)
                .ThenBy(m => m.info.MetadataToken)
                .Select(m => m.name));

            SetHeightDirty();
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            foreach (var member in inspectedMembers)
            {
                height += GetMemberHeight(member, width);
                height += Styles.spaceBetweenMembers;
            }

            if (inspectedMembers.Any())
            {
                height -= Styles.spaceBetweenMembers;
            }

            return height;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            BeginLabeledBlock(metadata, position, label);

            foreach (var member in inspectedMembers)
            {
                var memberPosition = position.VerticalSection(ref y, GetMemberHeight(member, position.width));

                OnMemberGUI(member, memberPosition);

                y += Styles.spaceBetweenMembers;
            }

            EndBlock(metadata);
        }

        protected virtual float GetMemberHeight(Metadata member, float width)
        {
            return LudiqGUI.GetInspectorHeight(this, member, width);
        }

        protected virtual void OnMemberGUI(Metadata member, Rect memberPosition)
        {
            LudiqGUI.Inspector(member, memberPosition);
        }

        public override float GetAdaptiveWidth()
        {
            return _adaptiveWidth;
        }

        public static class Styles
        {
            public static readonly float spaceBetweenMembers = EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
