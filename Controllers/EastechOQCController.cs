using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.OQC;
using MESWebDev.Models.OQC.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class EastechOQCController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IConfiguration _configuration;
        private readonly DownloadExcelExportService _excelExportService;

        public EastechOQCController(AppDbContext context, ITranslationService translationService, IConfiguration configuration, DownloadExcelExportService excelExportService)
            : base(context)
        {
            _translationService = translationService;
            _configuration = configuration;
            _excelExportService = excelExportService;
        }

        [HttpGet]
        public IActionResult OQCCheck()
        {
            var usrName = User.Identity?.Name ?? "system";
            var list = _context.EASTECH_OQCs.Where(r => r.CreatedBy == usrName).OrderByDescending(x => x.CreatedDate).Take(100).ToList();
            ViewBag.RecentEntries = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ✔️ Required if using AntiForgery
        public async Task<IActionResult> OQCCheck(EASTECH_OQC_VM model)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            ModelState.Remove(nameof(model.CreatedDate));
            ModelState.Remove(nameof(model.CreatedBy));
            //ModelState.Remove("Remark");
            //ModelState.Remove("Model");
            //ModelState.Remove("LotNo");
            if (!ModelState.IsValid)
            {
                return BadRequest(_translationService.GetTranslation("Invaliddata", languageCode));
            }

            var exists = await _context.A_OperatorProcessDatas
                .Where(x => x.QrCode == model.QRCode && x.Process == "OUTPUT")
                .FirstOrDefaultAsync();
            if (exists == null)
                return BadRequest(_translationService.GetTranslation("QRCodeNotFoundOrnotYesinOutput", languageCode));

            //return BadRequest("QRCode not found in TELSTAR_ASSY.");

            var existingDqc = await _context.EASTECH_OQCs.FirstOrDefaultAsync(x => x.QRCode == model.QRCode);
            bool isUpdate = false;
            if (existingDqc != null)
            {
                if (existingDqc.Status == model.Status)
                {
                    // ❗ Same status – don't save
                    return BadRequest(_translationService.GetTranslation("AlreadyExistsWithSameStatus", languageCode));
                }
                isUpdate = true;
                existingDqc.Status = model.Status;
                existingDqc.Remark = model.Status == "NG" ? model.Remark : null;
                existingDqc.UpdateDate = DateTime.Now;
                existingDqc.CreatedBy = User.Identity?.Name ?? "system";

                _context.EASTECH_OQCs.Update(existingDqc);
            }
            else
            {
                // Fetch PCBCode properly
                var serial = await _context.tbl_EstechSerialGenerals
                    .FirstOrDefaultAsync(x => x.Serial_Number == model.QRCode);
                var dqc = new EASTECH_OQC
                {
                    QRCode = model.QRCode,
                    Status = model.Status,
                    Remark = model.Status == "NG" ? model.Remark : null,
                    Model = exists.Model,
                    PCBCode = serial?.PCBCode,
                    LotNo = exists.lotSMT,
                    Market = exists.Market,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "system"
                };

                _context.EASTECH_OQCs.Add(dqc);
            }
            await _context.SaveChangesAsync();
            return Json(new
            {
                success = true,
                isUpdate = isUpdate,
                message = _translationService.GetTranslation("SavedSuccessfully", languageCode),
                data = new
                {
                    qrCode = model.QRCode,
                    status = model.Status,
                    remark = model.Remark,
                    model = exists.Model,
                    lotNo = exists.lotSMT,
                    market = exists.Market,
                    createdDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ReportEastechOQC(DateTime? startDate,
                                                       DateTime? endDate,
                                                       string? SearchTerm,
                                                       int page = 1, int pageSize = 10)
        {
            var query = _context.EASTECH_OQCs.AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(x => x.QRCode.Contains(SearchTerm) ||
                x.Model.Contains(SearchTerm) ||
                x.LotNo.Contains(SearchTerm));
            }
            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= inclusiveEndDate);
            }
            var resultPage = query
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new EASTECHOQCVM
                {
                    LotNo = x.LotNo,
                    PCBCode = x.PCBCode,
                    Status = x.Status,
                    Remark = x.Remark,
                    CreatedBy = x.CreatedBy,
                    Model = x.Model,
                    Market = x.Market,
                    QRCode = x.QRCode,
                    CreatedDate = x.CreatedDate
                })
                .ToPagedResult(page, pageSize);
            var viewModel = new EASTECH_ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                eastechOQCVM = resultPage
            };

            ViewBag.FromDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchText = SearchTerm ?? "";

            if (resultPage == null || !resultPage.Items.Any())
            {
                ViewBag.Message = "Không có dữ liệu nào trong khoảng thời gian này.";
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EastechExportToExcelOQC(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
            var results = _context.EASTECH_OQCs
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                results = results.Where(x => x.QRCode.Contains(SearchTerm) ||
                                                           x.Model.Contains(SearchTerm) ||
                                                           x.LotNo.Contains(SearchTerm));
            }

            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);
                results = results.Where(x => x.CreatedDate >= startDate.Value.Date && x.CreatedDate < endExclusive);
            }

            if (results == null || !results.Any())
            {
                // Nếu không có dữ liệu, trả về view với thông báo
                ViewBag.Message = "Không có dữ liệu nào trong khoảng thời gian này.";
                return View(new DownloadButtonModel());
            }
            //var bytes = _excelExportService.DownloadExportToExcel(results, columnMappings);
            var bytes = _excelExportService.DownloadExportToExcel(results, null);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Eastec_DQC_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}