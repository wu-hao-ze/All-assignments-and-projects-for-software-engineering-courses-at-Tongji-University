using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(IUnit))]
    public class UnitDescriptor<TUnit> : Descriptor<TUnit, UnitDescription>, IUnitDescriptor
        where TUnit : class, IUnit
    {
        public UnitDescriptor(TUnit target) : base(target)
        {
            unitType = unit.GetType();
        }

        protected Type unitType { get; }

        public TUnit unit => target;

        IUnit IUnitDescriptor.unit => unit;

        private enum State
        {
            Defined,

            NotDefined,

            FailedToDefine
        }

        private State state
        {
            get
            {
                if (unit.isDefined)
                {
                    return State.Defined;
                }
                else if (unit.failedToDefine)
                {
                    return State.FailedToDefine;
                }
                else
                {
                    return State.NotDefined;
                }
            }
        }


        #region Reflected Description

        static UnitDescriptor()
        {
            XmlDocumentation.loadComplete += FreeReflectedDescriptions;
        }

        public static void FreeReflectedDescriptions()
        {
            reflectedDescriptions.Clear();
            reflectedInputDescriptions.Clear();
            reflectedOutputDescriptions.Clear();
        }

        protected UnitDescription reflectedDescription
        {
            get
            {
                if (!reflectedDescriptions.TryGetValue(unitType, out var reflectedDescription))
                {
                    reflectedDescription = FetchReflectedDescription(unitType);
                    reflectedDescriptions.Add(unitType, reflectedDescription);
                }

                return reflectedDescription;
            }
        }

        protected UnitPortDescription ReflectedPortDescription(IUnitPort port)
        {
            if (port is IUnitInvalidPort)
            {
                return null;
            }

            if (port is IUnitInputPort)
            {
                if (!reflectedInputDescriptions.TryGetValue(unitType, out var _reflectedInputDescriptions))
                {
                    _reflectedInputDescriptions = FetchReflectedPortDescriptions<IUnitInputPort>(unitType);
                    reflectedInputDescriptions.Add(unitType, _reflectedInputDescriptions);
                }

                if (_reflectedInputDescriptions.TryGetValue(port.key, out var portDescription))
                {
                    return portDescription;
                }
            }
            else if (port is IUnitOutputPort)
            {
                if (!reflectedOutputDescriptions.TryGetValue(unitType, out var _reflectedOutputDescriptions))
                {
                    _reflectedOutputDescriptions = FetchReflectedPortDescriptions<IUnitOutputPort>(unitType);
                    reflectedOutputDescriptions.Add(unitType, _reflectedOutputDescriptions);
                }

                if (_reflectedOutputDescriptions.TryGetValue(port.key, out var portDescription))
                {
                    return portDescription;
                }
            }

            return null;
        }

        private static readonly Dictionary<Type, UnitDescription> reflectedDescriptions = new Dictionary<Type, UnitDescription>();

        private static readonly Dictionary<Type, Dictionary<string, UnitPortDescription>> reflectedInputDescriptions = new Dictionary<Type, Dictionary<string, UnitPortDescription>>();

        private static readonly Dictionary<Type, Dictionary<string, UnitPortDescription>> reflectedOutputDescriptions = new Dictionary<Type, Dictionary<string, UnitPortDescription>>();

        private static UnitDescription FetchReflectedDescription(Type unitType)
        {
            var oldName = BoltFlowNameUtility.UnitPreviousTitle(unitType);
            var prefix = string.IsNullOrEmpty(oldName) ? string.Empty : $"(Previously named {oldName}) ";

            return new UnitDescription()
            {
                title = BoltFlowNameUtility.UnitTitle(unitType, false, true),
                shortTitle = BoltFlowNameUtility.UnitTitle(unitType, true, true),
                surtitle = unitType.GetAttribute<UnitSurtitleAttribute>()?.surtitle,
                subtitle = unitType.GetAttribute<UnitSubtitleAttribute>()?.subtitle,
                summary = prefix + unitType.Summary()
            };
        }

        private static Dictionary<string, UnitPortDescription> FetchReflectedPortDescriptions<T>(Type unitType) where T : IUnitPort
        {
            var descriptions = new Dictionary<string, UnitPortDescription>();

            foreach (var portMember in unitType.GetMembers().Where(member => typeof(T).IsAssignableFrom(member.GetAccessorType())))
            {
                var key = portMember.GetAttribute<PortKeyAttribute>()?.key ?? portMember.Name;

                if (descriptions.ContainsKey(key))
                {
                    Debug.LogWarning("Duplicate reflected port description for: " + key);

                    continue;
                }

                descriptions.Add(key, FetchReflectedPortDescription(portMember));
            }

            return descriptions;
        }

        private static UnitPortDescription FetchReflectedPortDescription(MemberInfo portMember)
        {
            return new UnitPortDescription()
            {
                label = portMember.GetAttribute<PortLabelAttribute>()?.label ?? portMember.HumanName(),
                showLabel = !(portMember.HasAttribute<PortLabelHiddenAttribute>() || (portMember.GetAttribute<PortLabelAttribute>()?.hidden ?? false)),
                summary = portMember.Summary(),
                getMetadata = (unitMetadata) => unitMetadata[portMember.Name]
            };
        }

        #endregion


        #region Description

        [Assigns]
        public sealed override string Title()
        {
            switch (state)
            {
                case State.Defined: return DefinedTitle();
                case State.NotDefined: return DefaultTitle();
                case State.FailedToDefine: return ErrorTitle(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        [Assigns]
        public string ShortTitle()
        {
            switch (state)
            {
                case State.Defined: return DefinedShortTitle();
                case State.NotDefined: return DefaultShortTitle();
                case State.FailedToDefine: return ErrorShortTitle(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        [Assigns]
        public string Surtitle()
        {
            switch (state)
            {
                case State.Defined: return DefinedSurtitle();
                case State.NotDefined: return DefaultSurtitle();
                case State.FailedToDefine: return ErrorSurtitle(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        [Assigns]
        public string Subtitle()
        {
            switch (state)
            {
                case State.Defined: return DefinedSubtitle();
                case State.NotDefined: return DefaultSubtitle();
                case State.FailedToDefine: return ErrorSubtitle(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        [Assigns]
        public sealed override string Summary()
        {
            switch (state)
            {
                case State.Defined: return DefinedSummary();
                case State.NotDefined: return DefaultSummary();
                case State.FailedToDefine: return ErrorSummary(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        [Assigns]
        [RequiresUnityAPI]
        public sealed override EditorTexture Icon()
        {
            switch (state)
            {
                case State.Defined: return DefinedIcon();
                case State.NotDefined: return DefaultIcon();
                case State.FailedToDefine: return ErrorIcon(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        [Assigns]
        [RequiresUnityAPI]
        public IEnumerable<EditorTexture> Icons()
        {
            switch (state)
            {
                case State.Defined: return DefinedIcons();
                case State.NotDefined: return DefaultIcons();
                case State.FailedToDefine: return ErrorIcons(unit.definitionException);
                default: throw new UnexpectedEnumValueException<State>(state);
            }
        }

        protected virtual string DefinedTitle()
        {
            return reflectedDescription.title;
        }

        protected virtual string DefaultTitle()
        {
            return reflectedDescription.title;
        }

        protected virtual string ErrorTitle(Exception exception)
        {
            return reflectedDescription.title;
        }

        protected virtual string DefinedShortTitle()
        {
            return reflectedDescription.shortTitle;
        }

        protected virtual string DefaultShortTitle()
        {
            return reflectedDescription.shortTitle;
        }

        protected virtual string ErrorShortTitle(Exception exception)
        {
            return ErrorTitle(exception);
        }

        protected virtual string DefinedSurtitle()
        {
            return reflectedDescription.surtitle;
        }

        protected virtual string DefaultSurtitle()
        {
            return reflectedDescription.surtitle;
        }

        protected virtual string ErrorSurtitle(Exception exception)
        {
            return null;
        }

        protected virtual string DefinedSubtitle()
        {
            return reflectedDescription.subtitle;
        }

        protected virtual string DefaultSubtitle()
        {
            return reflectedDescription.subtitle;
        }

        protected virtual string ErrorSubtitle(Exception exception)
        {
            return null;
        }

        protected virtual string DefinedSummary()
        {
            return reflectedDescription.summary;
        }

        protected virtual string DefaultSummary()
        {
            return reflectedDescription.summary;
        }

        protected virtual string ErrorSummary(Exception exception)
        {
            return $"This node failed to define.\n\n{exception.DisplayName()}: {exception.Message}";
        }

        protected virtual EditorTexture DefinedIcon()
        {
            return unit.GetType().Icon();
        }

        protected virtual EditorTexture DefaultIcon()
        {
            return unit.GetType().Icon();
        }

        protected virtual EditorTexture ErrorIcon(Exception exception)
        {
            return BoltCore.Icons.errorState;
        }

        protected virtual IEnumerable<EditorTexture> DefinedIcons()
        {
            return Enumerable.Empty<EditorTexture>();
        }

        protected virtual IEnumerable<EditorTexture> DefaultIcons()
        {
            return Enumerable.Empty<EditorTexture>();
        }

        protected virtual IEnumerable<EditorTexture> ErrorIcons(Exception exception)
        {
            return Enumerable.Empty<EditorTexture>();
        }

        public void DescribePort(IUnitPort port, UnitPortDescription description)
        {
            description.getMetadata = (unitMetadata) => unitMetadata.StaticObject(port);

            // Only defined nodes can have specific ports
            if (state == State.Defined)
            {
                DefinedPort(port, description);
            }
        }

        protected virtual void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            var reflectedPortDescription = ReflectedPortDescription(port);

            if (reflectedPortDescription != null)
            {
                description.CopyFrom(reflectedPortDescription);
            }
        }

        #endregion
    }
}
