

using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.COMMON;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Services
{
    public class UV_LOTCONTROL_MASTER_Service : IUV_LOTCONTROL_MASTER_Service
    {
        private readonly AppDbContext _context;

        public UV_LOTCONTROL_MASTER_Service(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegenerateSerialsAsync(string lotNo)
        {
            var lot = await _context.UV_LOTCONTROL_MASTERs.FirstOrDefaultAsync(x => x.LotNo == lotNo);
            if (lot == null) return;

            var existing = _context.UV_LOTGENERALSUMMARY_MASTERs.Where(x => x.LotNo == lotNo);
            _context.UV_LOTGENERALSUMMARY_MASTERs.RemoveRange(existing);
            await _context.SaveChangesAsync();

            var serials = GenerateSerialNumbers(lot.SerialStart, lot.Quantity);
            var newSerials = serials.Select(s => new UV_LOTGENERALSUMMARY_MASTER
            {
                LotNo = lot.LotNo,
                SerialNumber = s
            });

            await _context.UV_LOTGENERALSUMMARY_MASTERs.AddRangeAsync(newSerials);
            await _context.SaveChangesAsync();
        }
        private List<string> GenerateSerialNumbers(string startSerial, int qty)
        {
            var prefix = new string(startSerial.TakeWhile(char.IsLetter).ToArray());
            var numPart = new string(startSerial.SkipWhile(char.IsLetter).ToArray());
            int startNum = int.Parse(numPart);
            int length = numPart.Length;

            return Enumerable.Range(startNum, qty)
                .Select(i => prefix + i.ToString().PadLeft(length, '0'))
                .ToList();
        }
        public async Task<PagedResult<LotControlViewModel>> GetFilteredLotsAsync(DateTime? startDate, DateTime? endDate, string searchTerm, int page, int pageSize)
        {           
            var inclusiveEnd = endDate.Value.Date.AddDays(1).AddTicks(-1);

            var query = _context.UV_LOTCONTROL_MASTERs
                .Include(x => x.SPO_MASTER)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(x => x.CreatedDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(x => x.CreatedDate <= inclusiveEnd);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    (x.LotNo ?? "").Contains(searchTerm) ||
                    (x.SerialStart ?? "").Contains(searchTerm) ||
                    (x.SerialEnd ?? "").Contains(searchTerm) ||
                    (x.PONumber ?? "").Contains(searchTerm) ||
                    (x.SPO_MASTER.Model ?? "").Contains(searchTerm));
            }

            var projected = query.Select(x => new LotControlViewModel
            {
                LotControlID = x.LotControlID,
                Model = x.SPO_MASTER.Model ?? "",
                LotNo = x.LotNo,
                Quantity = x.Quantity,
                SpecialInfo = x.SpecialInfo,
                DateCode = x.DateCode,
                Code = x.Code,
                SerialStart = x.SerialStart,
                SerialEnd = x.SerialEnd,
                ApprovedBy = x.ApprovedBy,
                Revised = x.Revised,
                IssuedBy = x.IssuedBy,
                ProDate = x.ProDate,
                IssueDate = x.IssueDate,
                PONumber = x.PONumber,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate
            });

            // Fix: Ensure ToPagedResult is awaited properly
            return await Task.FromResult(projected.ToPagedResult(page, pageSize));
        }
    }
}
