

using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.SPO;
using Microsoft.AspNetCore.Mvc;
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
            var match = System.Text.RegularExpressions.Regex.Match(startSerial, "^([A-Za-z]*)(\\d+)$");
            if (!match.Success) return new List<string>();

            var prefix = match.Groups[1].Value;
            var numPart = match.Groups[2].Value;
            int startNum = int.Parse(numPart);
            int length = numPart.Length;

            return Enumerable.Range(startNum, qty)
                .Select(i => prefix + i.ToString().PadLeft(length, '0'))
                .ToList();
            //var prefix = new string(startSerial.TakeWhile(char.IsLetter).ToArray());
            //var numPart = new string(startSerial.SkipWhile(char.IsLetter).ToArray());
            //int startNum = int.Parse(numPart);
            //int length = numPart.Length;

            //return Enumerable.Range(startNum, qty)
            //    .Select(i => prefix + i.ToString().PadLeft(length, '0'))
            //    .ToList();
        }
        public async Task<PagedResult<LotControlViewModel>> GetFilteredLotsAsync(DateTime? startDate, DateTime? endDate, string searchTerm, int page, int pageSize)
        {
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
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
                    (x.PoNumber ?? "").Contains(searchTerm) ||
                    (x.SPO_MASTER.Model ?? "").Contains(searchTerm));
            }
            query = query.OrderByDescending(x => x.CreatedDate);
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
                PoNumber = x.PoNumber,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate
            });

            // Fix: Ensure ToPagedResult is awaited properly
            return await Task.FromResult(projected.ToPagedResult(page, pageSize));
        }

        public async Task<bool> CheckLotExistsAsync(string lotNo)
        {
            return await _context.UV_LOTCONTROL_MASTERs.AnyAsync(x => x.LotNo == lotNo);
        }

        public async Task<LotControlViewModel?> LoadLotControlOrSpoAsync(string lotNo)
        {
            var lot = await _context.UV_LOTCONTROL_MASTERs.Include(x => x.SPO_MASTER).FirstOrDefaultAsync(x => x.LotNo == lotNo);
            if (lot != null)
            {
                return new LotControlViewModel
                {
                    LotControlID = lot.LotControlID,
                    Model = lot.SPO_MASTER?.Model ?? string.Empty,
                    LotNo = lot.LotNo,
                    Quantity = lot.Quantity,
                    SpecialInfo = lot.SpecialInfo,
                    DateCode = lot.DateCode,
                    Code = lot.Code,
                    SerialStart = lot.SerialStart,
                    SerialEnd = lot.SerialEnd,
                    ApprovedBy = lot.ApprovedBy,
                    Revised = lot.Revised,
                    IssuedBy = lot.IssuedBy,
                    ProDate = lot.ProDate,
                    IssueDate = lot.IssueDate,
                    PoNumber = lot.PoNumber,
                    CreatedBy = lot.CreatedBy,
                    CreatedDate = lot.CreatedDate
                };
            }

            var spo = await _context.UV_SPO_MASTER_ALLs.FirstOrDefaultAsync(x => x.LotNo == lotNo);
            if (spo != null)
            {
                return new LotControlViewModel
                {
                    LotNo = spo.LotNo,
                    Model = spo.Model ?? string.Empty,
                    Quantity = spo.LotSize,
                    PoNumber = spo.PoNo ?? string.Empty,
                    CreatedDate = DateTime.Now
                };
            }

            return null;
        }

        public async Task<UV_SPO_MASTER_ALL_Model?> GetSpoInfoByLotAsync(string lotNo)
        {
            return await _context.UV_SPO_MASTER_ALLs.FirstOrDefaultAsync(x => x.LotNo == lotNo);
        }

        public async Task SaveOrUpdateLotAsync(LotControlViewModel model)
        {
            var existingActiveLot = await _context.UV_LOTCONTROL_MASTERs.FirstOrDefaultAsync(x => x.LotNo == model.LotNo && x.IsActive);

            if (existingActiveLot != null)
            {
                // Deactivate existing active lot
                existingActiveLot.IsActive = false;

                // Delete existing serials
                var oldSerials = _context.UV_LOTGENERALSUMMARY_MASTERs.Where(s => s.LotNo == model.LotNo);
                _context.UV_LOTGENERALSUMMARY_MASTERs.RemoveRange(oldSerials);
            }

            // Create new lot
            var newLot = new UV_LOTCONTROL_MASTER
            {
                LotNo = model.LotNo,
                Quantity = model.Quantity,
                SpecialInfo = model.SpecialInfo,
                DateCode = model.DateCode,
                Code = model.Code,
                SerialStart = model.SerialStart,
                SerialEnd = model.SerialEnd,
                ApprovedBy = model.ApprovedBy,
                Revised = model.Revised,
                IssuedBy = model.IssuedBy,
                ProDate = model.ProDate,
                IssueDate = model.IssueDate,
                PoNumber = model.PoNumber,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _context.UV_LOTCONTROL_MASTERs.AddAsync(newLot);
            await _context.SaveChangesAsync();

            // Generate serials
            var serials = GenerateSerialNumbers(model.SerialStart, model.Quantity);
            var serialEntities = serials.Select(s => new UV_LOTGENERALSUMMARY_MASTER
            {
                LotNo = model.LotNo,
                SerialNumber = s,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            });

            await _context.UV_LOTGENERALSUMMARY_MASTERs.AddRangeAsync(serialEntities);
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> SaveLotControlAsync([FromBody] LotControlViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.LotNo))
            {
                return new BadRequestObjectResult("Invalid lot data.");
            }

            await SaveOrUpdateLotAsync(model);
            return new OkObjectResult("Lot saved successfully.");
        }
    }
}
