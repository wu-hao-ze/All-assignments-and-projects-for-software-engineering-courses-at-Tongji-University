using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class Warning
    {
        public Warning(WarningLevel level, string message)
        {
            Ensure.That(nameof(message)).IsNotNull(message);

            this.level = level;
            this.message = message;
        }

        public Warning(Exception exception)
        {
            Ensure.That(nameof(exception)).IsNotNull(exception);

            this.level = WarningLevel.Error;
            this.exception = exception;
            this.message = exception.DisplayName() + ": " + exception.Message;
        }

        public WarningLevel level { get; }
        public string message { get; }
        public Exception exception { get; }

        public MessageType MessageType
        {
            get
            {
                switch (level)
                {
                    case WarningLevel.Info:
                        return MessageType.Info;

                    case WarningLevel.Caution:
                    case WarningLevel.Severe:
                        return MessageType.Warning;

                    case WarningLevel.Error:
                        return MessageType.Error;

                    default:
                        return MessageType.None;
                }
            }
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(level, message);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Warning;

            if (other == null)
            {
                return false;
            }

            return level == other.level &&
                message == other.message;
        }

        public static Warning Info(string message)
        {
            return new Warning(WarningLevel.Info, message);
        }

        public static Warning Caution(string message)
        {
            return new Warning(WarningLevel.Caution, message);
        }

        public static Warning Severe(string message)
        {
            return new Warning(WarningLevel.Severe, message);
        }

        public static Warning Error(string message)
        {
            return new Warning(WarningLevel.Error, message);
        }

        public static Warning Exception(Exception exception)
        {
            return new Warning(exception);
        }

        public static WarningLevel MostSevere(params WarningLevel[] warnings)
        {
            return MostSevere((IEnumerable<WarningLevel>)warnings);
        }

        public static WarningLevel MostSevere(IEnumerable<WarningLevel> warnings)
        {
            return (WarningLevel)warnings.Select(w => (int)w).Max();
        }

        public static WarningLevel MostSevere(WarningLevel a, WarningLevel b)
        {
            return (WarningLevel)Mathf.Max((int)a, (int)b);
        }

        public static WarningLevel MostSevereLevel(List<Warning> warnings) // No alloc version
        {
            WarningLevel mostSevereWarningLevel = WarningLevel.Info;

            for (int i = 0; i < warnings.Count; i++)
            {
                var warning = warnings[i];

                if (warning.level > mostSevereWarningLevel)
                {
                    mostSevereWarningLevel = warning.level;
                }
            }

            return mostSevereWarningLevel;
        }

        public virtual float GetHeight(float width)
        {
            return LudiqGUIUtility.GetHelpBoxHeight(message, MessageType, width);
        }

        public virtual void OnGUI(Rect position)
        {
            EditorGUI.HelpBox(position, message, MessageType);

            if (exception != null && GUI.Button(position, GUIContent.none, GUIStyle.none))
            {
                Debug.LogException(exception);
            }
        }
    }

    public sealed class ActionButtonWarning : Warning
    {
        internal ActionButtonWarning(WarningLevel level, string message, string buttonTitle, Action onClick) : base(level, message)
        {
            m_ButtonTitle = buttonTitle;
            m_OnClick = onClick;
        }

        readonly string m_ButtonTitle;
        readonly Action m_OnClick;

        const int k_ButtonHorizontalMargin = 10;
        const int k_ButtonVerticalMargin = 5;

        public override float GetHeight(float width)
        {
            return LudiqGUIUtility.GetHelpBoxHeight(message, MessageType, width) + (m_OnClick == null ? 0 : (EditorGUIUtility.singleLineHeight + k_ButtonVerticalMargin));
        }

        public override void OnGUI(Rect position)
        {
            EditorGUI.HelpBox(position, message, MessageType);

            if (m_OnClick == null) return;

            var guiContent = new GUIContent(m_ButtonTitle);
            var style = EditorStyles.miniButton;
            var width = style.CalcSize(guiContent).x;
            var rect = new Rect(position.xMax - width - k_ButtonHorizontalMargin,
                position.yMax - EditorGUIUtility.singleLineHeight - k_ButtonVerticalMargin,
                width,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(rect, guiContent, style))
            {
                m_OnClick();
            }
        }
    }
}
