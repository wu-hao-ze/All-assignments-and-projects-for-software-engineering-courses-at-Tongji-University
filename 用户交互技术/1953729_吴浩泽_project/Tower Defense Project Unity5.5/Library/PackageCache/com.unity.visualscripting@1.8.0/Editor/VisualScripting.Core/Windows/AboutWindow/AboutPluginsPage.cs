using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class AboutPluginsPage : ListPage
    {
        public AboutPluginsPage(IEnumerable<Plugin> plugins)
        {
            Ensure.That(nameof(plugins)).IsNotNull(plugins);

            title = "About Plugins";
            shortTitle = "Plugins";
            icon = BoltCore.Resources.LoadIcon("AboutPluginsPage.png");

            foreach (var plugin in plugins)
            {
                pages.Add(new AboutablePage(plugin.manifest));
            }
        }
    }
}
