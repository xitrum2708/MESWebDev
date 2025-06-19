using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class IQCReportsController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        private readonly IIQCReportsRepository _repository;
        private readonly ILoggingService _loggingService;
        private readonly IExcelExportService _excelExportService;

        public IQCReportsController(AppDbContext context, ITranslationService translationService, IIQCReportsRepository repository,
            ILoggingService loggingService, IExcelExportService excelExportService)
            : base(context)
        {
            // _context = context;
            _translationService = translationService;
            _repository = repository;
            _loggingService = loggingService;
            _excelExportService = excelExportService;
        }

        // GET: Permission/Index
        public async Task<IActionResult> Index(string VendorCode, string PartCode, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 10)
        {
            var query = _context.UV_IQC_Reports.AsQueryable();

            if (!string.IsNullOrEmpty(VendorCode))
            {
                query = query.Where(r => r.VendorCode.Contains(VendorCode));
            }

            if (!string.IsNullOrEmpty(PartCode))
            {
                query = query.Where(r => r.PartCode == PartCode);
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.InspectionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.InspectionDate <= endDate.Value);
            }

            // Tính tổng số mục
            var totalItems = await query.CountAsync();

            // Lấy dữ liệu theo trang, sắp xếp ví dụ theo CreatedDate
            var reports = await query
                .Include(r => r.InspectionGroups)
                .OrderBy(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new ReportVM
                {
                    ReportID = m.ReportID,
                    LottagId = m.LottagId,
                    VendorCode = m.VendorCode,
                    VendorName = m.VendorName,
                    PartCode = m.PartCode,
                    PartName = m.PartName,
                    PO_NO = m.PO_NO,
                    InvoiceNo = m.InvoiceNo,
                    POQty = m.POQty,
                    POCount = m.POCount,
                    InspectionDate = m.InspectionDate,
                    InspectionGroupID = m.InspectionGroupID,
                    GroupName = m.InspectionGroups.GroupName,
                    FinalJudgment = m.FinalJudgment,
                    Status = m.Status,
                    CheckerStatus = m.CheckerStatus,
                    Remark = m.Remark,
                    CreatedBy = m.CreatedBy,
                    CreatedDate = m.CreatedDate,
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    Notes = m.Notes,
                    NotesReturn = m.NotesReturn,
                    TextRemark = m.TextRemark,
                })
                .ToListAsync();

            // Tạo đối tượng PagedResult
            var pagedReports = new PagedResult<ReportVM>
            {
                Items = reports,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize
            };

            var viewModel = new ReportIQCViewModel
            {
                ReportIQC = pagedReports,
                StartDate = startDate,
                EndDate = endDate,
                VenderCode = VendorCode,
                Partcode = PartCode,
            };

            return View(viewModel);
        }

        // Action Detail: hiển thị thông tin chi tiết của báo cáo
        public async Task<IActionResult> DetailReport(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ReportID is required");
            }

            // Lấy dữ liệu report từ database, có thể include các thực thể liên quan nếu cần
            var reportEntity = await _context.UV_IQC_Reports
                .FirstOrDefaultAsync(r => r.ReportID == id);

            if (reportEntity == null)
            {
                return NotFound();
            }

            // Ánh xạ dữ liệu sang view model ReportVM
            var viewModel = new ReportVM
            {
                ReportID = reportEntity.ReportID,
                LottagId = reportEntity.LottagId,
                VendorCode = reportEntity.VendorCode,
                VendorName = reportEntity.VendorName,
                PartCode = reportEntity.PartCode,
                PartName = reportEntity.PartName,
                PO_NO = reportEntity.PO_NO,
                InvoiceNo = reportEntity.InvoiceNo,
                POQty = reportEntity.POQty,
                POCount = reportEntity.POCount,
                InspectionDate = reportEntity.InspectionDate,
                InspectionGroupID = reportEntity.InspectionGroupID,
                // Nếu bạn có định nghĩa navigation property cho InspectionGroup, có thể lấy:
                GroupName = _context.UV_IQC_InspectionGroups
                                    .Where(k => k.InspectionGroupID == reportEntity.InspectionGroupID)
                                    .Select(g => g.GroupName)
                                    .FirstOrDefault(),
                FinalJudgment = reportEntity.FinalJudgment,
                Status = reportEntity.Status,
                CheckerStatus = reportEntity.CheckerStatus,
                Remark = reportEntity.Remark,
                CreatedBy = reportEntity.CreatedBy,
                CreatedDate = reportEntity.CreatedDate,
                UpdatedBy = reportEntity.UpdatedBy,
                UpdatedDate = reportEntity.UpdatedDate,
                Notes = reportEntity.Notes,
                NotesReturn = reportEntity.NotesReturn,
                TextRemark = reportEntity.TextRemark
            };

            return View(viewModel);
        }

        // Action để hiển thị biểu đồ Top 2 lỗi (tính từ tổng CRI + MAJ + MIN)
        public async Task<IActionResult> ChartTopErrors(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không truyền startDate hoặc endDate, mặc định lấy dữ liệu của 1 tuần vừa qua
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Giả sử bạn muốn từ 7 ngày trước đến hôm nay
                endDate = DateTime.Today; // hoặc DateTime.Now nếu bạn muốn cả thời gian
                startDate = endDate.Value.AddDays(-7);
            }
            // Lấy dữ liệu từ ReportItems, join với bảng ErrorsItemMaster và Reports
            var query = _context.UV_IQC_ReportItems
                .Include(ri => ri.Reports)              // để lấy VendorCode, PartCode và InspectionDate
                .Include(ri => ri.ErrorsItemMasters)           // để lấy mô tả lỗi (Description)
                .AsQueryable();

            // Lọc theo điều kiện: lỗi (ErrorCodeID) khác 4 và > 0
            query = query.Where(ri => ri.ErrorCodeID != 4 && ri.ErrorCodeID > 0);

            // Lọc theo khoảng thời gian dựa trên InspectionDate trong Reports (có thể dùng CreatedDate nếu cần)
            if (startDate.HasValue)
            {
                query = query.Where(ri => ri.CreatedDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(ri => ri.CreatedDate <= endDate.Value);
            }

            // Nhóm theo lỗi, nhà cung cấp và mã hàng: bạn có thể nhóm theo ErrorCodeID hoặc theo mô tả lỗi
            var groupData = query.GroupBy(ri => new
            {
                ri.ErrorCodeID,
                ErrorDescription = ri.ErrorsItemMasters.Description,
                ri.Reports.VendorCode,
                ri.Reports.PartCode
            })
            .Select(g => new
            {
                ErrorCodeID = g.Key.ErrorCodeID,
                ErrorDescription = g.Key.ErrorDescription,
                VendorCode = g.Key.VendorCode,
                PartCode = g.Key.PartCode,
                TotalError = g.Sum(ri => ri.CRI + ri.MAJ + ri.MIN),
                TotalPOQty = g.Sum(ri => ri.Reports.POQty)
            })
            .AsEnumerable()
            .Select(x => new
            {
                x.ErrorCodeID,
                x.ErrorDescription,
                x.VendorCode,
                x.PartCode,
                x.TotalError,
                x.TotalPOQty,
                ErrorRate = x.TotalPOQty != 0 ? (double)x.TotalError / x.TotalPOQty : 0
            });
            // Lấy top 20 nhóm theo tổng lỗi giảm dần
            var top20 = groupData.OrderByDescending(g => g.TotalError)
                                         .Take(20)
                                         .ToList();

            // Chuẩn bị dữ liệu cho biểu đồ
            // Ví dụ, mỗi nhãn là "VendorCode - PartCode - ErrorDescription"
            var labels = top20.Select(x => $"{x.PartCode} - {x.ErrorDescription}").ToArray();
            var data = top20.Select(x => x.TotalError).ToArray();
            var poQtyData = top20.Select(x => x.TotalPOQty).ToArray();
            var errorRateData = top20.Select(x => x.ErrorRate).ToArray();

            // Tạo view model cho chart
            var chartModel = new ChartTopErrorViewModel
            {
                Labels = labels,
                Data = data,
                POQty = poQtyData,
                ErrorRateData = errorRateData
            };

            return View(chartModel);
        }

        public async Task<IActionResult> IncommingPartInspectionSummaryReport(int Year = 0)
        {
            if (Year == 0)
            {
                Year = DateTime.Now.Year;
            }
            ViewBag.Years = Enumerable.Range(2025, DateTime.Now.Year - 2025 + 1)
                          .OrderByDescending(y => y)
                          .ToList();
            var monthlyReport = await _repository.GetMonthlyRejectRateModelAsync(Year);
            if (monthlyReport == null)
            {
                return NotFound();
            }
            return View(monthlyReport);
        }

        public async Task<IActionResult> IncommingGetMonthlyChartData(int year)
        {
            var monthlyReport = await _repository.GetMonthlyRejectRateModelAsync(year);
            if (monthlyReport == null)
            {
                return NotFound();
            }

            // Giả sử bạn muốn chia dữ liệu theo các nhóm IMPORT và LOCAL.
            // Nếu dữ liệu của bạn gồm nhiều group, bạn có thể chuyển đổi hoặc lọc theo ABBRE_GROUP.
            // Ở đây, ví dụ: nếu có 2 group, "IMPORT" và "LOCAL", bạn lọc riêng:
            var importData = monthlyReport.Where(m => m.ABBRE_GROUP.Equals("IMPORT", StringComparison.OrdinalIgnoreCase))
                                          .Select(m => new
                                          {
                                              m.JAN,
                                              m.FEB,
                                              m.MAR,
                                              m.APR,
                                              m.MAY,
                                              m.JUN,
                                              m.JUL,
                                              m.AUG,
                                              m.SEP,
                                              m.OCT,
                                              m.NOV,
                                              m.DEC
                                          }).FirstOrDefault();
            var localData = monthlyReport.Where(m => m.ABBRE_GROUP.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
                                         .Select(m => new
                                         {
                                             m.JAN,
                                             m.FEB,
                                             m.MAR,
                                             m.APR,
                                             m.MAY,
                                             m.JUN,
                                             m.JUL,
                                             m.AUG,
                                             m.SEP,
                                             m.OCT,
                                             m.NOV,
                                             m.DEC
                                         }).FirstOrDefault();

            // Nếu có nhiều group, bạn có thể sắp xếp theo thứ tự mong muốn.
            // Trả về định dạng JSON có mảng labels (các tháng) và 2 mảng dữ liệu
            var result = new
            {
                labels = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" },
                importData = new[] { importData?.JAN ?? 0, importData?.FEB ?? 0, importData?.MAR ?? 0, importData?.APR ?? 0,
                                importData?.MAY ?? 0, importData?.JUN ?? 0, importData?.JUL ?? 0, importData?.AUG ?? 0,
                                importData?.SEP ?? 0, importData?.OCT ?? 0, importData?.NOV ?? 0, importData?.DEC ?? 0 },
                localData = new[] { localData?.JAN ?? 0,  localData?.FEB ?? 0,  localData?.MAR ?? 0,  localData?.APR ?? 0,
                                localData?.MAY ?? 0,  localData?.JUN ?? 0,  localData?.JUL ?? 0,  localData?.AUG ?? 0,
                                localData?.SEP ?? 0,  localData?.OCT ?? 0,  localData?.NOV ?? 0,  localData?.DEC ?? 0 }
            };
            return Json(result);
        }

        public async Task<IActionResult> TopSupplierError(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không truyền startDate hoặc endDate, mặc định lấy dữ liệu của 1 tuần vừa qua
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Giả sử bạn muốn từ 7 ngày trước đến hôm nay
                endDate = DateTime.Today; // hoặc DateTime.Now nếu bạn muốn cả thời gian
                startDate = endDate.Value.AddDays(-14);
            }
            var supplierErrorReport = await _repository.GetMonthlyAcceptRateByGroupAsync(startDate, endDate);
            if (supplierErrorReport == null)
            {
                return NotFound();
            }
            return View(supplierErrorReport);
        }

        // Input IQC
        // --- Nhận lottagId từ view--
        [HttpGet]
        public async Task<IActionResult> IQCReceiveLottag(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var query = _context.UV_IQC_RESULTs
                .Select(t => new LottagVM
                {
                    id = t.id,
                    yusen_invno = t.yusen_invno,
                    invoice = t.invoice,
                    rcv_date = t.rcv_date,
                    vender_name = t.vender_name,
                    vender_code = t.vender_code,
                    partcode = t.partcode,
                    partname = t.partname,
                    partspec = t.partspec,
                    purchase_order = t.purchase_order,
                    count_pono = t.count_pono,
                    qty = t.qty,
                    location_rec = t.location_rec,
                    iqc_status = t.iqc_status,
                    iqc_rec_lottag = t.iqc_rec_lottag,
                    iqc_rec_person = t.iqc_rec_person,
                    status_lottag = t.status_lottag,
                    abbre_group = t.abbre_group,
                });//.ToListAsync();
            //if (!string.IsNullOrEmpty(searchTerm))
            //{
            //    query = query.Where(t => t.yusen_invno.Contains(searchTerm) ||
            //                    t.vender_code.Contains(searchTerm) ||
            //                    t.vender_name.Contains(searchTerm) ||
            //                    t.partcode.Contains(searchTerm) ||
            //                    t.partname.Contains(searchTerm) ||
            //                    t.partspec.Contains(searchTerm) ||
            //                    t.purchase_order.Contains(searchTerm) ||
            //                    t.location_rec.Contains(searchTerm) ||
            //                    t.iqc_rec_person.Contains(searchTerm) ||
            //                    t.status_lottag.Contains(searchTerm)).ToList();
            //}
            //var pagedResult = new PagedResult<LottagVM>
            //{
            //    Items = query,
            //    TotalItems = query.Count(),
            //    CurrentPage = page,
            //    PageSize = pageSize
            //};
            var searchLottag = new LottagVM();
            query = searchLottag.ApplySearch(query, searchTerm);
            var result = query.ToPagedResult(page, pageSize, searchTerm);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> ScanLotTag([FromBody] LottagVM req)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            string username = HttpContext.Session.GetString("Username");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(req.id))
                return BadRequest(new { status = "error", message = "ID trống" });

            // 1) Check existing
            var existing = await _context.UV_IQC_RESULTs.Where(t => t.id == req.id).FirstOrDefaultAsync();
            if (existing != null)
                return Json(new { status = "error", message = $"Đã nhận lot tag này ngày {existing.iqc_rec_lottag:yyyy-MM-dd} bởi {existing.iqc_rec_person}" });

            // 2) Check WHS_IQC_CONTROL
            var control = await _context.WHS_IQC_CONTROLs.Where(t => t.ID == req.id).FirstOrDefaultAsync();
            if (control == null)
                return Json(new { status = "error", message = "ID này chưa được WHS giao cho IQC." });

            // 3) Check WHS_RECEIVING_TOTAL
            var whs = await _context.WHS_RECEIVING_TOTALs.Where(t => t.ID == req.id).FirstOrDefaultAsync();
            if (whs == null)
                return Json(new { status = "error", message = "Chưa được WHS nhận về." });

            // 4) Insert into UV_IQC_RESULT
            var newLottag = new UV_IQC_RESULT
            {
                id = whs.ID,
                yusen_invno = whs.SK_INVOICE,
                rcv_date = whs.RECEIVED_DATE,
                invoice = whs.SUPPLIER_INVOICE,
                vender_code = whs.SUPPLIER_CODE,
                vender_name = whs.SUPPLIER,
                abbre_group = whs.SUPPLIER_GROUP,
                partcode = whs.PART_CODE,
                partname = whs.PART_DESC,
                partspec = whs.SPEC,
                purchase_order = whs.PURCHASE_ORDER,
                orgcount_pono = whs.PURCHASE_ORDER?.Split(';').Length ?? 0,
                count_pono = whs.PURCHASE_ORDER?.Split(';').Length ?? 0,
                orgqty = (int?)(whs.QTY ?? 0),
                qty = (int?)(whs.QTY ?? 0),
                location_rec = control.RecLocation,
                iqc_status = "IQC NEED CHECK",
                iqc_rec_lottag = DateTime.Now,
                iqc_rec_person = username,
                status_lottag = "WAIT INSPEC"
            };
            _context.UV_IQC_RESULTs.Add(newLottag);
            await _context.SaveChangesAsync();

            return Json(new { status = "ok", data = newLottag });
        }
    }
}