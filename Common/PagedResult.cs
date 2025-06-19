namespace MESWebDev.Common
{
    public class PagedResult<T> : IPagedResult
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public string SearchTerm { get; set; }

        public PagedResult()
        {
            Items = new List<T>();
        }
    }
}