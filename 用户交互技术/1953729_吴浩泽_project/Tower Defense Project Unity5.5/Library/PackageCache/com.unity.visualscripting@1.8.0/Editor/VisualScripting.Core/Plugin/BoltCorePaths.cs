using System.IO;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    public class BoltCorePaths : PluginPaths
    {
        public BoltCorePaths(Plugin plugin) : base(plugin) { }

        public string variableResources => Path.Combine(persistentGenerated, "Variables/Resources");
        public string propertyProviders => Path.Combine(transientGenerated, "Property Providers");
        public string propertyProvidersEditor => Path.Combine(propertyProviders, "Editor");
        public string assemblyDocumentations => Path.Combine(transientGenerated, "Documentation");
        public string dotNetDocumentation => Path.Combine(package, "DotNetDocumentation");
    }
}
