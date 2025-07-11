using MESWebDev.Common;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.SPO;
using Microsoft.AspNetCore.Mvc;

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
        Task<bool> CheckLotExistsAsync(string lotNo);
        Task<LotControlViewModel?> LoadLotControlOrSpoAsync(string lotNo);
        Task<UV_SPO_MASTER_ALL_Model?> GetSpoInfoByLotAsync(string lotNo);
        Task SaveOrUpdateLotAsync(LotControlViewModel model);
        Task<IActionResult> SaveLotControlAsync([FromBody] LotControlViewModel model);
    }
}
