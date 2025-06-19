using MESWebDev.Models.REPAIR;

namespace MESWebDev.Repositories
{
    public interface IRepairResultRepository
    {
        Task<List<UV_REPAIRRESULT>> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? searchText,
            string? UserDept);
    }
}