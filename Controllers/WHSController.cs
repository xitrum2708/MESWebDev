using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Filters;
using MESWebDev.Models;
using MESWebDev.Models.TELSTAR.VM;
using MESWebDev.Models.WHS;
using MESWebDev.Models.WHS.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;

namespace MESWebDev.Controllers
{
    public class WHSController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly DownloadExcelExportService _excelExportService;
        public WHSController(AppDbContext context,ITranslationService translationService, DownloadExcelExportService excelExportService)
            : base(context)
        {
            _translationService = translationService;
            _excelExportService = excelExportService;
        }
        [AuthorizeLogin]
        [HttpGet]
        public IActionResult WHSSortingList(DateTime? startDate, DateTime? endDate, string? SearchTerm, int page = 1, int pageSize = 10)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var errorData = (from a1 in _context.UV_IQC_ReportItems
                            where a1.ErrorCodeID > 0 && a1.ErrorCodeID != 4
                            group a1 by a1.ReportID into g
                            select new
                            {
                                ReportID = g.Key,
                                CombinedErrorDescriptions = string.Join(", ", g
                                    .Join(_context.UV_IQC_ErrorsItemMasters,
                                          a2 => a2.ErrorCodeID,
                                          b => b.ErrorCodeID,
                                          (a2, b) => b.Description ?? "")),
                                NG_Rate = g.Max(x => x.NG_Rate)
                            }).ToList();

            // Filter base query
            var reportsQuery = _context.UV_IQC_Reports
                .Where(r => r.Status == "REJECTED")
                .AsEnumerable() // 💡 Convert EF query to in-memory to support join with errorData
                .Join(errorData,
                    r => r.ReportID,
                    e => e.ReportID,
                    (r, sub) => new IQCRejectedReportViewModel
                    {
                        ReportID = r.ReportID,
                        LottagId = r.LottagId,
                        VendorName = r.VendorName,
                        VendorCode = r.VendorCode,
                        PartName = r.PartName,
                        PartCode = r.PartCode,
                        PO_NO = r.PO_NO,
                        InvoiceNo = r.InvoiceNo,
                        POQty = r.POQty,
                        POCount = r.POCount,
                        InspectionDate = r.InspectionDate,
                        InspectionGroupID = r.InspectionGroupID,
                        FinalJudgment = r.FinalJudgment,
                        Status = r.Status,
                        CheckerStatus = r.CheckerStatus,
                        Remark = r.Remark,
                        CreatedBy = r.CreatedBy,
                        CreatedDate = r.CreatedDate,
                        UpdatedBy = r.UpdatedBy,
                        UpdatedDate = r.UpdatedDate,
                        Notes = r.Notes,
                        NotesReturn = r.NotesReturn,
                        TextRemark = r.TextRemark,
                        CombinedErrorDescriptions = sub.CombinedErrorDescriptions,
                        NG_Rate = sub.NG_Rate
                    });

            // Apply date range filter if provided
            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                reportsQuery = reportsQuery.Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= inclusiveEndDate);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                reportsQuery = reportsQuery.Where(r =>
                (r.VendorName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.VendorCode ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.PartName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.PartCode ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.PO_NO ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.InvoiceNo ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.LottagId ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.ReportID ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            }

            var reports = reportsQuery
                .OrderByDescending(r => r.CreatedDate)
                .AsQueryable()
                .ToPagedResult(page, pageSize);
            var viewModel = new WHSViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                WHSSortingList = reports
            };

            ViewBag.FromDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchText = SearchTerm ?? "";

            if (reports == null || !reports.Items.Any())
            {
                ViewBag.Message =_translationService.GetTranslation("NotFoundData",languageCode) ;
            }
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ExportToExcelWHSSortingList(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var errorData = (from a1 in _context.UV_IQC_ReportItems
                             where a1.ErrorCodeID > 0 && a1.ErrorCodeID != 4
                             group a1 by a1.ReportID into g
                             select new
                             {
                                 ReportID = g.Key,
                                 CombinedErrorDescriptions = string.Join(", ", g
                                     .Join(_context.UV_IQC_ErrorsItemMasters,
                                           a2 => a2.ErrorCodeID,
                                           b => b.ErrorCodeID,
                                           (a2, b) => b.Description ?? "")),
                                 NG_Rate = g.Max(x => x.NG_Rate)
                             }).ToList();

            // Filter base query
            var reportsQuery = _context.UV_IQC_Reports
                .Where(r => r.Status == "REJECTED")
                .AsEnumerable() // 💡 Convert EF query to in-memory to support join with errorData
                .Join(errorData,
                    r => r.ReportID,
                    e => e.ReportID,
                    (r, sub) => new IQCRejectedReportViewModel
                    {
                        ReportID = r.ReportID,
                        LottagId = r.LottagId,
                        VendorName = r.VendorName,
                        VendorCode = r.VendorCode,
                        PartName = r.PartName,
                        PartCode = r.PartCode,
                        PO_NO = r.PO_NO,
                        InvoiceNo = r.InvoiceNo,
                        POQty = r.POQty,
                        POCount = r.POCount,
                        InspectionDate = r.InspectionDate,
                        InspectionGroupID = r.InspectionGroupID,
                        FinalJudgment = r.FinalJudgment,
                        Status = r.Status,
                        CheckerStatus = r.CheckerStatus,
                        Remark = r.Remark,
                        CreatedBy = r.CreatedBy,
                        CreatedDate = r.CreatedDate,
                        UpdatedBy = r.UpdatedBy,
                        UpdatedDate = r.UpdatedDate,
                        Notes = r.Notes,
                        NotesReturn = r.NotesReturn,
                        TextRemark = r.TextRemark,
                        CombinedErrorDescriptions = sub.CombinedErrorDescriptions,
                        NG_Rate = sub.NG_Rate
                    });

            // Apply date range filter if provided
            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                reportsQuery = reportsQuery.Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= inclusiveEndDate);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                reportsQuery = reportsQuery.Where(r =>
                (r.VendorName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.VendorCode ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.PartName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.PartCode ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.PO_NO ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.InvoiceNo ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.LottagId ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.ReportID ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            }

            if (reportsQuery == null || !reportsQuery.Any())
            {
                // Nếu không có dữ liệu, trả về view với thông báo
                ViewBag.Message = "Không có dữ liệu nào trong khoảng thời gian này.";
                return View(new DownloadButtonModel());
            }
            // Define column mappings for user-friendly headers
            //    var columnMappings = new Dictionary<string, string>
            //{
            //    { "Id", "Number" },
            //    { "Model", "Model" },
            //    { "LotNo", "LotNo" },
            //    { "Line", "Line" },
            //    { "QRCode", "QR Code" },
            //    { "CreatedDate", "Created Date" },
            //    { "CreatedBy", "Created By" },

            //};

            //var bytes = _excelExportService.DownloadExportToExcel(results, columnMappings);
            var bytes = _excelExportService.DownloadExportToExcel(reportsQuery, null);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"WHSSortingList_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcelSortingMonitorReport(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var rawList = (from s in _context.UV_IQC_WHS_SORTINGs
                           join r in _context.UV_IQC_Reports on s.ReportID equals r.ReportID into grp
                           from r in grp.DefaultIfEmpty()
                           where (';' + r.Remark + ';').Contains(";" + s.SLottagId + ";")
                           select new
                           {
                               Sorting = s,
                               Report = r
                           }).ToList(); // <--- now it's in memory, join-able with errorData
            // Get errorData first
            var errorData = (from a1 in _context.UV_IQC_ReportItems
                             where a1.ErrorCodeID > 0 && a1.ErrorCodeID != 4
                             group a1 by a1.ReportID into g
                             select new
                             {
                                 ReportID = g.Key,
                                 CombinedErrorDescriptions = string.Join(", ", g
                                     .Join(_context.UV_IQC_ErrorsItemMasters,
                                           a2 => a2.ErrorCodeID,
                                           b => b.ErrorCodeID,
                                           (a2, b) => b.Description ?? "")),
                                 NG_Rate = g.Max(x => x.NG_Rate)
                             }).ToList();

            // Main query
            var merged = from x in rawList
                         join ed in errorData on x.Sorting.ReportID equals ed.ReportID into edJoin
                         from ed in edJoin.DefaultIfEmpty()
                         select new SortingMonitorReportViewModel
                         {
                             ReportID = x.Report?.ReportID,
                             LottagId = x.Report?.LottagId,
                             VendorName = x.Report?.VendorName,
                             VendorCode = x.Report?.VendorCode,
                             PartName = x.Report?.PartName,
                             PartCode = x.Report?.PartCode,
                             PO_NO = x.Report?.PO_NO,
                             InvoiceNo = x.Report?.InvoiceNo,
                             POQty = x.Report?.POQty ?? 0,
                             POCount = x.Report?.POCount ?? 0,
                             InspectionDate = x.Report?.InspectionDate,
                             Status = x.Report?.Status,
                             CheckerStatus = x.Report?.CheckerStatus,
                             Remark = x.Report?.Remark,
                             CreatedBy = x.Report?.CreatedBy,
                             CreatedDate = x.Report?.CreatedDate,
                             Notes = x.Report?.Notes,
                             NotesReturn = x.Report?.NotesReturn,
                             TextRemark = x.Report?.TextRemark,

                             CombinedErrorDescriptions = ed?.CombinedErrorDescriptions ?? "",
                             NG_Rate = ed?.NG_Rate ?? 0,

                             SortingDate = x.Sorting.SortingDate,
                             SortingBy = x.Sorting.SortingBy,
                             TotalQtyReport = x.Sorting.TotalQtyReport,
                             QtyOK = x.Sorting.QtyOK,
                             QtyNG = x.Sorting.QtyNG,
                             WaitSorting = x.Sorting.WaitSorting,
                             RateSortNG = x.Sorting.RateSortNG,
                             SortingStatus = x.Sorting.SortingStatus,
                             IssueLot = x.Sorting.IssueLot,
                             IssueQty = x.Sorting.IssueQty,
                             SignQ = x.Sorting.SignQ,
                             BalQty = x.Sorting.BalQty,
                             TotalManPower = x.Sorting.TotalManPower,
                             TotalHours = x.Sorting.TotalHours,
                             CostPerHour = x.Sorting.CostPerHour,
                             TotalAM = x.Sorting.TotalAM,
                             NameSort = x.Sorting.NameSort,
                             Stock = x.Sorting.Stock,
                             DateCode = x.Sorting.DateCode,
                             Packing = x.Sorting.Packing,
                             Remark2 = x.Sorting.Remark,
                             SLottagId = x.Sorting.SLottagId,
                             NLottagId = x.Sorting.NLottagId,
                             ReportRemark = x.Sorting.ReportRemark,
                             CreatedBy2 = x.Sorting.CreatedBy,
                             CreatedDate2 = x.Sorting.CreatedDate
                         };
            if (startDate.HasValue)
                merged = merged.Where(x => x.SortingDate >= startDate.Value);

            if (endDate.HasValue)
            {
                var inclusiveTo = endDate.Value.Date.AddDays(1).AddTicks(-1);
                merged = merged.Where(x => x.SortingDate <= inclusiveTo);
            }

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                merged = merged.Where(x =>
                    (x.ReportID ?? "").Contains(SearchTerm) ||
                    (x.LottagId ?? "").Contains(SearchTerm));
            }

            if (merged == null || !merged.Any())
            {
                // Nếu không có dữ liệu, trả về view với thông báo
                ViewBag.Message = "Không có dữ liệu nào trong khoảng thời gian này.";
                return View(new DownloadButtonModel());
            }
            // Define column mappings for user-friendly headers
            //    var columnMappings = new Dictionary<string, string>
            //{
            //    { "Id", "Number" },
            //    { "Model", "Model" },
            //    { "LotNo", "LotNo" },
            //    { "Line", "Line" },
            //    { "QRCode", "QR Code" },
            //    { "CreatedDate", "Created Date" },
            //    { "CreatedBy", "Created By" },

            //};

            //var bytes = _excelExportService.DownloadExportToExcel(results, columnMappings);
            var bytes = _excelExportService.DownloadExportToExcel(merged, null);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ExportToExcelSortingMonitorReport_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        [HttpGet]
        public IActionResult GetDateCodeByRemark(string remark)
        {
            if (string.IsNullOrWhiteSpace(remark))
                return Json("");
            var totalQtyOK = _context.UV_IQC_WHS_SORTINGs
                .Where(x => x.SLottagId == remark)
                .Sum(x => ((int?)x.QtyOK ?? 0) + ((int?)x.QtyNG ?? 0));

            var record = _context.WHS_RECEIVING_TOTALs
                .Where(x => x.ID == remark)
                .Select(x => new
                {
                    dateCode = x.MFG != null ? x.MFG.Value.ToString("yyyy-MM-dd") : "",
                    packing = x.PACKING,
                    qty=x.QTY -totalQtyOK,
                    // Add more fields if needed
                })
                .FirstOrDefault();
            if (record == null)
                return Json(new { });
            return Json(record);
        }

        [HttpPost]
        public IActionResult SaveSortingData(WHSViewModel vm)
        {
            // Logic Khi chọn IQC Report để Sorting thì nó sẽ cho chọn Lottag tương ứng
            // và lottag đó đã có trong WHS_RECEIVING_TOTAL --> Lấy được ra datecode / packing người dùng không phải nhập datecode nữa.
            // Lưu dữ liệu vào WHS_RECEIVING_TOTAL để lấy ID lottag mới
            // Ghi số lượng Qty= QTOK và QTNG=0
            // Update số lượng QTOK và QTNG vào Lottag cũ
            // Giới hạn số lượng OK không vượt quá số lượng qty
            var model = vm.UV_IQC_WHS_SORTING;
            ModelState.Remove("WHSSortingList");

            // Validate Qty input
            if (model == null || (model.QtyOK == 0 && model.QtyNG == 0))
            {
                TempData["ErrorMessage"] = "Qty OK or Qty NG must be greater than 0";
                return RedirectToAction("WHSSortingList");
            }
            // Check Lottag source
            // Lấy dữ liệu cũ củ lottag để cập nhật vào bảng WHS_RECEIVING_TOTAL
            var totalQtyOK = _context.UV_IQC_WHS_SORTINGs
               .Where(x => x.SLottagId == model.SLottagId)
               .Sum(x => (int?)x.QtyOK) ?? 0;

            var existingWHStotal = _context.WHS_RECEIVING_TOTALs.FirstOrDefault(x => x.ID == model.SLottagId);
            if (existingWHStotal == null)
            {
                TempData["ErrorMessage"] = "LottagID not found";
                return RedirectToAction("WHSSortingList");
            }
            var report = _context.UV_IQC_Reports.FirstOrDefault(r => r.ReportID == model.ReportID);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found";
                return RedirectToAction("WHSSortingList");
            }
            if (existingWHStotal.QTY-totalQtyOK < model.QtyOK)
            {
                TempData["ErrorMessage"] = "Qty OK cannot exceed original Lottag quantity";
                return RedirectToAction("WHSSortingList");
            }
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Insert new WHS_RECEIVING_TOTAL
                var newWHS = new WHS_RECEIVING_TOTAL
                {
                    SK_INVOICE = (existingWHStotal.SK_INVOICE?.Replace("_SORTING99", "") ?? "") + "_SORTING99",
                    RECEIVED_DATE = DateTime.Now,
                    MFG = existingWHStotal.MFG,
                    SUPPLIER_INVOICE = existingWHStotal.SUPPLIER_INVOICE,
                    SUPPLIER = existingWHStotal.SUPPLIER,
                    SUPPLIER_CODE = existingWHStotal.SUPPLIER_CODE,
                    SUPPLIER_GROUP = existingWHStotal.SUPPLIER_GROUP,
                    PART_CODE = existingWHStotal.PART_CODE,
                    PART_DESC = existingWHStotal.PART_DESC,
                    SPEC = existingWHStotal.SPEC,
                    PURCHASE_ORDER = existingWHStotal.PURCHASE_ORDER,
                    IQC_STATUS="IQC",
                    QTY = model.QtyOK,
                    QtyOK = 0,
                    QtyNG = 0,
                    PART_LOCATION = existingWHStotal.PART_LOCATION,
                    PACKING = existingWHStotal.PACKING,
                    USE_DATE = existingWHStotal.USE_DATE,
                    LOT_NO = existingWHStotal.LOT_NO,
                    CREATEDATE = DateTime.Now
                };
                _context.WHS_RECEIVING_TOTALs.Add(newWHS);
                _context.SaveChanges();
                _context.Entry(newWHS).Reload(); // Get generated ID

                // Update model for UV_IQC_WHS_SORTING
                model.NLottagId = newWHS.ID;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Unknown";
                model.TotalQtyReport = model.QtyOK + model.QtyNG; 
                model.RateSortNG = Math.Round((decimal)model.QtyNG / model.TotalQtyReport * 100, 2);
                model.TotalAM = model.TotalHours * model.TotalManPower * model.CostPerHour;
                model.WaitSorting = (int)existingWHStotal.QTY-model.TotalQtyReport ;
                model.BalQty = model.QtyOK - (model.IssueQty ?? 0);

                _context.UV_IQC_WHS_SORTINGs.Add(model);

                // Update old lottag qty
                //existingWHStotal.QtyOK = (existingWHStotal.QtyOK ?? 0) + model.QtyOK;
                //existingWHStotal.QtyNG = (existingWHStotal.QtyNG ?? 0) + model.QtyNG;

                _context.SaveChanges();
                transaction.Commit();

                TempData["SuccessMessage"] = "Sorting data saved successfully!";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["ErrorMessage"] = $"Error saving sorting data: {ex.Message}";
            }
            return RedirectToAction("WHSSortingList");
        }

        [HttpGet]
        public IActionResult GetSortingHistory(string reportId)
        {
            var history = _context.UV_IQC_WHS_SORTINGs
                .Where(x => x.ReportID == reportId)
                .OrderByDescending(x => x.SortingDate)
                .Select(x => new {
                    sortingDate = x.SortingDate.ToString("yyyy-MM-dd"),
                    sortingBy = x.SortingBy,
                    qtyOK = x.QtyOK,
                    qtyNG = x.QtyNG,
                    sortingStatus = x.SortingStatus,
                    remark = x.Remark
                })
                .ToList();
            return Json(history);
        }
        [HttpGet]
        public IActionResult SortingMonitorReport(DateTime? startDate, DateTime? endDate, string? SearchTerm,  int page=1, int pageSize=10)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var rawList = (from s in _context.UV_IQC_WHS_SORTINGs
                           join r in _context.UV_IQC_Reports on s.ReportID equals r.ReportID into grp
                           from r in grp.DefaultIfEmpty()
                           where (';' + r.Remark + ';').Contains(";" + s.SLottagId + ";")
                           select new
                           {
                               Sorting = s,
                               Report = r
                           }).ToList(); // <--- now it's in memory, join-able with errorData
            // Get errorData first
            var errorData = (from a1 in _context.UV_IQC_ReportItems
                             where a1.ErrorCodeID > 0 && a1.ErrorCodeID != 4
                             group a1 by a1.ReportID into g
                             select new
                             {
                                 ReportID = g.Key,
                                 CombinedErrorDescriptions = string.Join(", ", g
                                     .Join(_context.UV_IQC_ErrorsItemMasters,
                                           a2 => a2.ErrorCodeID,
                                           b => b.ErrorCodeID,
                                           (a2, b) => b.Description ?? "")),
                                 NG_Rate = g.Max(x => x.NG_Rate)
                             }).ToList();

            // Main query
            var merged = from x in rawList
                         join ed in errorData on x.Sorting.ReportID equals ed.ReportID into edJoin
                         from ed in edJoin.DefaultIfEmpty()
                         select new SortingMonitorReportViewModel
                         {
                             ReportID = x.Report?.ReportID,
                             LottagId = x.Report?.LottagId,
                             VendorName = x.Report?.VendorName,
                             VendorCode = x.Report?.VendorCode,
                             PartName = x.Report?.PartName,
                             PartCode = x.Report?.PartCode,
                             PO_NO = x.Report?.PO_NO,
                             InvoiceNo = x.Report?.InvoiceNo,
                             POQty = x.Report?.POQty ?? 0,
                             POCount = x.Report?.POCount ?? 0,
                             InspectionDate = x.Report?.InspectionDate,
                             Status = x.Report?.Status,
                             CheckerStatus = x.Report?.CheckerStatus,
                             Remark = x.Report?.Remark,
                             CreatedBy = x.Report?.CreatedBy,
                             CreatedDate = x.Report?.CreatedDate,
                             Notes = x.Report?.Notes,
                             NotesReturn = x.Report?.NotesReturn,
                             TextRemark = x.Report?.TextRemark,

                             CombinedErrorDescriptions = ed?.CombinedErrorDescriptions ?? "",
                             NG_Rate = ed?.NG_Rate ?? 0,

                             SortingDate = x.Sorting.SortingDate,
                             SortingBy = x.Sorting.SortingBy,
                             TotalQtyReport = x.Sorting.TotalQtyReport,
                             QtyOK = x.Sorting.QtyOK,
                             QtyNG = x.Sorting.QtyNG,
                             WaitSorting = x.Sorting.WaitSorting,
                             RateSortNG = x.Sorting.RateSortNG,
                             SortingStatus = x.Sorting.SortingStatus,
                             IssueLot = x.Sorting.IssueLot,
                             IssueQty = x.Sorting.IssueQty,
                             SignQ = x.Sorting.SignQ,
                             BalQty = x.Sorting.BalQty,
                             TotalManPower = x.Sorting.TotalManPower,
                             TotalHours = x.Sorting.TotalHours,
                             CostPerHour = x.Sorting.CostPerHour,
                             TotalAM = x.Sorting.TotalAM,
                             NameSort = x.Sorting.NameSort,
                             Stock = x.Sorting.Stock,
                             DateCode = x.Sorting.DateCode,
                             Packing = x.Sorting.Packing,
                             Remark2 = x.Sorting.Remark,
                             SLottagId = x.Sorting.SLottagId,
                             NLottagId = x.Sorting.NLottagId,
                             ReportRemark = x.Sorting.ReportRemark,
                             CreatedBy2 = x.Sorting.CreatedBy,
                             CreatedDate2 = x.Sorting.CreatedDate
                         };
            if (startDate.HasValue)
                merged = merged.Where(x => x.SortingDate >= startDate.Value);

            if (endDate.HasValue)
            {
                var inclusiveTo = endDate.Value.Date.AddDays(1).AddTicks(-1);
                merged = merged.Where(x => x.SortingDate <= inclusiveTo);
            }

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                merged = merged.Where(x =>
                    (x.ReportID ?? "").Contains(SearchTerm) ||
                    (x.LottagId ?? "").Contains(SearchTerm));
            }

            var reports = merged
                .OrderByDescending(x => x.SortingDate)
                .AsQueryable()
                .ToPagedResult(page, pageSize);
            var viewModel = new WHSViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                SortingMonitorList = reports
            };

            ViewBag.FromDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchText = SearchTerm ?? "";

            if (reports == null || !reports.Items.Any())
            {
                ViewBag.Message = _translationService.GetTranslation("NotFoundData", languageCode);
            }
            return View(viewModel);            
        }
    }
}
