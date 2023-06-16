namespace Unity.VisualScripting
{
    public interface ISearchResult
    {
        object result { get; }
        float relevance { get; }
    }

    public struct SearchResult<T> : ISearchResult
    {
        public T result { get; }
        public float relevance { get; }

        object ISearchResult.result => result;

        public SearchResult(T result, float relevance)
        {
            this.result = result;
            this.relevance = relevance;
        }
    }
}
