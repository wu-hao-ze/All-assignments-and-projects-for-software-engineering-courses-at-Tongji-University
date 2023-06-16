using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    [PluginModule(required = true)]
    public abstract class PluginManifest : IPluginModule, IAboutable
    {
        protected PluginManifest(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public virtual void Initialize() { }

        public virtual void LateInitialize() { }

        public Plugin plugin { get; }

        public abstract string name { get; }
        public abstract string author { get; }
        public Texture2D logo { get; private set; }
        public abstract string description { get; }
        public abstract SemanticVersion version { get; }
        public virtual string url => null;

        public virtual string authorLabel => "Author: ";
        public Texture2D authorLogo { get; private set; }
        public virtual string copyrightHolder => author;
        public virtual int copyrightYear => DateTime.Now.Year;
        public virtual string authorUrl => null;

        public SemanticVersion currentVersion => version;

        public SemanticVersion savedVersion
        {
            get
            {
                return plugin.configuration.savedVersion;
            }
            set
            {
                plugin.configuration.savedVersion = value;
                plugin.configuration.Save();
            }
        }

        public bool versionMismatch => currentVersion != savedVersion;
    }
}
