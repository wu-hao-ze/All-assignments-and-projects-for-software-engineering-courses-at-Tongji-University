using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(IUnit))]
    public class UnitWidget<TUnit> : NodeWidget<FlowCanvas, TUnit>, IUnitWidget where TUnit : class, IUnit
    {
        public UnitWidget(FlowCanvas canvas, TUnit unit) : base(canvas, unit)
        {
            unit.onPortsChanged += CacheDefinition;
            unit.onPortsChanged += SubWidgetsChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            unit.onPortsChanged -= CacheDefinition;
            unit.onPortsChanged -= SubWidgetsChanged;
        }

        public override IEnumerable<IWidget> subWidgets => unit.ports.Select(port => canvas.Widget(port));


        #region Model

        protected TUnit unit => element;

        IUnit IUnitWidget.unit => unit;

        protected IUnitDebugData unitDebugData => GetDebugData<IUnitDebugData>();

        private UnitDescription description;

        private UnitAnalysis analysis => unit.Analysis<UnitAnalysis>(context);

        protected readonly List<IUnitPortWidget> ports = new List<IUnitPortWidget>();

        protected readonly List<IUnitPortWidget> inputs = new List<IUnitPortWidget>();

        protected readonly List<IUnitPortWidget> outputs = new List<IUnitPortWidget>();

        private readonly List<string> settingNames = new List<string>();

        protected IEnumerable<Metadata> settings
        {
            get
            {
                foreach (var settingName in settingNames)
                {
                    yield return metadata[settingName];
                }
            }
        }

        protected override void CacheItemFirstTime()
        {
            base.CacheItemFirstTime();
            CacheDefinition();
        }

        protected virtual void CacheDefinition()
        {
            inputs.Clear();
            outputs.Clear();
            ports.Clear();
            inputs.AddRange(unit.inputs.Select(port => canvas.Widget<IUnitPortWidget>(port)));
            outputs.AddRange(unit.outputs.Select(port => canvas.Widget<IUnitPortWidget>(port)));
            ports.AddRange(inputs);
            ports.AddRange(outputs);

            Reposition();
        }

        protected override void CacheDescription()
        {
            description = unit.Description<UnitDescription>();

            titleContent.text = description.shortTitle;
            titleContent.tooltip = description.summary;
            surtitleContent.text = description.surtitle;
            subtitleContent.text = description.subtitle;

            Reposition();
        }

        protected override void CacheMetadata()
        {
            settingNames.Clear();

            settingNames.AddRange(metadata.valueType
                .GetMembers()
                .Where(mi => mi.HasAttribute<UnitHeaderInspectableAttribute>())
                .OrderBy(mi => mi.GetAttributes<Attribute>().OfType<IInspectableAttribute>().FirstOrDefault()?.order ?? int.MaxValue)
                .ThenBy(mi => mi.MetadataToken)
                .Select(mi => mi.Name));

            lock (settingLabelsContents)
            {
                settingLabelsContents.Clear();

                foreach (var setting in settings)
                {
                    var settingLabel = setting.GetAttribute<UnitHeaderInspectableAttribute>().label;

                    GUIContent settingContent;

                    if (string.IsNullOrEmpty(settingLabel))
                    {
                        settingContent = null;
                    }
                    else
                    {
                        settingContent = new GUIContent(settingLabel);
                    }

                    settingLabelsContents.Add(setting, settingContent);
                }
            }

            Reposition();
        }

        public virtual Inspector GetPortInspector(IUnitPort port, Metadata metadata)
        {
            return metadata.Inspector();
        }

        #endregion


        #region Lifecycle

        public override bool foregroundRequiresInput => showSettings || unit.valueInputs.Any(vip => vip.hasDefaultValue);

        public override void HandleInput()
        {
            if (canvas.isCreatingConnection)
            {
                if (e.IsMouseDown(MouseButton.Left))
                {
                    var source = canvas.connectionSource;
                    var destination = source.CompatiblePort(unit);

                    if (destination != null)
                    {
                        UndoUtility.RecordEditedObject("Connect Nodes");
                        source.ValidlyConnectTo(destination);
                        canvas.connectionSource = null;
                        canvas.Widget(source.unit).Reposition();
                        canvas.Widget(destination.unit).Reposition();
                        GUI.changed = true;
                    }

                    e.Use();
                }
                else if (e.IsMouseDown(MouseButton.Right))
                {
                    canvas.CancelConnection();
                    e.Use();
                }
            }

            base.HandleInput();
        }

        #endregion


        #region Contents

        protected readonly GUIContent titleContent = new GUIContent();

        protected readonly GUIContent surtitleContent = new GUIContent();

        protected readonly GUIContent subtitleContent = new GUIContent();

        protected readonly Dictionary<Metadata, GUIContent> settingLabelsContents = new Dictionary<Metadata, GUIContent>();

        #endregion


        #region Positioning

        protected override bool snapToGrid => BoltCore.Configuration.snapToGrid;

        public override IEnumerable<IWidget> positionDependers => ports.Cast<IWidget>();

        protected Rect _position;

        public override Rect position
        {
            get { return _position; }
            set { unit.position = value.position; }
        }

        public Rect titlePosition { get; private set; }

        public Rect surtitlePosition { get; private set; }

        public Rect subtitlePosition { get; private set; }

        public Rect iconPosition { get; private set; }

        public List<Rect> iconsPositions { get; private set; } = new List<Rect>();

        public Dictionary<Metadata, Rect> settingsPositions { get; } = new Dictionary<Metadata, Rect>();

        public Rect headerAddonPosition { get; private set; }

        public Rect portsBackgroundPosition { get; private set; }

        public override void CachePosition()
        {
            // Width

            var inputsWidth = 0f;
            var outputsWidth = 0f;

            foreach (var input in inputs)
            {
                inputsWidth = Mathf.Max(inputsWidth, input.GetInnerWidth());
            }

            foreach (var output in outputs)
            {
                outputsWidth = Mathf.Max(outputsWidth, output.GetInnerWidth());
            }

            var portsWidth = 0f;

            portsWidth += inputsWidth;
            portsWidth += Styles.spaceBetweenInputsAndOutputs;
            portsWidth += outputsWidth;

            settingsPositions.Clear();

            var settingsWidth = 0f;

            if (showSettings)
            {
                foreach (var setting in settings)
                {
                    var settingWidth = 0f;

                    var settingLabelContent = settingLabelsContents[setting];

                    if (settingLabelContent != null)
                    {
                        settingWidth += Styles.settingLabel.CalcSize(settingLabelContent).x;
                    }

                    settingWidth += setting.Inspector().GetAdaptiveWidth();

                    settingWidth = Mathf.Min(settingWidth, Styles.maxSettingsWidth);

                    settingsPositions.Add(setting, new Rect(0, 0, settingWidth, 0));

                    settingsWidth = Mathf.Max(settingsWidth, settingWidth);
                }
            }

            var headerAddonWidth = 0f;

            if (showHeaderAddon)
            {
                headerAddonWidth = GetHeaderAddonWidth();
            }

            var titleWidth = Styles.title.CalcSize(titleContent).x;

            var headerTextWidth = titleWidth;

            var surtitleWidth = 0f;

            if (showSurtitle)
            {
                surtitleWidth = Styles.surtitle.CalcSize(surtitleContent).x;
                headerTextWidth = Mathf.Max(headerTextWidth, surtitleWidth);
            }

            var subtitleWidth = 0f;

            if (showSubtitle)
            {
                subtitleWidth = Styles.subtitle.CalcSize(subtitleContent).x;
                headerTextWidth = Mathf.Max(headerTextWidth, subtitleWidth);
            }

            var iconsWidth = 0f;

            if (showIcons)
            {
                var iconsColumns = Mathf.Ceil((float)description.icons.Length / Styles.iconsPerColumn);
                iconsWidth = iconsColumns * Styles.iconsSize + ((iconsColumns - 1) * Styles.iconsSpacing);
            }

            var headerWidth = Mathf.Max(headerTextWidth + iconsWidth, Mathf.Max(settingsWidth, headerAddonWidth)) + Styles.iconSize + Styles.spaceAfterIcon;

            var innerWidth = Mathf.Max(portsWidth, headerWidth);

            var edgeWidth = InnerToEdgePosition(new Rect(0, 0, innerWidth, 0)).width;

            // Height & Positioning

            var edgeOrigin = unit.position;
            var edgeX = edgeOrigin.x;
            var edgeY = edgeOrigin.y;
            var innerOrigin = EdgeToInnerPosition(new Rect(edgeOrigin, Vector2.zero)).position;
            var innerX = innerOrigin.x;
            var innerY = innerOrigin.y;

            iconPosition = new Rect
                (
                innerX,
                innerY,
                Styles.iconSize,
                Styles.iconSize
                );

            var headerTextX = iconPosition.xMax + Styles.spaceAfterIcon;

            var y = innerY;

            var headerHeight = 0f;

            var surtitleHeight = 0f;

            if (showSurtitle)
            {
                surtitleHeight = Styles.surtitle.CalcHeight(surtitleContent, headerTextWidth);

                surtitlePosition = new Rect
                    (
                    headerTextX,
                    y,
                    headerTextWidth,
                    surtitleHeight
                    );

                headerHeight += surtitleHeight;
                y += surtitleHeight;

                headerHeight += Styles.spaceAfterSurtitle;
                y += Styles.spaceAfterSurtitle;
            }

            var titleHeight = 0f;

            if (showTitle)
            {
                titleHeight = Styles.title.CalcHeight(titleContent, headerTextWidth);

                titlePosition = new Rect
                    (
                    headerTextX,
                    y,
                    headerTextWidth,
                    titleHeight
                    );

                headerHeight += titleHeight;
                y += titleHeight;
            }

            var subtitleHeight = 0f;

            if (showSubtitle)
            {
                headerHeight += Styles.spaceBeforeSubtitle;
                y += Styles.spaceBeforeSubtitle;

                subtitleHeight = Styles.subtitle.CalcHeight(subtitleContent, headerTextWidth);

                subtitlePosition = new Rect
                    (
                    headerTextX,
                    y,
                    headerTextWidth,
                    subtitleHeight
                    );

                headerHeight += subtitleHeight;
                y += subtitleHeight;
            }

            iconsPositions.Clear();

            if (showIcons)
            {
                var iconRow = 0;
                var iconCol = 0;

                for (int i = 0; i < description.icons.Length; i++)
                {
                    var iconPosition = new Rect
                        (
                        innerX + innerWidth - ((iconCol + 1) * Styles.iconsSize) - ((iconCol) * Styles.iconsSpacing),
                        innerY + (iconRow * (Styles.iconsSize + Styles.iconsSpacing)),
                        Styles.iconsSize,
                        Styles.iconsSize
                        );

                    iconsPositions.Add(iconPosition);

                    iconRow++;

                    if (iconRow % Styles.iconsPerColumn == 0)
                    {
                        iconCol++;
                        iconRow = 0;
                    }
                }
            }

            var settingsHeight = 0f;

            if (showSettings)
            {
                headerHeight += Styles.spaceBeforeSettings;

                foreach (var setting in settings)
                {
                    var settingWidth = settingsPositions[setting].width;

                    using (LudiqGUIUtility.currentInspectorWidth.Override(settingWidth))
                    {
                        var settingHeight = LudiqGUI.GetInspectorHeight(null, setting, settingWidth, settingLabelsContents[setting] ?? GUIContent.none);

                        var settingPosition = new Rect
                            (
                            headerTextX,
                            y,
                            settingWidth,
                            settingHeight
                            );

                        settingsPositions[setting] = settingPosition;

                        settingsHeight += settingHeight;
                        y += settingHeight;

                        settingsHeight += Styles.spaceBetweenSettings;
                        y += Styles.spaceBetweenSettings;
                    }
                }

                settingsHeight -= Styles.spaceBetweenSettings;
                y -= Styles.spaceBetweenSettings;

                headerHeight += settingsHeight;

                headerHeight += Styles.spaceAfterSettings;
                y += Styles.spaceAfterSettings;
            }

            if (showHeaderAddon)
            {
                var headerAddonHeight = GetHeaderAddonHeight(headerAddonWidth);

                headerAddonPosition = new Rect
                    (
                    headerTextX,
                    y,
                    headerAddonWidth,
                    headerAddonHeight
                    );

                headerHeight += headerAddonHeight;
                y += headerAddonHeight;
            }

            if (headerHeight < Styles.iconSize)
            {
                var difference = Styles.iconSize - headerHeight;
                var centeringOffset = difference / 2;

                if (showTitle)
                {
                    var _titlePosition = titlePosition;
                    _titlePosition.y += centeringOffset;
                    titlePosition = _titlePosition;
                }

                if (showSubtitle)
                {
                    var _subtitlePosition = subtitlePosition;
                    _subtitlePosition.y += centeringOffset;
                    subtitlePosition = _subtitlePosition;
                }

                if (showSettings)
                {
                    foreach (var setting in settings)
                    {
                        var _settingPosition = settingsPositions[setting];
                        _settingPosition.y += centeringOffset;
                        settingsPositions[setting] = _settingPosition;
                    }
                }

                if (showHeaderAddon)
                {
                    var _headerAddonPosition = headerAddonPosition;
                    _headerAddonPosition.y += centeringOffset;
                    headerAddonPosition = _headerAddonPosition;
                }

                headerHeight = Styles.iconSize;
            }

            y = innerY + headerHeight;

            var innerHeight = 0f;

            innerHeight += headerHeight;

            if (showPorts)
            {
                innerHeight += Styles.spaceBeforePorts;
                y += Styles.spaceBeforePorts;

                var portsBackgroundY = y;
                var portsBackgroundHeight = 0f;

                portsBackgroundHeight += Styles.portsBackground.padding.top;
                innerHeight += Styles.portsBackground.padding.top;
                y += Styles.portsBackground.padding.top;

                var portStartY = y;

                var inputsHeight = 0f;
                var outputsHeight = 0f;

                foreach (var input in inputs)
                {
                    input.y = y;

                    var inputHeight = input.GetHeight();

                    inputsHeight += inputHeight;
                    y += inputHeight;

                    inputsHeight += Styles.spaceBetweenPorts;
                    y += Styles.spaceBetweenPorts;
                }

                if (inputs.Count > 0)
                {
                    inputsHeight -= Styles.spaceBetweenPorts;
                    y -= Styles.spaceBetweenPorts;
                }

                y = portStartY;

                foreach (var output in outputs)
                {
                    output.y = y;

                    var outputHeight = output.GetHeight();

                    outputsHeight += outputHeight;
                    y += outputHeight;

                    outputsHeight += Styles.spaceBetweenPorts;
                    y += Styles.spaceBetweenPorts;
                }

                if (outputs.Count > 0)
                {
                    outputsHeight -= Styles.spaceBetweenPorts;
                    y -= Styles.spaceBetweenPorts;
                }

                var portsHeight = Math.Max(inputsHeight, outputsHeight);

                portsBackgroundHeight += portsHeight;
                innerHeight += portsHeight;
                y = portStartY + portsHeight;

                portsBackgroundHeight += Styles.portsBackground.padding.bottom;
                innerHeight += Styles.portsBackground.padding.bottom;
                y += Styles.portsBackground.padding.bottom;

                portsBackgroundPosition = new Rect
                    (
                    edgeX,
                    portsBackgroundY,
                    edgeWidth,
                    portsBackgroundHeight
                    );
            }

            var edgeHeight = InnerToEdgePosition(new Rect(0, 0, 0, innerHeight)).height;

            _position = new Rect
                (
                edgeX,
                edgeY,
                edgeWidth,
                edgeHeight
                );
        }

        protected virtual float GetHeaderAddonWidth()
        {
            return 0;
        }

        protected virtual float GetHeaderAddonHeight(float width)
        {
            return 0;
        }

        #endregion


        #region Drawing

        protected virtual NodeColorMix baseColor => NodeColor.Gray;

        protected override NodeColorMix color
        {
            get
            {
                if (unitDebugData.runtimeException != null)
                {
                    return NodeColor.Red;
                }

                var color = baseColor;

                if (analysis.warnings.Count > 0)
                {
                    var mostSevereWarning = Warning.MostSevereLevel(analysis.warnings);

                    switch (mostSevereWarning)
                    {
                        case WarningLevel.Error:
                            color = NodeColor.Red;

                            break;

                        case WarningLevel.Severe:
                            color = NodeColor.Orange;

                            break;

                        case WarningLevel.Caution:
                            color = NodeColor.Yellow;

                            break;
                    }
                }

                if (EditorApplication.isPaused)
                {
                    if (EditorTimeBinding.frame == unitDebugData.lastInvokeFrame)
                    {
                        return NodeColor.Blue;
                    }
                }
                else
                {
                    var mix = color;
                    mix.blue = Mathf.Lerp(1, 0, (EditorTimeBinding.time - unitDebugData.lastInvokeTime) / Styles.invokeFadeDuration);

                    return mix;
                }

                return color;
            }
        }

        protected override NodeShape shape => NodeShape.Square;

        protected virtual bool showTitle => !string.IsNullOrEmpty(description.shortTitle);

        protected virtual bool showSurtitle => !string.IsNullOrEmpty(description.surtitle);

        protected virtual bool showSubtitle => !string.IsNullOrEmpty(description.subtitle);

        protected virtual bool showIcons => description.icons.Length > 0;

        protected virtual bool showSettings => settingNames.Count > 0;

        protected virtual bool showHeaderAddon => false;

        protected virtual bool showPorts => ports.Count > 0;

        protected override bool dim
        {
            get
            {
                var dim = BoltCore.Configuration.dimInactiveNodes && !analysis.isEntered;

                if (isMouseOver || isSelected)
                {
                    dim = false;
                }

                if (BoltCore.Configuration.dimIncompatibleNodes && canvas.isCreatingConnection)
                {
                    dim = !unit.ports.Any(p => canvas.connectionSource == p || canvas.connectionSource.CanValidlyConnectTo(p));
                }

                return dim;
            }
        }

        public override void DrawForeground()
        {
            BeginDim();

            base.DrawForeground();

            DrawIcon();

            if (showSurtitle)
            {
                DrawSurtitle();
            }

            if (showTitle)
            {
                DrawTitle();
            }

            if (showSubtitle)
            {
                DrawSubtitle();
            }

            if (showIcons)
            {
                DrawIcons();
            }

            if (showSettings)
            {
                DrawSettings();
            }

            if (showHeaderAddon)
            {
                DrawHeaderAddon();
            }

            if (showPorts)
            {
                DrawPortsBackground();
            }

            EndDim();
        }

        protected void DrawIcon()
        {
            var icon = description.icon ?? BoltFlow.Icons.unit;

            if (icon != null && icon[(int)iconPosition.width])
            {
                GUI.DrawTexture(iconPosition, icon[(int)iconPosition.width]);
            }
        }

        protected void DrawTitle()
        {
            GUI.Label(titlePosition, titleContent, invertForeground ? Styles.titleInverted : Styles.title);
        }

        protected void DrawSurtitle()
        {
            GUI.Label(surtitlePosition, surtitleContent, invertForeground ? Styles.surtitleInverted : Styles.surtitle);
        }

        protected void DrawSubtitle()
        {
            GUI.Label(subtitlePosition, subtitleContent, invertForeground ? Styles.subtitleInverted : Styles.subtitle);
        }

        protected void DrawIcons()
        {
            for (int i = 0; i < description.icons.Length; i++)
            {
                var icon = description.icons[i];
                var position = iconsPositions[i];

                GUI.DrawTexture(position, icon?[(int)position.width]);
            }
        }

        private void DrawSettings()
        {
            if (graph.zoom < FlowCanvas.inspectorZoomThreshold)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(!e.IsRepaint && isMouseThrough && !isMouseOver);

            EditorGUI.BeginChangeCheck();

            foreach (var setting in settings)
            {
                DrawSetting(setting);
            }

            if (EditorGUI.EndChangeCheck())
            {
                unit.Define();
                Reposition();
            }

            EditorGUI.EndDisabledGroup();
        }

        protected void DrawSetting(Metadata setting)
        {
            var settingPosition = settingsPositions[setting];

            using (LudiqGUIUtility.currentInspectorWidth.Override(settingPosition.width))
            using (Inspector.expandTooltip.Override(false))
            {
                var label = settingLabelsContents[setting];

                if (label == null)
                {
                    LudiqGUI.Inspector(setting, settingPosition, GUIContent.none);
                }
                else
                {
                    using (Inspector.defaultLabelStyle.Override(Styles.settingLabel))
                    using (LudiqGUIUtility.labelWidth.Override(Styles.settingLabel.CalcSize(label).x))
                    {
                        LudiqGUI.Inspector(setting, settingPosition, label);
                    }
                }
            }
        }

        protected virtual void DrawHeaderAddon() { }

        protected void DrawPortsBackground()
        {
            if (canvas.showRelations)
            {
                foreach (var relation in unit.relations)
                {
                    var start = ports.Single(pw => pw.port == relation.source).handlePosition.center;
                    var end = ports.Single(pw => pw.port == relation.destination).handlePosition.center;

                    var startTangent = start;
                    var endTangent = end;

                    if (relation.source is IUnitInputPort &&
                        relation.destination is IUnitInputPort)
                    {
                        //startTangent -= new Vector2(20, 0);
                        endTangent -= new Vector2(32, 0);
                    }
                    else
                    {
                        startTangent += new Vector2(innerPosition.width / 2, 0);
                        endTangent += new Vector2(-innerPosition.width / 2, 0);
                    }

                    Handles.DrawBezier
                        (
                            start,
                            end,
                            startTangent,
                            endTangent,
                            new Color(0.136f, 0.136f, 0.136f, 1.0f),
                            null,
                            3
                        );
                }
            }
            else
            {
                if (e.IsRepaint)
                {
                    Styles.portsBackground.Draw(portsBackgroundPosition, false, false, false, false);
                }
            }
        }

        #endregion


        #region Selecting

        public override bool canSelect => true;

        #endregion


        #region Dragging

        public override bool canDrag => true;

        public override void ExpandDragGroup(HashSet<IGraphElement> dragGroup)
        {
            if (BoltCore.Configuration.carryChildren)
            {
                foreach (var output in unit.outputs)
                {
                    foreach (var connection in output.connections)
                    {
                        if (dragGroup.Contains(connection.destination.unit))
                        {
                            continue;
                        }

                        dragGroup.Add(connection.destination.unit);

                        canvas.Widget(connection.destination.unit).ExpandDragGroup(dragGroup);
                    }
                }
            }
        }

        #endregion


        #region Deleting

        public override bool canDelete => true;

        #endregion


        #region Clipboard

        public override void ExpandCopyGroup(HashSet<IGraphElement> copyGroup)
        {
            copyGroup.UnionWith(unit.connections.Cast<IGraphElement>());
        }

        #endregion


        #region Context

        protected override IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                yield return new DropdownOption((Action)ReplaceUnit, "Replace...");

                foreach (var baseOption in base.contextOptions)
                {
                    yield return baseOption;
                }
            }
        }

        private void ReplaceUnit()
        {
            UnitWidgetHelper.ReplaceUnit(unit, reference, context, selection, e);
        }

        #endregion


        public static class Styles
        {
            static Styles()
            {
                // Disabling word wrap because Unity's CalcSize and CalcHeight
                // are broken w.r.t. pixel-perfection and matrix

                title = new GUIStyle(BoltCore.Styles.nodeLabel);
                title.padding = new RectOffset(0, 5, 0, 2);
                title.margin = new RectOffset(0, 0, 0, 0);
                title.fontSize = 12;
                title.alignment = TextAnchor.MiddleLeft;
                title.wordWrap = false;

                surtitle = new GUIStyle(BoltCore.Styles.nodeLabel);
                surtitle.padding = new RectOffset(0, 5, 0, 0);
                surtitle.margin = new RectOffset(0, 0, 0, 0);
                surtitle.fontSize = 10;
                surtitle.alignment = TextAnchor.MiddleLeft;
                surtitle.wordWrap = false;

                subtitle = new GUIStyle(surtitle);
                subtitle.padding.bottom = 2;

                titleInverted = new GUIStyle(title);
                titleInverted.normal.textColor = ColorPalette.unityBackgroundDark;

                surtitleInverted = new GUIStyle(surtitle);
                surtitleInverted.normal.textColor = ColorPalette.unityBackgroundDark;

                subtitleInverted = new GUIStyle(subtitle);
                subtitleInverted.normal.textColor = ColorPalette.unityBackgroundDark;

                if (EditorGUIUtility.isProSkin)
                {
                    portsBackground = new GUIStyle("In BigTitle")
                    {
                        padding = new RectOffset(0, 0, 6, 5)
                    };
                }
                else
                {
                    TextureResolution[] textureResolution = { 2 };
                    var createTextureOptions = CreateTextureOptions.Scalable;
                    EditorTexture normalTexture = BoltCore.Resources.LoadTexture($"NodePortsBackground.png", textureResolution, createTextureOptions);

                    portsBackground = new GUIStyle
                    {
                        normal = { background = normalTexture.Single() },
                        padding = new RectOffset(0, 0, 6, 5)
                    };
                }

                settingLabel = new GUIStyle(BoltCore.Styles.nodeLabel);
                settingLabel.padding.left = 0;
                settingLabel.padding.right = 5;
                settingLabel.wordWrap = false;
                settingLabel.clipping = TextClipping.Clip;
            }

            public static readonly GUIStyle title;

            public static readonly GUIStyle surtitle;

            public static readonly GUIStyle subtitle;

            public static readonly GUIStyle titleInverted;

            public static readonly GUIStyle surtitleInverted;

            public static readonly GUIStyle subtitleInverted;

            public static readonly GUIStyle settingLabel;

            public static readonly float spaceAroundLineIcon = 5;

            public static readonly float spaceBeforePorts = 5;

            public static readonly float spaceBetweenInputsAndOutputs = 8;

            public static readonly float spaceBeforeSettings = 2;

            public static readonly float spaceBetweenSettings = 3;

            public static readonly float spaceBetweenPorts = 3;

            public static readonly float spaceAfterSettings = 0;

            public static readonly float maxSettingsWidth = 150;

            public static readonly GUIStyle portsBackground;

            public static readonly float iconSize = IconSize.Medium;

            public static readonly float iconsSize = IconSize.Small;

            public static readonly float iconsSpacing = 3;

            public static readonly int iconsPerColumn = 2;

            public static readonly float spaceAfterIcon = 6;

            public static readonly float spaceAfterSurtitle = 2;

            public static readonly float spaceBeforeSubtitle = 0;

            public static readonly float invokeFadeDuration = 0.5f;
        }
    }

    internal class UnitWidgetHelper
    {
        internal static void ReplaceUnit(IUnit unit, GraphReference reference, IGraphContext context, GraphSelection selection, EventWrapper eventWrapper)
        {
            var oldUnit = unit;
            var unitPosition = oldUnit.position;
            var preservation = UnitPreservation.Preserve(oldUnit);

            var options = new UnitOptionTree(new GUIContent("Node"));
            options.filter = UnitOptionFilter.Any;
            options.filter.NoConnection = false;
            options.reference = reference;

            var activatorPosition = new Rect(eventWrapper.mousePosition, new Vector2(200, 1));

            LudiqGUI.FuzzyDropdown
            (
                activatorPosition,
                options,
                null,
                delegate (object _option)
                {
                    var option = (IUnitOption)_option;

                    context.BeginEdit();
                    UndoUtility.RecordEditedObject("Replace Node");
                    var graph = oldUnit.graph;
                    oldUnit.graph.units.Remove(oldUnit);
                    var newUnit = option.InstantiateUnit();
                    newUnit.guid = Guid.NewGuid();
                    newUnit.position = unitPosition;
                    graph.units.Add(newUnit);
                    preservation.RestoreTo(newUnit);
                    option.PreconfigureUnit(newUnit);
                    selection.Select(newUnit);
                    GUI.changed = true;
                    context.EndEdit();
                }
            );
        }
    }
}
