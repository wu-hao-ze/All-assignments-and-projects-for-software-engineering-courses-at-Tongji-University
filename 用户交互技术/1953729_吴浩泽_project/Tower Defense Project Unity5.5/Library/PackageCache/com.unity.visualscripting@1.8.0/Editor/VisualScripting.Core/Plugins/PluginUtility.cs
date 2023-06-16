using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    public static class PluginUtility
    {
        public static IEnumerable<Plugin> OrderByDependencies(this IEnumerable<Plugin> plugins)
        {
            return plugins.OrderByDependencies(plugin => PluginContainer.pluginDependencies[plugin.id].Select(PluginContainer.GetPlugin));
        }

        public static IEnumerable<Plugin> ResolveDependencies(this IEnumerable<Plugin> plugins)
        {
            return plugins.Concat(plugins.SelectMany(plugin => plugin.dependencies)).Distinct().OrderByDependencies();
        }
    }
}
