namespace MESWebDev.Common
{
    public interface IPagedResult
    {
        int CurrentPage { get; set; }
        int TotalPages { get; set; }
        int PageSize { get; set; }
        int TotalItems { get; set; }
        string SearchTerm { get; set; }
    }
}