/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Inspector(typeof(MemberInvocation))]
    public sealed class MemberInvocationInspector : InvocationInspector
    {
        public MemberInvocationInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            memberInspector = memberMetadata.Inspector<MemberManipulatorInspector>();
            memberInspector.direction = (ActionDirection)directionMetadata.value;

            referenceInspectors = new Dictionary<Metadata, Inspector>();
            memberFilter = metadata.GetAttribute<MemberFilter>() ?? MemberFilter.Any;
            memberTypeFilter = metadata.GetAttribute<TypeFilter>();
            memberMetadata.valueChanged += previousMember => ReflectMember(false);
        }

        private MemberManipulatorInspector memberInspector;

        private Dictionary<Metadata, Inspector> referenceInspectors;

        private IFuzzyOptionTree GetMemberOptions()
        {
            return new MemberOptionTree(typeSet, memberFilter, memberTypeFilter, (ActionDirection)directionMetadata.value);
        }

        #region Metadata

        private Metadata memberMetadata => metadata[nameof(MemberInvocation.member)];

        private Metadata directionMetadata => metadata[nameof(MemberInvocation.direction)];

        private Metadata targetMetadata => metadata[nameof(MemberInvocation.target)];

        #endregion

        #region Settings

        private ReadOnlyCollection<Type> _typeSet;

        private MemberFilter _memberFilter;

        private TypeFilter _memberTypeFilter;

        public override Type expectedType
        {
            get
            {
                return base.expectedType;
            }
            set
            {
                base.expectedType = value;
                memberTypeFilter = value != null ? new TypeFilter(TypesMatching.Any, value) : null;
            }
        }

        public ReadOnlyCollection<Type> typeSet
        {
            get
            {
                return _typeSet;
            }
            set
            {
                _typeSet = value;
                memberInspector.typeSet = value;
            }
        }

        public MemberFilter memberFilter
        {
            get
            {
                return _memberFilter;
            }
            set
            {
                _memberFilter = value;
                memberInspector.memberFilter = value;
            }
        }

        public TypeFilter memberTypeFilter
        {
            get
            {
                return _memberTypeFilter;
            }
            set
            {
                _memberTypeFilter = value;
                memberInspector.memberTypeFilter = value;
            }
        }

        #endregion

        #region Reflection

        private bool reflectionSucceeded;

        private void ReflectMember(bool reset)
        {
            memberMetadata.DisposeChildren();

            var member = (Member)memberMetadata.value;

            // Clear the metadata tree and optionally the arguments

            if (reset)
            {
                targetMetadata.value = null;
                argumentsMetadata.Clear();
            }

            referenceInspectors.Clear();

            // Attempt to reflect the member

            reflectionSucceeded = false;

            if (member != null)
            {
                try
                {
                    member.EnsureReflected();
                    reflectionSucceeded = true;
                }
                catch { }
            }

            if (!reflectionSucceeded)
            {
                return;
            }

            // Create the metadata tree and optionally assign default arguments

            if (reset)
            {
                IExpression target;

                if (member.requiresTarget)
                {
                    if (ComponentHolderProtocol.IsComponentHolderType(member.targetType) && metadata.UnityObjectAncestor() != null)
                    {
                        target = new Self();
                    }
                    else
                    {
                        target = null;
                    }
                }
                else
                {
                    target = null;
                }

                targetMetadata.value = target;
            }

            targetMetadata.Inspector<IExpressionInspector>().expectedType = member.targetType;

            if (member.isInvocable)
            {
                var argumentIndex = 0;

                foreach (var parameterInfo in member.methodBase.GetParametersWithoutThis())
                {
                    if (reset || argumentIndex >= argumentsMetadata.Count)
                    {
                        argumentsMetadata.Add(parameterInfo.DefaultInspectableExpression());
                    }

                    var argumentMetadata = argumentsMetadata[argumentIndex];

                    PrepareParameterLabel(argumentMetadata, parameterInfo.HumanName(), member.methodBase.ParameterSummary(parameterInfo));

                    if (parameterInfo.ParameterType.IsByRef)
                    {
                        var argument = argumentsMetadata[argumentIndex].value as Literal;

                        if (argument == null || argument.type != typeof(IVariableReference))
                        {
                            argumentsMetadata[argumentIndex].value = new Literal(typeof(IVariableReference), null);
                        }

                        var referenceInspector = new IVariableReferenceInspector(argumentMetadata[nameof(ILiteral.value)]);
                        referenceInspector.Initialize();
                        referenceInspector.direction = ActionDirection.Set;
                        referenceInspectors.Add(argumentMetadata, referenceInspector);
                        PrepareParameterLabel(argumentMetadata[nameof(ILiteral.value)], parameterInfo.HumanName(), member.methodBase.ParameterSummary(parameterInfo));
                    }
                    else
                    {
                        PrepareParameterInspector(argumentMetadata, parameterInfo.ParameterType);
                    }

                    argumentIndex++;
                }
            }
            else if (member.isSettable && (ActionDirection)directionMetadata.value != ActionDirection.Get)
            {
                if (reset || argumentsMetadata.Count == 0)
                {
                    argumentsMetadata.Add(member.type.ToInspectableExpression());
                }

                var argumentMetadata = argumentsMetadata[0];

                PrepareParameterLabel(argumentMetadata, member.info.HumanName(), member.info.Summary());
                PrepareParameterInspector(argumentMetadata, member.type);
            }

            SetHeightDirty();
        }

        public static bool WillFail(bool reflectionSucceeded, MemberInvocation invocation, UnityObject owner)
        {
            if (!reflectionSucceeded)
            {
                return true;
            }

            // We can only analyze if the member isn't null and
            // if we can infer the value of the target expression in edit mode
            var canAnalyze = invocation.member != null && (invocation.target is ILiteral || invocation.target is Self);

            if (!canAnalyze)
            {
                return false;
            }

            object target;

            if (invocation.target is ILiteral)
            {
                target = ((ILiteral)invocation.target).value;
            }
            else if (invocation.target is Self)
            {
                target = owner;
            }
            else
            {
                throw new NotSupportedException();
            }

            if (target == null)
            {
                return true;
            }

            var targetType = target.GetType();

            if (ComponentHolderProtocol.IsComponentHolderType(invocation.member.targetType))
            {
                if (!ComponentHolderProtocol.IsComponentHolderType(targetType))
                {
                    return true;
                }

                // Don't fail true if the owner isn't a component holder
                // (e.g. it might be a graph asset)
                if (!owner.IsComponentHolder())
                {
                    return false;
                }

                return invocation.member.targetType != typeof(GameObject) && !((UnityObject)target).GetComponents<Component>().Any(c => invocation.member.targetType.IsInstanceOfType(c));
            }
            else
            {
                return !invocation.member.targetType.IsAssignableFrom(targetType);
            }
        }

        private bool willFail => WillFail(reflectionSucceeded, (MemberInvocation)metadata.value, metadata.UnityObjectAncestor());

        #endregion

        #region Rendering

        protected override IEnumerable<GUIContent> compactLabels => base.compactLabels.Concat(targetMetadata.label.Yield());

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            height += GetMemberHeight(width);

            using (LudiqGUIUtility.labelWidth.Override(GetCompactLabelsWidth(width)))
            {
                if (reflectionSucceeded)
                {
                    height += Styles.spaceBetweenParameters;

                    if (((Member)memberMetadata.value).requiresTarget)
                    {
                        height += GetTargetHeight(width);

                        if (parameters.Count > 0)
                        {
                            height += Styles.spaceBetweenParameters;
                        }
                    }

                    height += GetParametersHeight(width);
                }
            }

            height = HeightWithLabel(metadata, width, height, label);

            return height;
        }

        private float GetMemberHeight(float width)
        {
            return InspectorGUI.GetHeight(memberMetadata, width, GUIContent.none, this);
        }

        private float GetTargetHeight(float width)
        {
            return InspectorGUI.GetHeight(targetMetadata, width, targetMetadata.label, this);
        }

        protected override float GetParameterHeight(Metadata parameter, float width)
        {
            if (referenceInspectors.ContainsKey(parameter))
            {
                return referenceInspectors[parameter].GetHeight(width, GUIContent.none, this);
            }
            else
            {
                return InspectorGUI.GetHeight(parameter, width, GUIContent.none, this);
            }
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            var memberPosition = position.VerticalSection(ref y, GetMemberHeight(position.width));

            memberPosition = PrefixLabel(metadata, memberPosition, label);

            OnMemberGUI(memberPosition);

            position = ReclaimImplementationSelector(position);

            if (reflectionSucceeded)
            {
                using (LudiqGUIUtility.labelWidth.Override(GetCompactLabelsWidth(position.width)))
                {
                    y += Styles.spaceBetweenParameters;

                    if (((Member)memberMetadata.value).requiresTarget)
                    {
                        var targetPosition = position.VerticalSection(ref y, GetTargetHeight(position.width));

                        OnTargetGUI(targetPosition);

                        if (parameters.Count > 0)
                        {
                            y += Styles.spaceBetweenParameters;
                        }
                    }

                    OnParametersGUI(position);
                }
            }
        }

        private void OnMemberGUI(Rect memberPosition)
        {
            BeginBlock(memberMetadata, memberPosition, GUIContent.none);

            memberMetadata.Inspector<MemberManipulatorInspector>().willFail = willFail;

            InspectorGUI.Field(memberMetadata, memberPosition, GUIContent.none);

            if (EndBlock(memberMetadata))
            {
                ReflectMember(true);
            }
        }

        private void OnTargetGUI(Rect targetPosition)
        {
            InspectorGUI.Field(targetMetadata, targetPosition);
        }

        protected override void OnParameterGUI(Rect parameterPosition, Metadata parameter)
        {
            if (referenceInspectors.ContainsKey(parameter))
            {
                referenceInspectors[parameter].Field(parameterPosition);
            }
            else
            {
                base.OnParameterGUI(parameterPosition, parameter);
            }
        }

        #endregion
    }
}
*/
