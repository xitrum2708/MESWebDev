using MESWebDev.Data;
using MESWebDev.DTO;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.SMT;
using MESWebDev.Models.TELSTAR;
using MESWebDev.Models.TELSTAR.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.Drawing;

namespace MESWebDev.Controllers
{
    public class TelstarAssyController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IConfiguration _configuration;
        private readonly DownloadExcelExportService _excelExportService;

        public TelstarAssyController(AppDbContext context, ITranslationService translationService, IConfiguration configuration, DownloadExcelExportService excelExportService)
            : base(context)
        {
            _translationService = translationService;
            _configuration = configuration;
            _excelExportService = excelExportService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var qrCodesTodayByUser = _context.TELSTAR_ASSYs
                .Where(t => t.CreatedBy == User.Identity.Name &&
                            t.CreatedDate >= today &&
                            t.CreatedDate < tomorrow)
                .Count();

            var viewModel = new TELSTAR_ASSY_VM
            {
                LotNoList = _context.UV_SPO_MASTER_ALLs.Where(r => r.LotNo.Contains("OD"))
                    .Select(l => new SelectListItem
                    {
                        Value = l.LotNo,
                        Text = l.LotNo
                    }).ToList(),

                LineList = _context.UV_PRO_LINEs
                    .Select(l => new SelectListItem
                    {
                        Value = l.LineName,
                        Text = l.LineName
                    }).ToList(),

                QRCodesTodayByUser = qrCodesTodayByUser,
                // Add existing data for the user for today

                //ScannedData = _context.TELSTAR_ASSYs
                //.Where(t => t.CreatedBy == User.Identity.Name &&
                //            t.CreatedDate >= today &&
                //            t.CreatedDate < tomorrow)
                //.OrderByDescending(t => t.CreatedDate) // Latest first
                //.Select(t => new TelstarAssyVM
                //{
                //    SelectedLotNo = t.LotNo,
                //    Model = t.Model,
                //    SelectedLine = t.Line,
                //    QRCode = t.QRCode,
                //    CreatedDate = t.CreatedDate
                //})
                //.ToList()
            };

            ViewBag.CurrentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetDataByLotNo(string lotNo)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var data = _context.TELSTAR_ASSYs
                .Where(t => t.CreatedBy == User.Identity.Name &&
                            t.LotNo == lotNo &&
                            t.CreatedDate >= today &&
                            t.CreatedDate < tomorrow)
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => new
                {
                    SelectedLotNo = t.LotNo,
                    Model = t.Model,
                    SelectedLine = t.Line,
                    QRCode = t.QRCode,
                    CreatedDate = t.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToList();

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TelstarAssyDto formData)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            Console.WriteLine("Raw Form Data:");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"  {key}: {Request.Form[key]}");
            }

            Console.WriteLine($"Bound DTO: LotNo={formData?.SelectedLotNo}, Model={formData?.Model}, Line={formData?.SelectedLine}, QRCode={formData?.QRCode}");

            var model = new TELSTAR_ASSY_VM
            {
                SelectedLotNo = formData?.SelectedLotNo,
                Model = formData?.Model,
                SelectedLine = formData?.SelectedLine,
                QRCode = formData?.QRCode,
                LotNoList = _context.UV_SPO_MASTER_ALLs.Where(r => r.LotNo.Contains("OD"))
                    .Select(l => new SelectListItem { Value = l.LotNo, Text = l.LotNo }).ToList(),
                LineList = _context.UV_PRO_LINEs
                    .Select(l => new SelectListItem { Value = l.LineName, Text = l.LineName }).ToList()
            };

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"  {error.ErrorMessage}");
                }
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, errors });
                }
                return View(model);
            }

            try
            {
                _context.Database.SetCommandTimeout(120);

                bool qrExists = _context.TELSTAR_ASSYs.Any(t => t.QRCode == formData.QRCode);
                if (qrExists)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, errors = new[] { "QRCode đã có trong cơ sở dữ liệu." } });
                    }
                    ModelState.AddModelError("QRCode", "QRCode đã có trong cơ sở dữ liệu.");
                    return View(model);
                }

                var stopwatch = Stopwatch.StartNew();
                bool qrInSmtOutput = _context.tbl_EstechSerialGenerals
                    .AsNoTracking()
                    .Any(e => e.Serial_Number == formData.QRCode);
                stopwatch.Stop();
                Console.WriteLine($"QRCode query took {stopwatch.ElapsedMilliseconds} ms");

                if (!qrInSmtOutput)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, errors = new[] { "QRCode này chưa được đăng ký." } });
                    }
                    ModelState.AddModelError("QRCode", "QRCode này chưa được đăng ký.");
                    return View(model);
                }

                var telstarAssy = new TELSTAR_ASSY_Model
                {
                    LotNo = formData.SelectedLotNo,
                    Model = formData.Model,
                    Line = formData.SelectedLine,
                    QRCode = formData.QRCode,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _context.TELSTAR_ASSYs.Add(telstarAssy);
                await _context.SaveChangesAsync();

                // Fetch the updated total QR codes for this LotNo
                var totalQRCodesForLot = _context.TELSTAR_ASSYs
                    .Count(t => t.LotNo == formData.SelectedLotNo);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = true,
                        message = _translationService.GetTranslation("SavedSuccessfully", languageCode),
                        totalQRCodesForLot,
                        createdDate = telstarAssy.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Query Exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, errors = new[] { "Database query failed: " + ex.Message } });
                }
                ModelState.AddModelError("", "Database query failed.");
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetModelByLotNo(string lotNo)
        {
            var result = _context.UV_SPO_MASTER_ALLs
                .Where(l => l.LotNo == lotNo)
                .Select(l => new
                {
                    Model = l.Model,
                    LotSize = l.LotSize // Assuming LotSize exists in UV_SPO_MASTER_ALLs
                })
                .FirstOrDefault();

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetTotalQRCodesForLot(string lotNo)
        {
            if (string.IsNullOrEmpty(lotNo))
            {
                return Json(0);
            }

            var total = _context.TELSTAR_ASSYs
                .Count(t => t.LotNo == lotNo);

            return Json(total);
        }

        public async Task<IActionResult> ReportTelStar(DateTime? startDate,
                                                       DateTime? endDate,
                                                       string? SearchTerm,
                                                       int page = 1, int pageSize = 10)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var telstarListPro = _context.TELSTAR_ASSYs
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                telstarListPro = telstarListPro.Where(x => x.QRCode.Contains(SearchTerm) ||
                                                           x.Model.Contains(SearchTerm) ||
                                                           x.LotNo.Contains(SearchTerm));
            }

            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                telstarListPro = telstarListPro.Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= inclusiveEndDate);
            }

            var resultPage = telstarListPro
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new TelstarAssyVM
                {
                    SelectedLotNo = x.LotNo,
                    Model = x.Model,
                    SelectedLine = x.Line,
                    QRCode = x.QRCode,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy
                })
                .ToPagedResult(page, pageSize);

            var viewModel = new TELSTAR_ASSY_ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                TelstarVM = resultPage
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
        public async Task<IActionResult> ReportTelStarDQC(DateTime? startDate,
                                                       DateTime? endDate,
                                                       string? SearchTerm,
                                                       int page = 1, int pageSize = 10)
        {

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var telstarListPro = _context.TELSTAR_DQCs
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                telstarListPro = telstarListPro.Where(x => x.QRCode.Contains(SearchTerm) ||
                                                           x.Model.Contains(SearchTerm) ||
                                                           x.LotNo.Contains(SearchTerm));
            }

            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                telstarListPro = telstarListPro.Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= inclusiveEndDate);
            }

            var resultPage = telstarListPro
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new TelstarDQCVM
                {
                    LotNo = x.LotNo,
                    Status = x.Status,
                    Remark = x.Remark,
                    CreatedBy = x.CreatedBy,
                    Model = x.Model,
                    QRCode = x.QRCode,
                    CreatedDate = x.CreatedDate
                })
                .ToPagedResult(page, pageSize);

            var viewModel = new TELSTAR_ASSY_ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                TelstarDQCVM = resultPage
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
        public async Task<IActionResult> ExportToExcel(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
            var results = _context.TELSTAR_ASSYs
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
            var bytes = _excelExportService.DownloadExportToExcel(results, null);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Telstar_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcelDQC(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
           
            var results = _context.TELSTAR_DQCs
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
            var bytes = _excelExportService.DownloadExportToExcel(results, null);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Telstar_DQC_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet]
        public IActionResult CreateTelStarDQC()
        {
            var usrName = User.Identity?.Name ?? "system";
            var list = _context.TELSTAR_DQCs.Where(r => r.CreatedBy == usrName).OrderByDescending(x => x.CreatedDate).Take(100).ToList();
            ViewBag.RecentEntries = list;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTelStarDQC(TELSTAR_DQC_VM model)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            ModelState.Remove("CreatedDate");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Remark");
            ModelState.Remove("Model");
            ModelState.Remove("LotNo");
            if (!ModelState.IsValid)
                return BadRequest(_translationService.GetTranslation("Invaliddata", languageCode));

            var exists = await _context.TELSTAR_ASSYs.Where(x => x.QRCode == model.QRCode).FirstOrDefaultAsync();
            if (exists == null)
                return BadRequest(_translationService.GetTranslation("QRCodeNotFound", languageCode));
            //return BadRequest("QRCode not found in TELSTAR_ASSY.");

            var existingDqc = await _context.TELSTAR_DQCs.FirstOrDefaultAsync(x => x.QRCode == model.QRCode);
            bool isUpdate = existingDqc != null;
            if (existingDqc != null)
            {
                existingDqc.Status = model.Status;
                existingDqc.Remark = model.Status == "NG" ? model.Remark : null;
                existingDqc.UpdateDate = DateTime.Now;
                existingDqc.CreatedBy = User.Identity?.Name ?? "system";

                _context.TELSTAR_DQCs.Update(existingDqc);
            }
            else
            {
                var dqc = new TELSTAR_DQC
                {
                    QRCode = model.QRCode,
                    Status = model.Status,
                    Remark = model.Status == "NG" ? model.Remark : null,
                    Model = exists.Model,
                    LotNo = exists.LotNo,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "system"
                };

                _context.TELSTAR_DQCs.Add(dqc);
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
                    lotNo = exists.LotNo,
                    createdDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                }
            });
        }

        [HttpGet]
        public IActionResult CreateTonlyDQC()
        {
            var usrName = User.Identity?.Name ?? "system";
            var list = _context.TONLY_DQCs.Where(r => r.CreatedBy == usrName).OrderByDescending(x => x.CreatedDate).Take(100).ToList();
            ViewBag.RecentEntries = list;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTonlyDQC(EASTECH_OQC_VM model)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            ModelState.Remove("CreatedDate");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Remark");
            ModelState.Remove("Model");
            ModelState.Remove("LotNo");
            ModelState.Remove("PCBCode");
            if (!ModelState.IsValid)
                return BadRequest(_translationService.GetTranslation("Invaliddata", languageCode));

            var exists = await _context.tbl_EstechSerialGenerals.Where(x => x.Serial_Number == model.QRCode).FirstOrDefaultAsync();
            if (exists == null)
                return BadRequest(_translationService.GetTranslation("QRCodeNotFound", languageCode));
            //return BadRequest("QRCode not found in TELSTAR_ASSY.");

            var existingDqc = await _context.TONLY_DQCs.FirstOrDefaultAsync(x => x.QRCode == model.QRCode);
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
                _context.TONLY_DQCs.Update(existingDqc);
            }
            else
            {
                var dqc = new TONLY_DQC
                {
                    QRCode = model.QRCode,
                    Status = model.Status,
                    Checker = model.Checker,
                    Remark = model.Status == "NG" ? model.Remark : null,
                    Model = exists.Model,
                    LotNo = exists.Lot_no,
                    PCBCode = exists.PCBCode,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "system"
                };

                _context.TONLY_DQCs.Add(dqc);
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
                    checker=model.Checker,
                    remark = model.Remark,
                    model = exists.Model,
                    lotNo = exists.Lot_no,
                    createdDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                }
            });
        }
        private static void SetExcelRowError(
    ExcelWorksheet worksheet,
    int row,
    int resultColumn,
    string errorMessage)
        {
            var resultCell = worksheet.Cells[row, resultColumn];

            resultCell.Value = errorMessage;
            resultCell.Style.Font.Color.SetColor(Color.DarkRed);
            resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            resultCell.Style.Fill.BackgroundColor.SetColor(Color.LightPink);
            resultCell.Style.WrapText = true;

            // Tô màu 3 cột dữ liệu bị lỗi
            var dataRange = worksheet.Cells[row, 1, row, 3];

            dataRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            dataRange.Style.Fill.BackgroundColor.SetColor(Color.MistyRose);
        }

        private static void SetExcelRowSuccess(
            ExcelWorksheet worksheet,
            int row,
            int resultColumn,
            string message)
        {
            var resultCell = worksheet.Cells[row, resultColumn];

            resultCell.Value = message;
            resultCell.Style.Font.Color.SetColor(Color.DarkGreen);
            resultCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            resultCell.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
        }

        private sealed class TonlyDqcImportRow
        {
            public int ExcelRow { get; set; }

            public string Qrcode { get; set; } = string.Empty;

            public string Status { get; set; } = string.Empty;

            public string? RemarkAfterCheck { get; set; }

            public string? ErrorMessage { get; set; }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDataRemarkStatus(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file Excel.";
                return RedirectToAction("ReportTonlyDQC");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension != ".xlsx")
            {
                TempData["Error"] = "Chỉ hỗ trợ file Excel định dạng .xlsx.";
                return RedirectToAction("ReportTonlyDQC");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                await using var inputStream = new MemoryStream();
                await file.CopyToAsync(inputStream);
                inputStream.Position = 0;

                using var package = new ExcelPackage(inputStream);

                if (package.Workbook.Worksheets.Count == 0)
                {
                    TempData["Error"] = "File Excel không có worksheet.";
                    return RedirectToAction("ReportTonlyDQC");
                }

                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                {
                    TempData["Error"] = "File Excel không có dữ liệu để import.";
                    return RedirectToAction("ReportTonlyDQC");
                }

                var rowCount = worksheet.Dimension.Rows;

                const int qrcodeColumn = 1;
                const int statusColumn = 2;
                const int remarkColumn = 3;
                const int resultColumn = 4;

                // Thêm cột kết quả import
                worksheet.Cells[1, resultColumn].Value = "Import Result";

                worksheet.Cells[1, resultColumn].Style.Font.Bold = true;
                worksheet.Cells[1, resultColumn].Style.Fill.PatternType =
                    ExcelFillStyle.Solid;
                worksheet.Cells[1, resultColumn].Style.Fill.BackgroundColor.SetColor(
                    Color.LightBlue
                );

                var excelRows = new List<TonlyDqcImportRow>();

                // Dùng để kiểm tra QR code bị trùng trong file
                var duplicateChecker = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase
                );

                var failedCount = 0;
                var updatedCount = 0;

                for (var row = 2; row <= rowCount; row++)
                {
                    var qrcode = worksheet.Cells[row, qrcodeColumn]
                        .Text
                        .Trim();

                    var status = worksheet.Cells[row, statusColumn]
                        .Text
                        .Trim()
                        .ToUpperInvariant();

                    var remark = worksheet.Cells[row, remarkColumn]
                        .Text
                        .Trim();

                    // Bỏ qua dòng hoàn toàn trống
                    if (string.IsNullOrWhiteSpace(qrcode)
                        && string.IsNullOrWhiteSpace(status)
                        && string.IsNullOrWhiteSpace(remark))
                    {
                        continue;
                    }

                    var importRow = new TonlyDqcImportRow
                    {
                        ExcelRow = row,
                        Qrcode = qrcode,
                        Status = status,
                        RemarkAfterCheck = remark
                    };

                    // Validate QR code
                    if (string.IsNullOrWhiteSpace(qrcode))
                    {
                        importRow.ErrorMessage = "Qrcode không được để trống.";
                        failedCount++;

                        SetExcelRowError(
                            worksheet,
                            row,
                            resultColumn,
                            importRow.ErrorMessage
                        );

                        excelRows.Add(importRow);
                        continue;
                    }

                    // Validate Status
                    if (status != "OK" && status != "NG")
                    {
                        importRow.ErrorMessage =
                            $"Status '{status}' không hợp lệ. Chỉ được nhập OK hoặc NG.";

                        failedCount++;

                        SetExcelRowError(
                            worksheet,
                            row,
                            resultColumn,
                            importRow.ErrorMessage
                        );

                        excelRows.Add(importRow);
                        continue;
                    }

                    // Kiểm tra QR code trùng trong cùng file Excel
                    if (!duplicateChecker.Add(qrcode))
                    {
                        importRow.ErrorMessage =
                            $"Qrcode '{qrcode}' bị trùng trong file Excel.";

                        failedCount++;

                        SetExcelRowError(
                            worksheet,
                            row,
                            resultColumn,
                            importRow.ErrorMessage
                        );

                        excelRows.Add(importRow);
                        continue;
                    }

                    excelRows.Add(importRow);
                }

                // Chỉ lấy các dòng chưa có lỗi validation
                var validRows = excelRows
                    .Where(x => string.IsNullOrWhiteSpace(x.ErrorMessage))
                    .ToList();

                if (validRows.Count > 0)
                {
                    var qrcodes = validRows
                        .Select(x => x.Qrcode)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    /*
                     * SQL Server có giới hạn số lượng parameter.
                     * Chia nhỏ danh sách để tránh lỗi nếu file có nhiều QR code.
                     */
                    const int batchSize = 1000;

                    var databaseRecords = new List<TONLY_DQC>();

                    for (var index = 0; index < qrcodes.Count; index += batchSize)
                    {
                        var qrcodeBatch = qrcodes
                            .Skip(index)
                            .Take(batchSize)
                            .ToList();

                        var records = await _context.TONLY_DQCs
                            .Where(x =>
                                x.QRCode != null &&
                                qrcodeBatch.Contains(x.QRCode)
                            )
                            .ToListAsync();

                        databaseRecords.AddRange(records);
                    }

                    // Nếu database có thể chứa Qrcode trùng,
                    // cần kiểm tra và báo lỗi thay vì update không rõ bản ghi nào.
                    var recordsByQrcode = databaseRecords
                        .Where(x => !string.IsNullOrWhiteSpace(x.QRCode))
                        .GroupBy(
                            x => x.QRCode!.Trim(),
                            StringComparer.OrdinalIgnoreCase
                        )
                        .ToDictionary(
                            group => group.Key,
                            group => group.ToList(),
                            StringComparer.OrdinalIgnoreCase
                        );

                    foreach (var excelRow in validRows)
                    {
                        if (!recordsByQrcode.TryGetValue(
                                excelRow.Qrcode,
                                out var matchedRecords))
                        {
                            excelRow.ErrorMessage =
                                $"Không tìm thấy Qrcode '{excelRow.Qrcode}' trong database.";

                            failedCount++;

                            SetExcelRowError(
                                worksheet,
                                excelRow.ExcelRow,
                                resultColumn,
                                excelRow.ErrorMessage
                            );

                            continue;
                        }

                        if (matchedRecords.Count > 1)
                        {
                            excelRow.ErrorMessage =
                                $"Qrcode '{excelRow.Qrcode}' có nhiều hơn một bản ghi trong database.";

                            failedCount++;

                            SetExcelRowError(
                                worksheet,
                                excelRow.ExcelRow,
                                resultColumn,
                                excelRow.ErrorMessage
                            );

                            continue;
                        }

                        var record = matchedRecords[0];

                        record.Status = excelRow.Status;
                        record.RemarkAfterCheck = excelRow.RemarkAfterCheck;

                        updatedCount++;

                        SetExcelRowSuccess(
                            worksheet,
                            excelRow.ExcelRow,
                            resultColumn,
                            "Updated successfully"
                        );
                    }

                    if (updatedCount > 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                worksheet.Cells[worksheet.Dimension.Address]
                    .AutoFitColumns();

                worksheet.Column(resultColumn).Width = 55;

                // Có ít nhất một dòng không import được:
                // trả lại file Excel đã ghi lỗi.
                if (failedCount > 0)
                {
                    var outputStream = new MemoryStream();
                    await package.SaveAsAsync(outputStream);

                    outputStream.Position = 0;

                    var originalFileName =
                        Path.GetFileNameWithoutExtension(file.FileName);

                    var outputFileName =
                        $"{originalFileName}_ImportResult_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    Response.Headers.Append(
                        "X-Import-Message",
                        $"Updated: {updatedCount}; Failed: {failedCount}"
                    );

                    return File(
                        outputStream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        outputFileName
                    );
                }

                TempData["Success"] =
                    $"Import thành công. Đã cập nhật {updatedCount} bản ghi.";

                return RedirectToAction("ReportTonlyDQC");
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] =
                    $"Không thể cập nhật database: {ex.InnerException?.Message ?? ex.Message}";

                return RedirectToAction("ReportTonlyDQC");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"Có lỗi khi xử lý file Excel: {ex.Message}";

                return RedirectToAction("ReportTonlyDQC");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReportTonlyDQC(DateTime? startDate,
                                                       DateTime? endDate,
                                                       string? SearchTerm,
                                                       int page = 1, int pageSize = 10)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var tonlyListPro = _context.TONLY_DQCs
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                tonlyListPro = tonlyListPro.Where(x => x.QRCode.Contains(SearchTerm) ||
                                                           x.Model.Contains(SearchTerm) ||
                                                           x.LotNo.Contains(SearchTerm));
            }

            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                tonlyListPro = tonlyListPro.Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= inclusiveEndDate);
            }

            var resultPage = tonlyListPro
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new TonlyDQCVM
                {
                    LotNo = x.LotNo,
                    PCBCode = x.PCBCode,
                    Status = x.Status,
                    Checker= x.Checker,
                    Remark = x.Remark,
                    RemarkAfterCheck=x.RemarkAfterCheck,                    
                    Model = x.Model,
                    QRCode = x.QRCode,
                    CreatedDate = x.CreatedDate
                })
                .ToPagedResult(page, pageSize);

            var viewModel = new TELSTAR_ASSY_ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                TonlyDQCVM = resultPage
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
        public async Task<IActionResult> TonlyExportToExcelDQC(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var results = _context.TONLY_DQCs
                .AsQueryable()
                .Select(x => new
                {
                    x.Checker,
                    x.QRCode,
                    x.Status,
                    x.Model,
                    x.LotNo,
                    x.PCBCode,
                    x.Remark,
                    x.RemarkAfterCheck,
                    x.CreatedDate,                    
                    x.UpdateDate
                });

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
                        $"Tonly_DQC_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet]
        public IActionResult CreateTonlyDQC_CTN()
        {
            var usrName = User.Identity?.Name ?? "system";
            var list = _context.TONLY_DQC_CTNs.Where(r => r.CreatedBy == usrName).OrderByDescending(x => x.CreatedAt).Take(100).ToList();
            ViewBag.RecentEntries = list;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTonlyDQC_CTN(TONLY_DQC_CTN model)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Remark");
            ModelState.Remove("Model");
            ModelState.Remove("LotNo");            
            if (!ModelState.IsValid)
                return BadRequest(_translationService.GetTranslation("Invaliddata", languageCode));
            string result = model.QRCodeCTN.Split('*')[3].Substring(0, 13);
            var exists = await _context.UV_SPO_MASTER_ALLs.Where(x => x.LotSMT == result).FirstOrDefaultAsync();
            if (exists == null)
                return BadRequest(_translationService.GetTranslation("LotNoNotFound", languageCode));
            //return BadRequest("QRCode not found in TELSTAR_ASSY.");

            var existingDqc = await _context.TONLY_DQC_CTNs.FirstOrDefaultAsync(x => x.QRCodeCTN == model.QRCodeCTN);
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
                existingDqc.UpdatedAt = DateTime.Now;
                existingDqc.UpdatedBy = User.Identity?.Name ?? "system";
                existingDqc.CreatedBy = User.Identity?.Name ?? "system";
                _context.TONLY_DQC_CTNs.Update(existingDqc);
            }
            else
            {
                var dqc = new TONLY_DQC_CTN
                {
                    QRCodeCTN = model.QRCodeCTN,
                    Status = model.Status,
                    Checker = model.Checker,
                    Remark = model.Status == "NG" ? model.Remark : null,
                    Model = exists.BModel,
                    LotNo = exists.LotSMT,                    
                    CreatedAt = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "system"
                };

                _context.TONLY_DQC_CTNs.Add(dqc);
            }
            await _context.SaveChangesAsync();
            return Json(new
            {
                success = true,
                isUpdate = isUpdate,
                message = _translationService.GetTranslation("SavedSuccessfully", languageCode),
                data = new
                {
                    qrCode = model.QRCodeCTN,
                    status = model.Status,
                    checker = model.Checker,
                    remark = model.Remark,
                    model = exists.BModel,
                    lotNo = exists.LotSMT,
                    createdAt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ReportTonlyDQC_CTN(DateTime? startDate,
                                                       DateTime? endDate,
                                                       string? SearchTerm,
                                                       int page = 1, int pageSize = 10)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var tonlyListPro = _context.TONLY_DQC_CTNs
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                tonlyListPro = tonlyListPro.Where(x => x.QRCodeCTN.Contains(SearchTerm) ||
                                                           x.Model.Contains(SearchTerm) ||
                                                           x.LotNo.Contains(SearchTerm));
            }

            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                tonlyListPro = tonlyListPro.Where(x => x.CreatedAt >= startDate.Value && x.CreatedAt <= inclusiveEndDate);
            }

            var resultPage = tonlyListPro
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TonlyDQC_CTNVM
                {
                    LotNo = x.LotNo,                    
                    Status = x.Status,
                    Checker = x.Checker,
                    Remark = x.Remark,
                    CreatedBy = x.CreatedBy,
                    Model = x.Model,
                    QRCodeCTN = x.QRCodeCTN,
                    CreatedAt = x.CreatedAt
                })
                .ToPagedResult(page, pageSize);

            var viewModel = new TELSTAR_ASSY_ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                TonlyDQC_CTNVM= resultPage
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
        public async Task<IActionResult> TonlyExportToExcelDQC_CTN(DateTime? startDate, DateTime? endDate, string? SearchTerm)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var results = _context.TONLY_DQC_CTNs
                .AsQueryable()
                .Select(x=>new{ 
                x.Checker,
                x.QRCodeCTN,
                x.Status,
                x.Remark,
                x.Model,
                x.LotNo,
                x.CreatedAt                
                });

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                results = results.Where(x => x.QRCodeCTN.Contains(SearchTerm) ||
                                                           x.Model.Contains(SearchTerm) ||
                                                           x.LotNo.Contains(SearchTerm));
            }

            if (startDate.HasValue && endDate.HasValue && endDate.Value >= startDate.Value)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);
                results = results.Where(x => x.CreatedAt >= startDate.Value.Date && x.CreatedAt < endExclusive);
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
                        $"Tonly_DQC_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}