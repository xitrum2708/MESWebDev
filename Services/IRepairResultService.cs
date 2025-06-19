using MESWebDev.DTO;

namespace MESWebDev.Services
{
    public interface IRepairResultService
    {
        Task<List<RepairResultDto>> GetWithBulkCopyAsync(
           List<(string QRCode, string Partcode)> keys,
           DateTime? startDate,
           DateTime? endDate,
           string? searchText,
           string? UserDept);
    }
}