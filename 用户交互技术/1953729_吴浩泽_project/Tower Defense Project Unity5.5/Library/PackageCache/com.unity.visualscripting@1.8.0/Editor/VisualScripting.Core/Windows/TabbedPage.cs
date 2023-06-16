using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class TabbedPage : Page
    {
        public TabbedPage() : base()
        {
            pages = new List<Page>();
            pageOptions = new List<ListOption>();
        }

        private Page _currentPage;

        public List<Page> pages { get; }
        private List<ListOption> pageOptions { get; }

        public Page currentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                currentPage?.Close();

                _currentPage = value;

                currentPage?.Show();
            }
        }

        public void UpdateOptions()
        {
            pageOptions.Clear();
            pageOptions.AddRange(pages.Select(page => new ListOption(page, new GUIContent(page.shortTitle))));
            currentPage = pages.FirstOrDefault();
        }

        protected override void OnShow()
        {
            base.OnShow();

            UpdateOptions();
        }

        public override void Update()
        {
            if (currentPage.CompleteSwitch())
            {
                return;
            }

            currentPage?.Update();
        }

        protected virtual void OnEmptyGUI()
        {
            GUILayout.BeginVertical(Styles.emptyBackground);
            LudiqGUI.FlexibleSpace();
            GUILayout.Label("No item found.", LudiqStyles.centeredLabel);
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndVertical();
        }

        protected override void OnHeaderGUI()
        {
            if (currentPage == null)
            {
                base.OnHeaderGUI();
                return;
            }

            GUILayout.BeginVertical(LudiqStyles.windowHeaderBackground, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            LudiqGUI.FlexibleSpace();

            if (currentPage?.icon != null)
            {
                GUILayout.Box(new GUIContent(currentPage.icon?[(int)LudiqStyles.windowHeaderIcon.fixedWidth]), LudiqStyles.windowHeaderIcon);
                LudiqGUI.Space(LudiqStyles.spaceBetweenWindowHeaderIconAndTitle);
            }

            GUILayout.Label(title, LudiqStyles.windowHeaderTitle);
            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();

            LudiqGUI.Space(13);
            OnTabsGUI();
            LudiqGUI.Space(-5);

            LudiqGUI.EndVertical();
        }

        protected void OnTabsGUI()
        {
            LudiqGUI.BeginHorizontal();
            LudiqGUI.FlexibleSpace();

            for (var i = 0; i < pageOptions.Count; i++)
            {
                GUIStyle tabStyle;

                if (pageOptions.Count > 1)
                {
                    if (i == 0)
                    {
                        tabStyle = Styles.tabLeft;
                    }
                    else if (i == pageOptions.Count - 1)
                    {
                        tabStyle = Styles.tabRight;
                    }
                    else
                    {
                        tabStyle = Styles.tabMid;
                    }
                }
                else
                {
                    tabStyle = Styles.tabSingle;
                }

                var tabPage = (Page)pageOptions[i].value;
                var tabLabel = pageOptions[i].label;

                if (GUILayout.Toggle(currentPage == tabPage, tabLabel, tabStyle) && currentPage != tabPage)
                {
                    currentPage = tabPage;
                }
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
        }

        protected override void OnContentGUI()
        {
            if (pageOptions.Count == 0)
            {
                OnEmptyGUI();
            }
            else
            {
                currentPage?.DrawContent();
            }
        }

        public static class Styles
        {
            static Styles()
            {
                emptyBackground = ColorPalette.unityBackgroundMid.CreateBackground();
                emptyBackground.padding = new RectOffset(10, 10, 10, 10);

                tabLeft = new GUIStyle("ButtonLeft");
                tabMid = new GUIStyle("ButtonMid");
                tabRight = new GUIStyle("ButtonRight");
                tabSingle = new GUIStyle("Button");

                tabLeft.padding = tabMid.padding = tabRight.padding = tabSingle.padding = new RectOffset(10, 10, 5, 5);
            }

            public static readonly GUIStyle emptyBackground;

            public static readonly GUIStyle tabLeft;
            public static readonly GUIStyle tabMid;
            public static readonly GUIStyle tabRight;
            public static readonly GUIStyle tabSingle;
        }
    }
}
