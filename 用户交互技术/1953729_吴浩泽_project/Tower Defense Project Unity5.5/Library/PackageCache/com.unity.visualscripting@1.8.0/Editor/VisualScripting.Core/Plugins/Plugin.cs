using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class Plugin
    {
        protected Plugin()
        {
            id = PluginContainer.GetPluginID(GetType());
            if (PluginContainer.pluginDependencies != null)
                dependencies = PluginContainer.pluginDependencies[id].Select(PluginContainer.GetPlugin).ToList().AsReadOnly();
        }

        public string id { get; }
        public ReadOnlyCollection<Plugin> dependencies { get; }

        public PluginManifest manifest { get; internal set; }
        public PluginConfiguration configuration { get; internal set; }
        public PluginPaths paths { get; internal set; }
        public PluginResources resources { get; internal set; }

        public virtual IEnumerable<ScriptReferenceReplacement> scriptReferenceReplacements => Enumerable.Empty<ScriptReferenceReplacement>();

        public virtual IEnumerable<object> aotStubs => Enumerable.Empty<object>();

        public virtual IEnumerable<string> tips => Enumerable.Empty<string>();

        public virtual IEnumerable<Page> SetupWizardPages()
        {
            return Enumerable.Empty<Page>();
        }

        public Assembly editorAssembly => GetType().Assembly;

        public Assembly runtimeAssembly
        {
            get
            {
                return Codebase.ludiqRuntimeAssemblies.Single(a => a.GetName().Name == GetType().GetAttribute<PluginRuntimeAssemblyAttribute>().assemblyName);
            }
        }

        public virtual void RunAction() { }
    }
}
