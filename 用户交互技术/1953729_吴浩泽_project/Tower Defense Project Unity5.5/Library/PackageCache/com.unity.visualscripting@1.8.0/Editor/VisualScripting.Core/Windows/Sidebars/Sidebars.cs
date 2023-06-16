using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public sealed class Sidebars
    {
        public Sidebars()
        {
            left = new Sidebar(this, SidebarAnchor.Left);
            right = new Sidebar(this, SidebarAnchor.Right);
        }

        [Serialize]
        public Sidebar left { get; private set; }

        [Serialize]
        public Sidebar right { get; private set; }

        [Serialize]
        public List<SidebarPanel> panels { get; private set; } = new List<SidebarPanel>();

        [DoNotSerialize]
        public Sidebar this[SidebarAnchor anchor]
        {
            get
            {
                switch (anchor)
                {
                    case SidebarAnchor.Left: return left;
                    case SidebarAnchor.Right: return right;
                    default: throw new UnexpectedEnumValueException<SidebarAnchor>(anchor);
                }
            }
        }

        public void Feed(IEnumerable<ISidebarPanelContent> panelContents)
        {
            Ensure.That(nameof(panelContents)).IsNotNull(panelContents);

            foreach (var panel in panels)
            {
                panel.Disable();
            }

            foreach (var panelContent in panelContents)
            {
                Feed(panelContent);
            }
        }

        public void Feed(ISidebarPanelContent panelContent)
        {
            var associated = panels.Any(panel => panel.TryAssociate(panelContent));

            if (!associated)
            {
                panels.Add(new SidebarPanel(panelContent));
            }

            foreach (var panel in panels)
            {
                panel.sidebars = this;
            }
        }

        public void Remove<T>() where T : ISidebarPanelContent
        {
            left.Remove<T>();
            right.Remove<T>();
        }
    }
}
