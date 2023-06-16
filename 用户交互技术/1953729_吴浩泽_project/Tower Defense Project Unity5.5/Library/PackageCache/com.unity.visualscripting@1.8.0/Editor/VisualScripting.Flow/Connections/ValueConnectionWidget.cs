using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(ValueConnection))]
    public sealed class ValueConnectionWidget : UnitConnectionWidget<ValueConnection>
    {
        public ValueConnectionWidget(FlowCanvas canvas, ValueConnection connection) : base(canvas, connection) { }

        private new ValueConnection.DebugData ConnectionDebugData => GetDebugData<ValueConnection.DebugData>();


        #region Drawing

        public override Color color => DetermineColor(connection.source.type, connection.destination.type);

        protected override bool colorIfActive => !BoltFlow.Configuration.animateControlConnections || !BoltFlow.Configuration.animateValueConnections;

        public override void DrawForeground()
        {
            base.DrawForeground();

            if (BoltFlow.Configuration.showConnectionValues)
            {
                var showLastValue = EditorApplication.isPlaying && ConnectionDebugData.assignedLastValue;
                var showPredictedvalue = BoltFlow.Configuration.predictConnectionValues && !EditorApplication.isPlaying && Flow.CanPredict(connection.source, reference);

                if (showLastValue || showPredictedvalue)
                {
                    var previousIconSize = EditorGUIUtility.GetIconSize();
                    EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

                    object value;

                    if (showLastValue)
                    {
                        value = ConnectionDebugData.lastValue;
                    }
                    else // if (showPredictedvalue)
                    {
                        value = Flow.Predict(connection.source, reference);
                    }

                    var label = new GUIContent(value.ToShortString(), Icons.Type(value?.GetType())?[IconSize.Small]);
                    var labelSize = Styles.prediction.CalcSize(label);
                    var labelPosition = new Rect(position.position - labelSize / 2, labelSize);

                    BeginDim();

                    GUI.Label(labelPosition, label, Styles.prediction);

                    EndDim();

                    EditorGUIUtility.SetIconSize(previousIconSize);
                }
            }
        }

        public static Color DetermineColor(Type source, Type destination)
        {
            if (destination == typeof(object))
            {
                return DetermineColor(source);
            }

            return DetermineColor(destination);
        }

        public static Color DetermineColor(Type type)
        {
            if (type == null)
            {
                return new Color(0.8f, 0.8f, 0.8f);
            }

            if (type == typeof(string))
            {
                return new Color(1.0f, 0.62f, 0.35f);
            }

            if (type == typeof(bool))
            {
                return new Color(0.86f, 0.55f, 0.92f);
            }

            if (type == typeof(char))
            {
                return new Color(1.0f, 0.90f, 0.40f);
            }

            if (type.IsEnum)
            {
                return new Color(1.0f, 0.63f, 0.66f);
            }

            if (type.IsNumeric())
            {
                return new Color(0.45f, 0.78f, 1f);
            }

            if (type.IsNumericConstruct())
            {
                return new Color(0.45f, 1.00f, 0.82f);
            }

            return new Color(0.60f, 0.88f, 0.00f);
        }

        #endregion


        #region Droplets

        protected override bool showDroplets => BoltFlow.Configuration.animateValueConnections;

        protected override Vector2 GetDropletSize()
        {
            return BoltFlow.Icons.valuePortConnected?[12].Size() ?? 12 * Vector3.one;
        }

        protected override void DrawDroplet(Rect position)
        {
            if (BoltFlow.Icons.valuePortConnected != null)
            {
                GUI.DrawTexture(position, BoltFlow.Icons.valuePortConnected?[12]);
            }
        }

        #endregion


        private static class Styles
        {
            static Styles()
            {
                prediction = new GUIStyle(EditorStyles.label);
                prediction.normal.textColor = Color.white;
                prediction.fontSize = 9;
                prediction.normal.background = new Color(0, 0, 0, 0.25f).GetPixel();
                prediction.padding = new RectOffset(4, 6, 3, 3);
                prediction.margin = new RectOffset(0, 0, 0, 0);
                prediction.alignment = TextAnchor.MiddleCenter;
            }

            public static readonly GUIStyle prediction;
        }
    }
}
