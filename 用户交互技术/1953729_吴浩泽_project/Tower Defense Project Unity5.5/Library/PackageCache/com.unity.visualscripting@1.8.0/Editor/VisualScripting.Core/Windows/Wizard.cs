using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class Wizard : EditorWindowWrapper
    {
        protected Wizard()
        {
            pages = new List<Page>();
            canNavigateForward = false;
            canNavigateBackward = true;
        }

        private Page _currentPage;

        public List<Page> pages { get; }

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

                window.Repaint();
            }
        }

        public int farthestNavigatedIndex { get; set; }
        public bool canNavigateForward { get; set; }
        public bool canNavigateBackward { get; set; }

        public new void Show()
        {
            if (window == null)
            {
                ShowUtility();
                window.Center();
            }
            else
            {
                window.Focus();
            }
        }

        public override void OnShow()
        {
            for (var i = 0; i < pages.Count; i++)
            {
                var page = pages[i];

                var nextIndex = i + 1;

                if (i < pages.Count - 1)
                {
                    page.onComplete = () =>
                    {
                        currentPage = pages[nextIndex];
                        farthestNavigatedIndex = Mathf.Max(farthestNavigatedIndex, nextIndex);
                    };

                    page.completeLabel = "Next";
                }
                else
                {
                    page.onComplete = () => { throw new WindowClose(); };
                }
            }

            currentPage = pages.First();
            farthestNavigatedIndex = 0;
        }

        public override void OnClose()
        {
            currentPage.Close();
        }

        public override void Update()
        {
            if (currentPage.CompleteSwitch())
            {
                return;
            }

            currentPage.Update();
        }

        public override void OnGUI()
        {
            LudiqGUI.BeginVertical();

            currentPage?.DrawHeader();

            GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            currentPage?.DrawContent();
            LudiqGUI.EndVertical();

            GUILayout.Box(GUIContent.none, Styles.sectionBorder);
            GUILayout.BeginHorizontal(Styles.footerBackground);
            LudiqGUI.FlexibleSpace();

            for (var i = 0; i < pages.Count; i++)
            {
                GUIStyle breadcrumbStyle;

                if (i == 0)
                {
                    breadcrumbStyle = Styles.breadcrumbStart;
                }
                else if (i == pages.Count - 1)
                {
                    breadcrumbStyle = Styles.breadcrumbEnd;
                }
                else
                {
                    breadcrumbStyle = Styles.breadcrumbMid;
                }

                var page = pages[i];

                EditorGUI.BeginDisabledGroup((!canNavigateForward && i > farthestNavigatedIndex) || (!canNavigateBackward && i < farthestNavigatedIndex));

                if (GUILayout.Toggle(page == currentPage, page.shortTitle ?? page.title, breadcrumbStyle) && page != currentPage)
                {
                    currentPage = page;
                }

                EditorGUI.EndDisabledGroup();
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndHorizontal();
            LudiqGUI.EndVertical();
        }

        private static class Styles
        {
            static Styles()
            {
                footerBackground = ColorPalette.unityBackgroundDark.CreateBackground();
                footerBackground.padding = new RectOffset(10, 10, 10, 10);

                sectionBorder = ColorPalette.unityBackgroundVeryDark.CreateBackground();
                sectionBorder.fixedHeight = 1;

                breadcrumbStart = new GUIStyle("GUIEditor.BreadcrumbLeft");
                breadcrumbStart.fixedHeight = 0;
                breadcrumbStart.border = new RectOffset(3, 10, 3, 3);
                breadcrumbStart.padding = new RectOffset(3, 10, 6, 7);

                breadcrumbMid = new GUIStyle("GUIEditor.BreadcrumbMid");
                breadcrumbMid.fixedHeight = 0;
                breadcrumbMid.border = new RectOffset(10, 10, 3, 3);
                breadcrumbMid.padding = new RectOffset(10, 8, 6, 7);

                breadcrumbEnd = breadcrumbMid;
            }

            public static readonly GUIStyle sectionBorder;
            public static readonly GUIStyle footerBackground;
            public static readonly GUIStyle breadcrumbStart;
            public static readonly GUIStyle breadcrumbMid;
            public static readonly GUIStyle breadcrumbEnd;
        }
    }
}
