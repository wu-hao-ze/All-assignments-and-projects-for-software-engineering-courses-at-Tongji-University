using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity.VisualScripting
{
    public abstract class PluginMigration : IPluginLinked, IComparable<PluginMigration>
    {
        protected PluginMigration(Plugin plugin)
        {
            this.plugin = plugin;

            _requiredActions = new List<string>();
            requiredActions = _requiredActions.AsReadOnly();
        }

        public Plugin plugin { get; }

        public abstract SemanticVersion from { get; }
        public abstract SemanticVersion to { get; }

        // migrations are ordered by this order (is optional) starting from 1 -> n
        public int order { get; protected set; } = 1;

        private List<string> _requiredActions { get; }

        public ReadOnlyCollection<string> requiredActions { get; private set; }

        protected void RequireAction(string action)
        {
            _requiredActions.Add(action);
        }

        protected void RequireActions(params string[] actions)
        {
            _requiredActions.AddRange(actions);
        }

        public abstract void Run();

        public int CompareTo(PluginMigration other)
        {
            return from.CompareTo(other.from);
        }
    }
}
