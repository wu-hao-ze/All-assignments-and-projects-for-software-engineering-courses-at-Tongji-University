using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(IUnit))]
    public class UnitEditor : GraphElementEditor<FlowGraphContext>
    {
        public UnitEditor(Metadata metadata) : base(metadata) { }

        protected IUnit unit => (IUnit)element;

        protected new UnitDescription description => (UnitDescription)base.description;

        protected new UnitAnalysis analysis => (UnitAnalysis)base.analysis;

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            height += GetHeaderHeight(width);

            height += GetWrappedInspectorHeight(width);

            if (exception != null)
            {
                height += GetExceptionHeight(width);
            }

            height += analysis.warnings.Sum(warning => warning.GetHeight(width));

            if (unit.inputs.Any())
            {
                height += GetPortsHeight(width, new GUIContent("Inputs"), unit.inputs.Cast<IUnitPort>());
            }

            if (unit.outputs.Any())
            {
                height += GetPortsHeight(width, new GUIContent("Outputs"), unit.outputs.Cast<IUnitPort>());
            }

            return height;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            y = 0;

            OnHeaderGUI(position);

            EditorGUI.BeginChangeCheck();

            OnWrappedInspectorGUI(position);

            if (EditorGUI.EndChangeCheck())
            {
                unit.Define();
            }

            if (exception != null)
            {
                y--;
                OnExceptionGUI(position.VerticalSection(ref y, GetExceptionHeight(position.width) + 1));
            }

            foreach (var warning in analysis.warnings)
            {
                y--;
                warning.OnGUI(position.VerticalSection(ref y, warning.GetHeight(position.width) + 1));
            }

            if (unit.inputs.Any())
            {
                y--;
                OnPortsGUI(position, new GUIContent("Inputs"), unit.inputs.Cast<IUnitPort>());
            }

            if (unit.outputs.Any())
            {
                y--;
                OnPortsGUI(position, new GUIContent("Outputs"), unit.outputs.Cast<IUnitPort>());
            }
        }

        private float GetPortsInnerWidth(float totalWidth)
        {
            return totalWidth - Styles.portsBackground.padding.left - Styles.portsBackground.padding.right;
        }

        private float GetPortsHeight(float totalWidth, GUIContent label, IEnumerable<IUnitPort> ports)
        {
            var innerWidth = GetPortsInnerWidth(totalWidth);

            var height = 0f;

            height += Styles.portsBackground.padding.top;
            height += Styles.portsLabel.CalcHeight(label, innerWidth);

            foreach (var port in ports)
            {
                height += Styles.spaceBetweenPorts;
                height += GetPortHeight(innerWidth, port);
            }

            height += Styles.portsBackground.padding.bottom;

            return height;
        }

        private void OnPortsGUI(Rect position, GUIContent label, IEnumerable<IUnitPort> ports)
        {
            var backgroundPosition = new Rect
                (
                position.x,
                y,
                position.width,
                GetPortsHeight(position.width, label, ports)
                );

            if (e.type == EventType.Repaint)
            {
                Styles.portsBackground.Draw(backgroundPosition, false, false, false, false);
            }

            var innerWidth = GetPortsInnerWidth(position.width);

            y += Styles.portsBackground.padding.top;
            position.x += Styles.portsBackground.padding.left;

            var labelPosition = new Rect
                (
                position.x,
                y,
                innerWidth,
                Styles.portsLabel.CalcHeight(label, innerWidth)
                );

            GUI.Label(labelPosition, label, Styles.portsLabel);

            y += labelPosition.height;

            foreach (var port in ports)
            {
                y += Styles.spaceBetweenPorts;

                var portPosition = new Rect
                    (
                    position.x,
                    y,
                    innerWidth,
                    GetPortHeight(innerWidth, port)
                    );

                OnPortGUI(portPosition, port);

                y += portPosition.height;
            }

            y += Styles.portsBackground.padding.bottom;
        }

        private GUIContent GetLabelContent(IUnitPort port)
        {
            string type;

            if (port is IUnitControlPort)
            {
                type = "Flow";
            }
            else if (port is IUnitValuePort)
            {
                type = ((IUnitValuePort)port).type.DisplayName();
            }
            else if (port is IUnitInvalidPort)
            {
                type = "Invalid";
            }
            else
            {
                throw new NotSupportedException();
            }

            return new GUIContent(string.Format($"<b>{port.Description<UnitPortDescription>().label}</b> <color=#{ColorPalette.unityForegroundDim.ToHexString()}>: {LudiqGUIUtility.EscapeRichText(type)}</color>"));
        }

        private float GetPortHeight(float paddedWidth, IUnitPort port)
        {
            var portDescription = port.Description<UnitPortDescription>();

            var labelWidth = paddedWidth - Styles.portIcon.fixedWidth - Styles.portIcon.margin.right;

            var height = 0f;

            height += Styles.portLabel.CalcHeight(GetLabelContent(port), labelWidth);

            var summary = portDescription.summary;

            if (!StringUtility.IsNullOrWhiteSpace(summary))
            {
                height += Styles.portDescription.CalcHeight(new GUIContent(summary), labelWidth);
            }

            return height;
        }

        private void OnPortGUI(Rect portPosition, IUnitPort port)
        {
            var portDescription = port.Description<UnitPortDescription>();

            var labelWidth = portPosition.width - Styles.portIcon.fixedWidth - Styles.portIcon.margin.right;

            var iconPosition = new Rect
                (
                portPosition.x,
                portPosition.y,
                Styles.portIcon.fixedWidth,
                Styles.portIcon.fixedHeight
                );

            var icon = portDescription.icon?[IconSize.Small];

            if (icon != null)
            {
                GUI.DrawTexture(iconPosition, icon);
            }

            var labelContent = GetLabelContent(port);

            var labelPosition = new Rect
                (
                iconPosition.xMax + Styles.portIcon.margin.right,
                portPosition.y,
                labelWidth,
                Styles.portLabel.CalcHeight(labelContent, labelWidth)
                );

            GUI.Label(labelPosition, labelContent, Styles.portLabel);

            var summary = portDescription.summary;

            if (!StringUtility.IsNullOrWhiteSpace(summary))
            {
                var descriptionContent = new GUIContent(summary);

                var descriptionPosition = new Rect
                    (
                    labelPosition.x,
                    labelPosition.yMax,
                    labelWidth,
                    Styles.portDescription.CalcHeight(descriptionContent, labelWidth)
                    );

                GUI.Label(descriptionPosition, descriptionContent, Styles.portDescription);
            }
        }

        public new static class Styles
        {
            static Styles()
            {
                portsBackground = new GUIStyle("IN BigTitle");
                portsBackground.padding = new RectOffset(10, 5, 7, 10);

                portsLabel = new GUIStyle(EditorStyles.label);
                portsLabel.fontSize = 13;
                portsLabel.padding = new RectOffset(0, 0, 0, 3);

                portLabel = new GUIStyle(EditorStyles.label);
                portLabel.imagePosition = ImagePosition.TextOnly;
                portLabel.wordWrap = true;
                portLabel.richText = true;

                portDescription = new GUIStyle(EditorStyles.label);
                portDescription.imagePosition = ImagePosition.TextOnly;
                portDescription.wordWrap = true;
                portDescription.richText = true;
                portDescription.fontSize = 10;
                portDescription.normal.textColor = ColorPalette.unityForegroundDim;

                portIcon = new GUIStyle();
                portIcon.imagePosition = ImagePosition.ImageOnly;
                portIcon.fixedWidth = portIcon.fixedHeight = IconSize.Small;
                portIcon.margin.right = 5;

                inspectorBackground = new GUIStyle();
                inspectorBackground.padding = new RectOffset(10, 10, 10, 10);
            }

            public static readonly GUIStyle portsBackground;

            public static readonly GUIStyle portsLabel;

            public static readonly GUIStyle portLabel;

            public static readonly GUIStyle portDescription;

            public static readonly GUIStyle portIcon;

            public static readonly float spaceBetweenPorts = 5;

            public static readonly GUIStyle inspectorBackground;
        }
    }
}
