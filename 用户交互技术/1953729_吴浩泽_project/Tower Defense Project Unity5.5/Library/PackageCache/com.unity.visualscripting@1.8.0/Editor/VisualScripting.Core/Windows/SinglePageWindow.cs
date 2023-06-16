using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class SinglePageWindow<TPage> : EditorWindowWrapper where TPage : Page
    {
        protected SinglePageWindow() { }

        private TPage _page;

        public TPage page
        {
            get
            {
                if (_page == null)
                {
                    _page = CreatePage();

                    if (_page == null)
                    {
                        throw new InvalidImplementationException();
                    }

                    _page.onComplete = Close;
                }

                return _page;
            }
        }

        protected abstract TPage CreatePage();

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent(page.title, page.icon?[IconSize.Small]);
        }

        public override void OnShow()
        {
            page.Show();
        }

        public override void Update()
        {
            if (page.CompleteSwitch())
            {
                return;
            }

            page.Update();
        }

        public override void OnClose()
        {
            page.Close();
        }

        public override void OnFocus()
        {
            page.OnFocus();
        }

        public override void OnLostFocus()
        {
            page.OnLostFocus();
        }

        public override void OnGUI()
        {
            LudiqGUI.BeginVertical();
            page.DrawHeader();
            // GUILayout.Box(GUIContent.none, LudiqStyles.horizontalSeparator);
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            page.DrawContent();
            LudiqGUI.EndHorizontal();
            LudiqGUI.EndVertical();
        }
    }
}
