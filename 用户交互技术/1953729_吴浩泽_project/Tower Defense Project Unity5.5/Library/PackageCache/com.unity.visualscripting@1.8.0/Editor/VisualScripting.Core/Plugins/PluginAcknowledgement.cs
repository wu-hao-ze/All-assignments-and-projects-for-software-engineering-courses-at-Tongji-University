using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Unity.VisualScripting
{
    public abstract class PluginAcknowledgement : IPluginLinked
    {
        protected PluginAcknowledgement(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public Plugin plugin { get; }

        public abstract string title { get; }
        public abstract string author { get; }

        public virtual int? copyrightYear => null;
        public virtual string url => null;
        public virtual string licenseName => null;
        public virtual string licenseText => null;
    }
}
