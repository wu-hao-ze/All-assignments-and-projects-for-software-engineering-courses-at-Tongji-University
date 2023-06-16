using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(IUnit))]
    public class UnitOption<TUnit> : IUnitOption where TUnit : IUnit
    {
        public UnitOption()
        {
            sourceScriptGuids = new HashSet<string>();
        }

        public UnitOption(TUnit unit) : this()
        {
            this.unit = unit;

            FillFromUnit();
        }

        [DoNotSerialize]
        protected bool filled { get; private set; }

        private TUnit _unit;

        protected UnitOptionRow source { get; private set; }

        public TUnit unit
        {
            get
            {
                // Load the node on demand to avoid deserialization overhead
                // Deserializing the entire database takes many seconds,
                // which is the reason why UnitOptionRow and SQLite are used
                // in the first place.

                if (_unit == null)
                {
                    _unit = (TUnit)new SerializationData(source.unit).Deserialize();
                }

                return _unit;
            }
            protected set
            {
                _unit = value;
            }
        }

        IUnit IUnitOption.unit => unit;

        public Type unitType { get; private set; }

        protected IUnitDescriptor descriptor => unit.Descriptor<IUnitDescriptor>();

        // Avoid using the descriptions for each option, because we don't need all fields described until the option is hovered

        protected UnitDescription description => unit.Description<UnitDescription>();

        protected UnitPortDescription PortDescription(IUnitPort port)
        {
            return port.Description<UnitPortDescription>();
        }

        public virtual IUnit InstantiateUnit()
        {
            var instance = unit.CloneViaSerialization();
            instance.Define();
            return instance;
        }

        void IUnitOption.PreconfigureUnit(IUnit unit)
        {
            PreconfigureUnit((TUnit)unit);
        }

        public virtual void PreconfigureUnit(TUnit unit)
        {
        }

        protected virtual void FillFromUnit()
        {
            unit.EnsureDefined();
            unitType = unit.GetType();

            labelHuman = Label(true);
            haystackHuman = Haystack(true);

            labelProgrammer = Label(false);
            haystackProgrammer = Haystack(false);

            category = Category();
            order = Order();
            favoriteKey = FavoriteKey();
            UnityAPI.Async(() => icon = Icon());

            showControlInputsInFooter = ShowControlInputsInFooter();
            showControlOutputsInFooter = ShowControlOutputsInFooter();
            showValueInputsInFooter = ShowValueInputsInFooter();
            showValueOutputsInFooter = ShowValueOutputsInFooter();

            controlInputCount = unit.controlInputs.Count;
            controlOutputCount = unit.controlOutputs.Count;
            valueInputTypes = unit.valueInputs.Select(vi => vi.type).ToHashSet();
            valueOutputTypes = unit.valueOutputs.Select(vo => vo.type).ToHashSet();

            filled = true;
        }

        protected virtual void FillFromData()
        {
            unit.EnsureDefined();
            unitType = unit.GetType();
            UnityAPI.Async(() => icon = Icon());

            showControlInputsInFooter = ShowControlInputsInFooter();
            showControlOutputsInFooter = ShowControlOutputsInFooter();
            showValueInputsInFooter = ShowValueInputsInFooter();
            showValueOutputsInFooter = ShowValueOutputsInFooter();

            filled = true;
        }

        public virtual void Deserialize(UnitOptionRow row)
        {
            source = row;

            if (row.sourceScriptGuids != null)
            {
                sourceScriptGuids = row.sourceScriptGuids.Split(',').ToHashSet();
            }

            unitType = Codebase.DeserializeType(row.unitType);

            category = row.category == null ? null : new UnitCategory(row.category);
            labelHuman = row.labelHuman;
            labelProgrammer = row.labelProgrammer;
            order = row.order;
            haystackHuman = row.haystackHuman;
            haystackProgrammer = row.haystackProgrammer;
            favoriteKey = row.favoriteKey;

            controlInputCount = row.controlInputCount;
            controlOutputCount = row.controlOutputCount;
        }

        public virtual UnitOptionRow Serialize()
        {
            var row = new UnitOptionRow();

            if (sourceScriptGuids.Count == 0)
            {
                // Important to set to null here, because the code relies on
                // null checks, not empty string checks.
                row.sourceScriptGuids = null;
            }
            else
            {
                row.sourceScriptGuids = string.Join(",", sourceScriptGuids.ToArray());
            }

            row.optionType = Codebase.SerializeType(GetType());
            row.unitType = Codebase.SerializeType(unitType);
            row.unit = unit.Serialize().json;

            row.category = category?.fullName;
            row.labelHuman = labelHuman;
            row.labelProgrammer = labelProgrammer;
            row.order = order;
            row.haystackHuman = haystackHuman;
            row.haystackProgrammer = haystackProgrammer;
            row.favoriteKey = favoriteKey;

            row.controlInputCount = controlInputCount;
            row.controlOutputCount = controlOutputCount;
            row.valueInputTypes = valueInputTypes.Select(Codebase.SerializeType).ToSeparatedString("|").NullIfEmpty();
            row.valueOutputTypes = valueOutputTypes.Select(Codebase.SerializeType).ToSeparatedString("|").NullIfEmpty();

            return row;
        }

        public virtual void OnPopulate()
        {
            if (!filled)
            {
                FillFromData();
            }
        }

        public virtual void Prewarm() { }


        #region Configuration

        public object value => this;

        public bool parentOnly => false;

        public virtual string headerLabel => label;

        public virtual bool showHeaderIcon => false;

        public virtual bool favoritable => true;

        #endregion


        #region Properties

        public HashSet<string> sourceScriptGuids { get; protected set; }

        protected string labelHuman { get; set; }

        protected string labelProgrammer { get; set; }

        public string label => BoltCore.Configuration.humanNaming ? labelHuman : labelProgrammer;

        public UnitCategory category { get; private set; }

        public int order { get; private set; }

        public EditorTexture icon { get; private set; }

        protected string haystackHuman { get; set; }

        protected string haystackProgrammer { get; set; }

        public string haystack => BoltCore.Configuration.humanNaming ? haystackHuman : haystackProgrammer;

        public string favoriteKey { get; private set; }

        public virtual string formerHaystack => BoltFlowNameUtility.UnitPreviousTitle(unitType);

        GUIStyle IFuzzyOption.style => Style();

        #endregion


        #region Contextual Filtering

        public int controlInputCount { get; private set; }

        public int controlOutputCount { get; private set; }

        private HashSet<Type> _valueInputTypes;

        private HashSet<Type> _valueOutputTypes;

        // On demand loading for initialization performance (type deserialization is expensive)

        public HashSet<Type> valueInputTypes
        {
            get
            {
                if (_valueInputTypes == null)
                {
                    if (string.IsNullOrEmpty(source.valueInputTypes))
                    {
                        _valueInputTypes = new HashSet<Type>();
                    }
                    else
                    {
                        _valueInputTypes = source.valueInputTypes.Split('|').Select(Codebase.DeserializeType).ToHashSet();
                    }
                }

                return _valueInputTypes;
            }
            private set
            {
                _valueInputTypes = value;
            }
        }

        public HashSet<Type> valueOutputTypes
        {
            get
            {
                if (_valueOutputTypes == null)
                {
                    if (string.IsNullOrEmpty(source.valueOutputTypes))
                    {
                        _valueOutputTypes = new HashSet<Type>();
                    }
                    else
                    {
                        _valueOutputTypes = source.valueOutputTypes.Split('|').Select(Codebase.DeserializeType).ToHashSet();
                    }
                }

                return _valueOutputTypes;
            }
            private set
            {
                _valueOutputTypes = value;
            }
        }

        #endregion


        #region Providers

        protected virtual string Label(bool human)
        {
            return BoltFlowNameUtility.UnitTitle(unitType, false, true);
        }

        protected virtual UnitCategory Category()
        {
            return unitType.GetAttribute<UnitCategory>();
        }

        protected virtual int Order()
        {
            return unitType.GetAttribute<UnitOrderAttribute>()?.order ?? int.MaxValue;
        }

        protected virtual string Haystack(bool human)
        {
            return Label(human);
        }

        protected virtual EditorTexture Icon()
        {
            return descriptor.Icon();
        }

        protected virtual GUIStyle Style()
        {
            return FuzzyWindow.defaultOptionStyle;
        }

        protected virtual string FavoriteKey()
        {
            return unit.GetType().FullName;
        }

        #endregion


        #region Search

        public virtual string SearchResultLabel(string query)
        {
            var label = SearchUtility.HighlightQuery(haystack, query);

            if (category != null)
            {
                label += $" <color=#{ColorPalette.unityForegroundDim.ToHexString()}>(in {category.fullName})</color>";
            }

            return label;
        }

        #endregion


        #region Footer

        private string summary => description.summary;

        public bool hasFooter => !StringUtility.IsNullOrWhiteSpace(summary) || footerPorts.Any();

        protected virtual bool ShowControlInputsInFooter()
        {
            return unitType.GetAttribute<UnitFooterPortsAttribute>()?.ControlInputs ?? false;
        }

        protected virtual bool ShowControlOutputsInFooter()
        {
            return unitType.GetAttribute<UnitFooterPortsAttribute>()?.ControlOutputs ?? false;
        }

        protected virtual bool ShowValueInputsInFooter()
        {
            return unitType.GetAttribute<UnitFooterPortsAttribute>()?.ValueInputs ?? true;
        }

        protected virtual bool ShowValueOutputsInFooter()
        {
            return unitType.GetAttribute<UnitFooterPortsAttribute>()?.ValueOutputs ?? true;
        }

        [DoNotSerialize]
        protected bool showControlInputsInFooter { get; private set; }

        [DoNotSerialize]
        protected bool showControlOutputsInFooter { get; private set; }

        [DoNotSerialize]
        protected bool showValueInputsInFooter { get; private set; }

        [DoNotSerialize]
        protected bool showValueOutputsInFooter { get; private set; }

        private IEnumerable<IUnitPort> footerPorts
        {
            get
            {
                if (showControlInputsInFooter)
                {
                    foreach (var controlInput in unit.controlInputs)
                    {
                        yield return controlInput;
                    }
                }

                if (showControlOutputsInFooter)
                {
                    foreach (var controlOutput in unit.controlOutputs)
                    {
                        yield return controlOutput;
                    }
                }

                if (showValueInputsInFooter)
                {
                    foreach (var valueInput in unit.valueInputs)
                    {
                        yield return valueInput;
                    }
                }

                if (showValueOutputsInFooter)
                {
                    foreach (var valueOutput in unit.valueOutputs)
                    {
                        yield return valueOutput;
                    }
                }
            }
        }

        public float GetFooterHeight(float width)
        {
            var hasSummary = !StringUtility.IsNullOrWhiteSpace(summary);
            var hasIcon = icon != null;
            var hasPorts = footerPorts.Any();

            var height = 0f;

            width -= 2 * FooterStyles.padding;

            height += FooterStyles.padding;

            if (hasSummary)
            {
                if (hasIcon)
                {
                    height += Mathf.Max(FooterStyles.unitIconSize, GetFooterSummaryHeight(width - FooterStyles.unitIconSize - FooterStyles.spaceAfterUnitIcon));
                }
                else
                {
                    height += GetFooterSummaryHeight(width);
                }
            }

            if (hasSummary && hasPorts)
            {
                height += FooterStyles.spaceBetweenDescriptionAndPorts;
            }

            foreach (var port in footerPorts)
            {
                height += GetFooterPortHeight(width, port);
                height += FooterStyles.spaceBetweenPorts;
            }

            if (hasPorts)
            {
                height -= FooterStyles.spaceBetweenPorts;
            }

            height += FooterStyles.padding;

            return height;
        }

        public void OnFooterGUI(Rect position)
        {
            var hasSummary = !StringUtility.IsNullOrWhiteSpace(summary);
            var hasIcon = icon != null;
            var hasPorts = footerPorts.Any();

            var y = position.y;

            y += FooterStyles.padding;

            position.x += FooterStyles.padding;
            position.width -= FooterStyles.padding * 2;

            if (hasSummary)
            {
                if (hasIcon)
                {
                    var iconPosition = new Rect
                        (
                        position.x,
                        y,
                        FooterStyles.unitIconSize,
                        FooterStyles.unitIconSize
                        );

                    var summaryWidth = position.width - iconPosition.width - FooterStyles.spaceAfterUnitIcon;

                    var summaryPosition = new Rect
                        (
                        iconPosition.xMax + FooterStyles.spaceAfterUnitIcon,
                        y,
                        summaryWidth,
                        GetFooterSummaryHeight(summaryWidth)
                        );

                    GUI.DrawTexture(iconPosition, icon?[FooterStyles.unitIconSize]);

                    OnFooterSummaryGUI(summaryPosition);

                    y = Mathf.Max(iconPosition.yMax, summaryPosition.yMax);
                }
                else
                {
                    OnFooterSummaryGUI(position.VerticalSection(ref y, GetFooterSummaryHeight(position.width)));
                }
            }

            if (hasSummary && hasPorts)
            {
                y += FooterStyles.spaceBetweenDescriptionAndPorts;
            }

            foreach (var port in footerPorts)
            {
                OnFooterPortGUI(position.VerticalSection(ref y, GetFooterPortHeight(position.width, port)), port);
                y += FooterStyles.spaceBetweenPorts;
            }

            if (hasPorts)
            {
                y -= FooterStyles.spaceBetweenPorts;
            }

            y += FooterStyles.padding;
        }

        private float GetFooterSummaryHeight(float width)
        {
            return FooterStyles.description.CalcHeight(new GUIContent(summary), width);
        }

        private void OnFooterSummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, summary, FooterStyles.description);
        }

        private string GetFooterPortLabel(IUnitPort port)
        {
            string type;

            if (port is ValueInput)
            {
                type = ((IUnitValuePort)port).type.DisplayName() + " Input";
            }
            else if (port is ValueOutput)
            {
                type = ((IUnitValuePort)port).type.DisplayName() + " Output";
            }
            else if (port is ControlInput)
            {
                type = "Trigger Input";
            }
            else if (port is ControlOutput)
            {
                type = "Trigger Output";
            }
            else
            {
                throw new NotSupportedException();
            }

            var portDescription = PortDescription(port);

            if (!StringUtility.IsNullOrWhiteSpace(portDescription.summary))
            {
                return $"<b>{portDescription.label}:</b> {portDescription.summary} {LudiqGUIUtility.DimString($"({type})")}";
            }
            else
            {
                return $"<b>{portDescription.label}:</b> {LudiqGUIUtility.DimString($"({type})")}";
            }
        }

        private float GetFooterPortDescriptionHeight(float width, IUnitPort port)
        {
            return FooterStyles.portDescription.CalcHeight(new GUIContent(GetFooterPortLabel(port)), width);
        }

        private void OnFooterPortDescriptionGUI(Rect position, IUnitPort port)
        {
            GUI.Label(position, GetFooterPortLabel(port), FooterStyles.portDescription);
        }

        private float GetFooterPortHeight(float width, IUnitPort port)
        {
            var descriptionWidth = width - FooterStyles.portIconSize - FooterStyles.spaceAfterPortIcon;

            return GetFooterPortDescriptionHeight(descriptionWidth, port);
        }

        private void OnFooterPortGUI(Rect position, IUnitPort port)
        {
            var iconPosition = new Rect
                (
                position.x,
                position.y,
                FooterStyles.portIconSize,
                FooterStyles.portIconSize
                );

            var descriptionWidth = position.width - FooterStyles.portIconSize - FooterStyles.spaceAfterPortIcon;

            var descriptionPosition = new Rect
                (
                iconPosition.xMax + FooterStyles.spaceAfterPortIcon,
                position.y,
                descriptionWidth,
                GetFooterPortDescriptionHeight(descriptionWidth, port)
                );

            var portDescription = PortDescription(port);

            var icon = portDescription.icon?[FooterStyles.portIconSize];

            if (icon != null)
            {
                GUI.DrawTexture(iconPosition, icon);
            }

            OnFooterPortDescriptionGUI(descriptionPosition, port);
        }

        public static class FooterStyles
        {
            static FooterStyles()
            {
                description = new GUIStyle(EditorStyles.label);
                description.padding = new RectOffset(0, 0, 0, 0);
                description.wordWrap = true;
                description.richText = true;

                portDescription = new GUIStyle(EditorStyles.label);
                portDescription.padding = new RectOffset(0, 0, 0, 0);
                portDescription.wordWrap = true;
                portDescription.richText = true;
                portDescription.imagePosition = ImagePosition.TextOnly;
            }

            public static readonly GUIStyle description;
            public static readonly GUIStyle portDescription;
            public static readonly float spaceAfterUnitIcon = 7;
            public static readonly int unitIconSize = IconSize.Medium;
            public static readonly float spaceAfterPortIcon = 6;
            public static readonly int portIconSize = IconSize.Small;
            public static readonly float spaceBetweenDescriptionAndPorts = 8;
            public static readonly float spaceBetweenPorts = 8;
            public static readonly float padding = 8;
        }

        #endregion
    }
}
