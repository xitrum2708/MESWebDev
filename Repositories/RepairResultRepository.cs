using MESWebDev.Data;
using MESWebDev.Models.REPAIR;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Repositories
{
    public class RepairResultRepository : IRepairResultRepository
    {
        private readonly AppDbContext _context;

        public RepairResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UV_REPAIRRESULT>> GetFilteredAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? searchText,
             string? UserDept)
        {
            // If all parameters are null, return an empty list
            if (!startDate.HasValue && !endDate.HasValue && string.IsNullOrWhiteSpace(searchText))
            {
                return new List<UV_REPAIRRESULT>();
            }
            var q = _context.UV_REPAIRRESULTs
                    .Where(r => r.UserDept == UserDept)
                    .AsNoTracking();

            if (startDate.HasValue)
                q = q.Where(r => r.CreatedDate >= startDate.Value || r.DDRDate >= startDate.Value);
            if (endDate.HasValue)
                q = q.Where(r => r.CreatedDate <= endDate.Value || r.DDRDate <= endDate.Value);
            //}

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                q = q.Where(r =>
                    r.Qrcode.Contains(searchText) ||
                    r.Model.Contains(searchText) ||
                    r.Lot.Contains(searchText) ||
                    r.Partcode.Contains(searchText));
            }
            // Clean Causetype in memory
            var results = await q.ToListAsync();
            foreach (var result in results)
            {
                if (!string.IsNullOrEmpty(result.Causetype))
                {
                    int index = result.Causetype.IndexOf(";-->");
                    if (index >= 0)
                        result.Causetype = result.Causetype.Substring(index + 4);
                }
                if (!string.IsNullOrEmpty(result.Errortype))
                {
                    int index = result.Errortype.IndexOf(";-->");
                    if (index >= 0)
                        result.Errortype = result.Errortype.Substring(index + 4);
                }
            }
            return results;
        }
    }
}