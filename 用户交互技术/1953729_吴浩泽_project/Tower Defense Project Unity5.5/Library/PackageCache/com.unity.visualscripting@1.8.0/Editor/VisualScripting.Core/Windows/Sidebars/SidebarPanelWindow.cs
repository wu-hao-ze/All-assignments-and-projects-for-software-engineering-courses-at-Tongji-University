using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class SidebarPanelWindow<TPanelContent> : LudiqEditorWindow
        where TPanelContent : ISidebarPanelContent
    {
        protected abstract GUIContent defaultTitleContent { get; }

        protected Event e => Event.current;

        [DoNotSerialize]
        private TPanelContent _panel;

        [DoNotSerialize]
        public TPanelContent panel
        {
            get => _panel;
            set
            {
                _panel = value;
                titleContent = panel?.titleContent ?? defaultTitleContent;
            }
        }

        private Vector2 scroll;

        protected virtual void OnEnable()
        {
            titleContent = defaultTitleContent;
            autoRepaintOnSceneChange = true;
        }

        protected virtual void OnDisable()
        {
        }

        protected override void Update()
        {
            base.Update();
            Repaint();
        }

        protected virtual void BeforeGUI()
        {
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (EditorApplication.isCompiling)
            {
                LudiqGUI.CenterLoader();
                return;
            }

            if (PluginContainer.anyVersionMismatch)
            {
                LudiqGUI.VersionMismatchShieldLayout();
                return;
            }

            if (e.type == EventType.Layout)
            {
                return;
            }

            BeforeGUI();

            if (panel == null)
            {
                return;
            }

            LudiqGUIUtility.BeginScrollableWindow(position, panel.GetHeight, out var innerPosition, ref scroll);

            EditorGUI.BeginChangeCheck();

            panel.OnGUI(innerPosition);

            if (EditorGUI.EndChangeCheck())
            {
                Repaint();
            }

            LudiqGUIUtility.EndScrollableWindow();
        }
    }
}
