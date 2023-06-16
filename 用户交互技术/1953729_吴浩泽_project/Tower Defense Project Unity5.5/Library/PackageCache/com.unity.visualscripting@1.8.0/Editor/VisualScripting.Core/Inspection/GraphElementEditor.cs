using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Editor(typeof(IGraphElement))]
    public class GraphElementEditor<TGraphContext> : GraphInspector<TGraphContext> where TGraphContext : class, IGraphContext
    {
        public GraphElementEditor(Metadata metadata) : base(metadata) { }

        protected IGraphElement element => (IGraphElement)metadata.value;

        protected IGraphElementDescription description => element.Description<IGraphElementDescription>();

        protected IGraphElementAnalysis analysis => element.Analysis<IGraphElementAnalysis>(context);

        protected GUIContent headerContent => description.ToGUIContent(IconSize.Medium);

        protected virtual GraphReference headerReference => reference;

        protected IGraphContext headerContext => headerReference?.Context();

        protected virtual bool editableHeader => headerReference != null && headerReference.isValid && headerTitleMetadata != null && headerSummaryMetadata != null;

        protected virtual Metadata headerTitleMetadata => null;

        protected virtual Metadata headerSummaryMetadata => null;

        protected Exception exception => ((IGraphElementWithDebugData)element).GetException(reference);

        protected string exceptionMessage => exception.ToSummaryString();

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            if (editableHeader)
            {
                height += LudiqGUI.GetHeaderHeight(this, headerTitleMetadata, headerSummaryMetadata, description.icon, width, false);
            }
            else
            {
                height += LudiqGUI.GetHeaderHeight(headerContent, width, false);
            }

            height += GetWrappedInspectorHeight(width);

            if (exception != null)
            {
                height += GetExceptionHeight(width);
            }

            return height;
        }

        protected float GetHeaderHeight(float width)
        {
            if (editableHeader)
            {
                return LudiqGUI.GetHeaderHeight(this, headerTitleMetadata, headerSummaryMetadata, description.icon, width, false);
            }
            else
            {
                return LudiqGUI.GetHeaderHeight(headerContent, width, false);
            }
        }

        protected float GetWrappedInspectorHeight(float totalWidth)
        {
            var innerWidth = GetInspectorInnerWidth(totalWidth);

            var height = GetInspectorHeight(innerWidth);

            if (height > 0)
            {
                height += Styles.inspectorBackground.padding.top;
                height += Styles.inspectorBackground.padding.bottom;
            }

            return height;
        }

        protected virtual float GetInspectorHeight(float width)
        {
            return LudiqGUI.GetInspectorHeight(this, metadata, width, GUIContent.none);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            y = 0;

            OnHeaderGUI(position);

            OnWrappedInspectorGUI(position);

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
        }

        protected void OnHeaderGUI(Rect position)
        {
            if (editableHeader)
            {
                // HACK: Have to cache the reference here because the getter uses the current context
                // as a starting point for the nester.
                var headerContext = this.headerContext;

                headerContext.BeginEdit();
                LudiqGUI.OnHeaderGUI(headerTitleMetadata, headerSummaryMetadata, description.icon, position, ref y, false);
                headerContext.EndEdit();
            }
            else
            {
                LudiqGUI.OnHeaderGUI(headerContent, position, ref y, false);
            }
        }

        protected void OnWrappedInspectorGUI(Rect position)
        {
            var innerWidth = GetInspectorInnerWidth(position.width);

            var inspectorHeight = GetInspectorHeight(innerWidth);

            if (inspectorHeight == 0)
            {
                return;
            }

            var backgroundPosition = new Rect
                (
                position.x,
                y,
                position.width,
                GetWrappedInspectorHeight(position.width)
                );

            if (e.type == EventType.Repaint)
            {
                Styles.inspectorBackground.Draw(backgroundPosition, false, false, false, false);
            }

            y += Styles.inspectorBackground.padding.top;

            var innerPosition = new Rect
                (
                position.x + Styles.inspectorBackground.padding.left,
                y,
                innerWidth,
                GetInspectorHeight(innerWidth)
                );

            var inspectorPosition = innerPosition.VerticalSection(ref y, inspectorHeight);

            OnInspectorGUI(inspectorPosition);

            y += Styles.inspectorBackground.padding.bottom;
        }

        protected virtual void OnInspectorGUI(Rect position)
        {
            LudiqGUI.Inspector(metadata, position, GUIContent.none);
        }

        protected float GetInspectorInnerWidth(float totalWidth)
        {
            return totalWidth - Styles.inspectorBackground.padding.left - Styles.inspectorBackground.padding.right;
        }

        protected float GetExceptionHeight(float width)
        {
            return LudiqGUIUtility.GetHelpBoxHeight(exceptionMessage, MessageType.Error, width);
        }

        protected void OnExceptionGUI(Rect exceptionPosition)
        {
            EditorGUI.HelpBox(exceptionPosition, exceptionMessage, MessageType.Error);

            if (GUI.Button(exceptionPosition, GUIContent.none, GUIStyle.none))
            {
                Debug.LogException(exception);
            }
        }

        public static class Styles
        {
            static Styles()
            {
                inspectorBackground = new GUIStyle();
                inspectorBackground.padding = new RectOffset(10, 10, 10, 10);
            }

            public static readonly GUIStyle inspectorBackground;
        }
    }
}
