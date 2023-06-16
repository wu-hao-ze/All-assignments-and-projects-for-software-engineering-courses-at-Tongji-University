using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IFuzzyOptionTree
    {
        bool multithreaded { get; }

        GUIContent header { get; }

        bool searchable { get; }

        ICollection<object> favorites { get; }

        ICollection<object> selected { get; }

        bool showBackgroundWorkerProgress { get; }

        IFuzzyOption Option(object item);

        void Prewarm();
        IEnumerable<object> Root();
        IEnumerable<object> Children(object parent);
        IEnumerable<object> OrderedSearchResults(string query, CancellationToken cancellation);
        IEnumerable<ISearchResult> SearchResults(string query, CancellationToken cancellation);
        string SearchResultLabel(object item, string query);
        string FavoritesLabel(object item);
        bool CanFavorite(object item);
        void OnFavoritesChange();
    }
}
