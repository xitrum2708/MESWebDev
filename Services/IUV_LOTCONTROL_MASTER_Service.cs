using MESWebDev.Common;
using MESWebDev.Models.COMMON;

namespace MESWebDev.Services
{
    public interface IUV_LOTCONTROL_MASTER_Service
    {
        public Task RegenerateSerialsAsync(string lotNo);
        Task<PagedResult<LotControlViewModel>> GetFilteredLotsAsync(
            DateTime? startDate,
            DateTime? endDate,
            string searchTerm,
            int page,
            int pageSize
            );
    }
}
