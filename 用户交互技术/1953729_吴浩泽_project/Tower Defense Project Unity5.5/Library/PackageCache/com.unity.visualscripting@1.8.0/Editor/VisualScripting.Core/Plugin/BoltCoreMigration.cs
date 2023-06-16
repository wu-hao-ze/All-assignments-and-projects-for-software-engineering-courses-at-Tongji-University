using System;

namespace Unity.VisualScripting
{
    [Plugin(BoltCore.ID)]
    internal abstract class BoltCoreMigration : PluginMigration
    {
        protected BoltCoreMigration(Plugin plugin) : base(plugin) { }

        protected void AddDefaultTypeOption(Type typeOption)
        {
            if (!BoltCore.Configuration.typeOptions.Contains(typeOption))
            {
                BoltCore.Configuration.typeOptions.Add(typeOption);
                BoltCore.Configuration.Save();
            }
        }

        protected void AddDefaultAssemblyOption(string assemblyOption)
        {
            if (!BoltCore.Configuration.assemblyOptions.Contains(assemblyOption))
            {
                BoltCore.Configuration.assemblyOptions.Add(assemblyOption);
                BoltCore.Configuration.Save();
            }
        }
    }
}
