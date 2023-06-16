using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class FuzzyOptionTree : IFuzzyOptionTree
    {
        protected FuzzyOptionTree() { }

        protected FuzzyOptionTree(GUIContent header) : this()
        {
            this.header = header;
        }

        public GUIContent header { get; protected set; }

        public ICollection<object> selected { get; } = new HashSet<object>();

        public bool showBackgroundWorkerProgress { get; protected set; }

        public virtual void Prewarm() { }

        public virtual IFuzzyOption Option(object item)
        {
            if (item == null)
            {
                return GetNullOption();
            }

            return FuzzyOptionProvider.instance.GetDecorator(item);
        }

        protected virtual IFuzzyOption GetNullOption()
        {
            return new NullOption();
        }

        #region Multithreading

        public bool multithreaded { get; } = true;

        #endregion

        #region Hierarchy

        public abstract IEnumerable<object> Root();

        public abstract IEnumerable<object> Children(object parent);

        #endregion

        #region Search

        public virtual bool searchable { get; } = false;

        public virtual IEnumerable<object> OrderedSearchResults(string query, CancellationToken cancellation)
        {
            return SearchResults(query, cancellation).OrderByRelevance();
        }

        public virtual IEnumerable<ISearchResult> SearchResults(string query, CancellationToken cancellation)
        {
            return Enumerable.Empty<ISearchResult>();
        }

        public virtual string SearchResultLabel(object item, string query)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Favorites

        public virtual ICollection<object> favorites => null;

        public virtual string FavoritesLabel(object item)
        {
            throw new NotSupportedException();
        }

        public virtual bool CanFavorite(object item)
        {
            throw new NotSupportedException();
        }

        public virtual void OnFavoritesChange() { }

        #endregion
    }
}
