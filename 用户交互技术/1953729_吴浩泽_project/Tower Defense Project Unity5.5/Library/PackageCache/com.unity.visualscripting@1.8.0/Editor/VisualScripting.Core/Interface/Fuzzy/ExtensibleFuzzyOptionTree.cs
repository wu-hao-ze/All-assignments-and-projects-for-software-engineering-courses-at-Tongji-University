using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class ExtensibleFuzzyOptionTree : FuzzyOptionTree
    {
        protected ExtensibleFuzzyOptionTree()
        {
            extensions = this.Extensions().ToList();
        }

        protected ExtensibleFuzzyOptionTree(GUIContent header) : this()
        {
            this.header = header;
        }

        public IList<IFuzzyOptionTree> extensions { get; }

        public override void Prewarm()
        {
            foreach (var extension in extensions)
            {
                extension.Prewarm();
            }
        }

        public override IFuzzyOption Option(object item)
        {
            if (item is IFuzzyOption option)
            {
                return option;
            }

            foreach (var extension in extensions)
            {
                var extensionOption = extension.Option(item);

                if (extensionOption != null)
                {
                    return extensionOption;
                }
            }

            return base.Option(item);
        }

        #region Hierarchy

        public override IEnumerable<object> Root()
        {
            foreach (var extension in extensions)
            {
                foreach (var extensionRootItem in extension.Root())
                {
                    yield return extensionRootItem;
                }
            }
        }

        public override IEnumerable<object> Children(object parent)
        {
            foreach (var extension in extensions)
            {
                foreach (var extensionChild in extension.Children(parent))
                {
                    yield return extensionChild;
                }
            }
        }

        #endregion

        #region Search

        public override IEnumerable<object> OrderedSearchResults(string query, CancellationToken cancellation)
        {
            foreach (var baseSearchResult in base.OrderedSearchResults(query, cancellation))
            {
                yield return baseSearchResult;
            }

            foreach (var extension in extensions)
            {
                if (extension.searchable)
                {
                    foreach (var extensionSearchResult in extension.OrderedSearchResults(query, cancellation))
                    {
                        yield return extensionSearchResult;
                    }
                }
            }
        }

        public override IEnumerable<ISearchResult> SearchResults(string query, CancellationToken cancellation)
        {
            foreach (var baseSearchResult in base.SearchResults(query, cancellation))
            {
                yield return baseSearchResult;
            }

            foreach (var extension in extensions)
            {
                if (extension.searchable)
                {
                    foreach (var extensionSearchResult in extension.SearchResults(query, cancellation))
                    {
                        yield return extensionSearchResult;
                    }
                }
            }
        }

        public override string SearchResultLabel(object item, string query)
        {
            foreach (var extension in extensions)
            {
                var extensionSearchResultLabel = extension.SearchResultLabel(item, query);

                if (extensionSearchResultLabel != null)
                {
                    return extensionSearchResultLabel;
                }
            }

            return null;
        }

        #endregion
    }
}
