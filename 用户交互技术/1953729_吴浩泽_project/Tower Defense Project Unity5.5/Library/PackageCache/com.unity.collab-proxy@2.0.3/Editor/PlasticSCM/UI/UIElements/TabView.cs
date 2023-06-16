using System.Collections.Generic;

using UnityEngine.UIElements;

using Unity.PlasticSCM.Editor;

namespace Unity.PlasticSCM.Editor.UI.UIElements
{
    internal class TabView : VisualElement
    {
        internal TabView()
        {
            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal Button AddTab(string name, VisualElement content)
        {
            mTabs.Add(name, content);

            Button newButton = new Button()
            {
                text = name,
                name = name
            };
            newButton.AddToClassList("tab-button");

            mButtons.Add(name, newButton);

            newButton.clickable.clickedWithEventInfo += OnClickButton;

            mTabArea.Add(newButton);

            if (mTabs.Count == 1)
                ButtonClicked(newButton);

            return newButton;
        }

        internal void SwitchContent(VisualElement content)
        {
            mContentArea.Clear();
            mContentArea.Add(content);

            foreach (Button button in mButtons.Values)
                button.RemoveFromClassList("active");
        }

        void OnClickButton(EventBase eventBase)
        {
            ButtonClicked((Button)eventBase.target);
        }

        void ButtonClicked(Button clickedButton)
        {
            VisualElement content;
            mTabs.TryGetValue(clickedButton.text, out content);

            mContentArea.Clear();
            mContentArea.Add(content);

            foreach (Button button in mButtons.Values)
                button.RemoveFromClassList("active");

            clickedButton.AddToClassList("active");
        }

        void BuildComponents()
        {
            mTabArea = this.Query<VisualElement>("TabArea");
            mContentArea = this.Query<VisualElement>("ContentArea");
        }

        void InitializeLayoutAndStyles()
        {
            name = "TabView";

            this.LoadLayout(typeof(TabView).Name);

            this.LoadStyle(typeof(TabView).Name);
        }

        VisualElement mContentArea;
        VisualElement mTabArea;

        Dictionary<string, VisualElement> mTabs = new Dictionary<string, VisualElement>();
        Dictionary<string, Button> mButtons = new Dictionary<string, Button>();
    }
}