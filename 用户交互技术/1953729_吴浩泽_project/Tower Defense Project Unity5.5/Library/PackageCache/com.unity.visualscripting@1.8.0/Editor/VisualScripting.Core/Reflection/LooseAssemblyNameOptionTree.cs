using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class LooseAssemblyNameOptionTree : FuzzyOptionTree
    {
        public LooseAssemblyNameOptionTree() : base(new GUIContent("Assembly"))
        {
            looseAssemblyNames = Codebase.assemblies.Select(a => new LooseAssemblyName(a.GetName().Name)).ToList();
        }

        private readonly List<LooseAssemblyName> looseAssemblyNames;

        public override bool searchable { get; } = true;

        public override IEnumerable<object> Root()
        {
            return looseAssemblyNames.OrderBy(lan => lan.name).Cast<object>();
        }

        public override IEnumerable<object> Children(object parent)
        {
            return Enumerable.Empty<object>();
        }

        public override IEnumerable<ISearchResult> SearchResults(string query, CancellationToken cancellation)
        {
            return looseAssemblyNames.Cancellable(cancellation).OrderableSearchFilter(query, LooseAssemblyNameOption.Haystack).Cast<ISearchResult>();
        }

        public override string SearchResultLabel(object item, string query)
        {
            return LooseAssemblyNameOption.SearchResultLabel((LooseAssemblyName)item, query);
        }
    }
}
