namespace MESWebDev.Services
{
    public interface ISearchable<T>
    {
        IQueryable<T> ApplySearch(IQueryable<T> query, string searchTerm);
    }
}