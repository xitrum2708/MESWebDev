using MESWebDev.Data;
using MESWebDev.DTO;
using MESWebDev.Filters;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class IQCCreateReportController : BaseController
    {
        private readonly IIQCReportsRepository _repo;
        
        public IQCCreateReportController(AppDbContext context, IIQCReportsRepository repository) : base(context)
        {
            _repo = repository;
        }
        [AuthorizeLogin]
        [HttpGet]
        public IActionResult CreateReport()
        {
            var model = new IQCReportCreateVM
            {
                AvailableGroups = _context.UV_IQC_InspectionGroups.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ScanLottag([FromBody] string LottagId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            string username = HttpContext.Session.GetString("Username");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(LottagId))
                return Json(new { status = "error", message = "Lottag không được để trống." });

            // Tìm lottag trong báo cáo đã tồn tại
            var existingReport = _context.UV_IQC_Reports
                .FirstOrDefault(r => r.LottagId == LottagId || r.Remark.Contains(LottagId));

            if (existingReport != null)
            {
                if (existingReport.Status == "PENDING" || existingReport.Status == "RETURN TO INSPECTOR")
                {
                    return Json(new
                    {
                        status = "resume",
                        message = "Lottag đã được tạo trước đó và đang chờ xử lý.",
                        reportId = existingReport.ReportID
                    });
                }

                return Json(new { status = "error", message = "Lottag đang được xử lý bởi người khác hoặc đã được xử lý xong." });
            }

            var result = _context.UV_IQC_RESULTs.FirstOrDefault(x => x.id == LottagId);
            if (result == null)
                return Json(new { status = "error", message = "Không tìm thấy lottag trong IQC_RESULT" });

            return Json(new
            {
                status = "ok",
                data = new
                {
                    id = result.id,
                    yusen_invno = result.yusen_invno,
                    rcv_date = result.rcv_date,
                    vender_code = result.vender_code,
                    vender_name = result.vender_name,
                    partcode = result.partcode,
                    partname = result.partname,
                    purchase_order = result.purchase_order,
                    invoice = result.invoice,
                    orgcount_pono = result.orgcount_pono,
                    orgqty = result.orgqty
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveReport(IQCReportCreateVM model)
        {
            string username = HttpContext.Session.GetString("Username");
            if (model.ScannedLottags == null || model.ScannedLottags.Count == 0)
                return Json(new { status = "error", message = "Chưa có lottag nào được thêm." });
            // Lấy tất cả dữ liệu từ UV_IQC_RESULT
            // var ids = model.ScannedLottags.ToArray();
            var ids = model.ScannedLottags
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToArray();

            //Console.WriteLine("DANH SÁCH IDS: " + string.Join(" | ", ids));
            //foreach (var id in ids)
            //{
            //    var found = await _context.UV_IQC_RESULTs.FirstOrDefaultAsync(x => x.id == id);
            //    Console.WriteLine($"Check {id}: {(found != null ? "FOUND" : "NOT FOUND")}");
            //}

            var lottagResults = new List<UV_IQC_RESULT>();
            foreach (var id in ids)
            {
                var found = await _context.UV_IQC_RESULTs.FirstOrDefaultAsync(x => x.id == id);
                if (found != null)
                {
                    lottagResults.Add(found);
                }
            }
            // Không chạy đúng với sql 2008 vì chưa hỗ trợ json trong câu lệnh sql
            //var lottagResults = await _context.UV_IQC_RESULTs
            //    .Where(x => ids.Contains(x.id))
            //    .ToListAsync();

            if (lottagResults.Count != model.ScannedLottags.Count)
                return Json(new { status = "error", message = "Một số lottag không tồn tại trong hệ thống." });
            // Lấy lottag đầu tiên làm đại diện
            var firstLottag = model.ScannedLottags.First();
            var firstResult = lottagResults.FirstOrDefault(x => x.id == firstLottag);

            if (firstResult == null)
                return Json(new { status = "error", message = "Không tìm thấy lottag đầu tiên." });
            // Tổng hợp các trường
            var allInvoices = lottagResults
                .Where(x => !string.IsNullOrEmpty(x.invoice))
                .Select(x => x.invoice)
                .Distinct();

            var allPONOs = lottagResults
                .Where(x => !string.IsNullOrEmpty(x.purchase_order))
                .Select(x => x.purchase_order)
                .Distinct();

            int totalPOQty = lottagResults.Sum(x => x.orgqty ?? 0);
            int totalPOCount = lottagResults.Sum(x => x.count_pono ?? 0);

            var rep = new UV_IQC_Report
            {
                LottagId = model.ScannedLottags.First(),
                VendorName = firstResult.vender_name,
                VendorCode = firstResult.vender_code,
                PartCode = firstResult.partcode,
                PartName = firstResult.partname,
                PO_NO = string.Join(';', allPONOs),
                InvoiceNo = string.Join(';', allInvoices),
                InspectionGroupID = model.SelectedGroupId,
                POQty = totalPOQty,
                POCount = totalPOCount,
                Status = "PENDING",
                Remark = string.Join(';', model.ScannedLottags),
                CreatedBy = username ?? "System",
                UpdatedBy = username ?? "System",
                InspectionDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };
            _context.Entry(rep).State = EntityState.Detached;
            await _context.Database.ExecuteSqlRawAsync(
                 "INSERT INTO UV_IQC_Reports (LottagId, VendorName, VendorCode, PartName, PartCode, PO_NO, InvoiceNo, " +
                 "POQty, POCount, InspectionDate, InspectionGroupID, Status, Remark, CreatedDate, CreatedBy) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}" +
                 ", {9}, {10}, {11}, {12}, {13}, {14})",
                 rep.LottagId, rep.VendorName, rep.VendorCode, rep.PartName, rep.PartCode, rep.PO_NO, rep.InvoiceNo,
                 rep.POQty, rep.POCount, rep.InspectionDate, rep.InspectionGroupID, rep.Status, rep.Remark, rep.CreatedDate, rep.CreatedBy);

            //_context.UV_IQC_Reports.Add(rep);
            //await _context.SaveChangesAsync();

            //return RedirectToAction("Index");

            var report = await _context.UV_IQC_Reports
                        .OrderByDescending(r => r.CreatedDate)
                        .FirstOrDefaultAsync(r => r.LottagId == model.ScannedLottags.First());

            if (report == null)
                return Json(new { status = "error", message = "Không lấy được Report mới tạo." });

            return RedirectToAction("FillItems", new { reportId = report.ReportID });
        }

        public async Task<IActionResult> LoadForm(int itemId, string reportId)
        {
            var item = await _context.UV_IQC_ItemNames.FindAsync(itemId);
            var isExists = _context.UV_IQC_ReportItems.Any(r => r.ReportID == reportId && r.ItemID == itemId);
            var prev = await _context.UV_IQC_ReportItems
                 .Where(r => r.ReportID == reportId && r.ItemID == itemId)
                 .OrderByDescending(r => r.CreatedDate)
                 .FirstOrDefaultAsync();
            var report = await _context.UV_IQC_Reports
                .Where(r => r.ReportID == reportId)
                .FirstOrDefaultAsync();
            //var prev= await _context.UV_IQC_ReportItems
            //     .Where(r => r.ReportID == reportId && r.ItemID == itemId
            //                 && r.SamplingSize>0)
            //     .OrderByDescending(r => r.CreatedDate)
            //     .Select(r => r.SamplingSize)
            //     .FirstOrDefaultAsync();
            //var prevStd = await _context.UV_IQC_ReportItems
            //   .Where(r => r.ReportID == reportId && r.ItemID == itemId && !string.IsNullOrEmpty(r.Standard))
            //   .OrderByDescending(r => r.CreatedDate)
            //   .Select(r => r.Standard)
            //   .FirstOrDefaultAsync();

            var errorMasterList = await _context.UV_IQC_ErrorsItemMasters
                                .ToListAsync();
            var vm = new FillItemsVM
            {
                ItemName = item,
                ReportId = reportId,
                IsAlreadyFilled = isExists,
                FormPartial = item.ItemName.Substring(0, 3) switch
                {
                    // E = ELECTRICAL, P = PLASTIC, A = PACKING, R = PRINTING
                    "E01" or "P01" or "A01" or "R01" or
                    "E07" or "P07" => "_DeliveryForm",
                    "E02" or "P02" or "A02" or "R02" => "_AppearanceForm",
                    "E03" or "P03" or "A03" or "R03" => "_DimensionalForm",

                    "E04" or "E05" or "E06" or
                    "P04" or "P05" or "P06" or "P08" or
                    "A04" or "A05" or "A06" or "A07" or
                    "R04" or "R05" or
                    "E08" or "A08" or "R06" => "_CommonForm",

                    //"E07" or "P07" => "_RohsForm",
                    //"E08" or "A08" or "R06" => "_OtherForm",
                    "E09" or "P09" or "A09" or "R09" => "_FinalForm",
                    "E10" or "P10" or "A10" or "R10" => "_SaveSendForm",
                },
                Report = new UV_IQC_Report
                {
                    Status = report.Status,
                },
                ErrorsItemMasterList = errorMasterList,
                ReportItem = new ReportItemVM
                {
                    ReportID = reportId,
                    ItemID = itemId,
                    SamplingSize = prev?.SamplingSize ?? 0,
                    Standard = prev?.Standard,
                    Spec = prev?.Spec,
                    SpecDetail = prev?.SpecDetail,
                }
            };

            return PartialView("_ItemForm", vm);
        }

        // Trả về danh sách các bản ghi đã nhập cho item này
        public async Task<IActionResult> LoadList(int itemId, string reportId)
        {
            var item = await _context.UV_IQC_ItemNames.FindAsync(itemId);
            var report = await _context.UV_IQC_Reports.Where(r => r.ReportID == reportId).FirstOrDefaultAsync();
            ViewBag.IsFinished = report.Status;
            var formPartial = item.ItemName.Substring(0, 3) switch
            {
                // E = ELECTRICAL, P = PLASTIC, A = PACKING, R = PRINTING
                "E01" or "P01" or "A01" or "R01" or
                "E07" or "P07" => "_DeliveryForm",
                "E02" or "P02" or "A02" or "R02" => "_AppearanceForm",
                "E03" or "P03" or "A03" or "R03" => "_DimensionalForm",

                "E04" or "E05" or "E06" or
                "P04" or "P05" or "P06" or "P08" or
                "A04" or "A05" or "A06" or "A07" or
                "R04" or "R05" or
                "E08" or "A08" or "R06" => "_CommonForm",
                //"E07" or "P07" => "_RohsForm",
                //"E08" or "A08" or "R06" => "_OtherForm",
                "E09" or "P09" or "A09" or "R09" => "_FinalForm",
                "E10" or "P10" or "A10" or "R10" => "_SaveSendForm",
            };
            var list = await (
                from r in _context.UV_IQC_ReportItems
                where r.ReportID == reportId && r.ItemID == itemId
                join em in _context.UV_IQC_ErrorsItemMasters
                on r.ErrorCodeID equals em.ErrorCodeID into ems
                from em in ems.DefaultIfEmpty()   // ← đây tạo LEFT JOIN
                select new ReportItemDto
                {
                    ReportItemID = r.ReportItemID,
                    ReportID = r.ReportID,
                    ErrorCodeID = r.ErrorCodeID,
                    ErrorName = em != null ? em.Description : null,
                    ItemID = r.ItemID,
                    ItemName = r.ItemName,
                    SamplingSize = r.SamplingSize,
                    Spec = r.Spec,
                    SpecDetail = r.SpecDetail,
                    CRI = r.CRI,
                    MAJ = r.MAJ,
                    MIN = r.MIN,
                    NG_Total = r.NG_Total,
                    NG_Rate = r.NG_Rate,
                    Standard = r.Standard,
                    Judgment = r.Judgment,
                    Remark = r.Remark,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = r.CreatedDate
                }).ToListAsync();
            ViewBag.FormPartial = formPartial;

            return PartialView("_ItemDataList", list);
        }

        // Xóa một record
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int reportItemId)
        {
            var rec = _context.UV_IQC_ReportItems
                .Where(x => x.ReportItemID == reportItemId).FirstOrDefault();
            if (rec == null)
                return BadRequest("ReportItemId is required.");
            try
            {
                await _repo.DeleteItemAsync(reportItemId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting item: {ex.Message}");
            }
        }

        // Lưu record mới
        [HttpPost]
        public async Task<IActionResult> SaveItem([FromForm] FillItemsVM model)
        {
            if (string.IsNullOrEmpty(model.ReportItem.ReportID) || model.ReportItem.ItemID == 0)
            {
                return BadRequest("ReportID and ItemID are required.");
            }
            // Kiểm tra xem trạng thái của ReportID này là ACCEPTED
            var report = await _context.UV_IQC_Reports
                .Where(r => r.ReportID == model.ReportItem.ReportID)
                .FirstOrDefaultAsync();

            // 1) Clear tất cả lỗi validation ban đầu
            ModelState.Clear();

            //// 2) Manual validate chỉ những gì mình cần
            //if (model.ReportItem.SamplingSize <= 0)
            //{
            //    ModelState.AddModelError(
            //        "ReportItem.SamplingSize",
            //        "Sampling Size phải lớn hơn 0.");
            //}
            // Chỉ chạy check CRI nếu đã nhập (> 0)
            if (model.ReportItem.CRI > 0
             && model.ReportItem.CRI > model.ReportItem.SamplingSize)
            {
                ModelState.AddModelError(
                    "ReportItem.CRI",
                    $"Số lỗi CRI không được vượt quá {model.ReportItem.SamplingSize}.");
            }
            if (model.ReportItem.MAJ > 0
             && model.ReportItem.MAJ > model.ReportItem.SamplingSize)
            {
                ModelState.AddModelError(
                    "ReportItem.MAJ",
                    $"Số lỗi MAJ không được vượt quá {model.ReportItem.SamplingSize}.");
            }
            if (model.ReportItem.MIN > 0
             && model.ReportItem.MIN > model.ReportItem.SamplingSize)
            {
                ModelState.AddModelError(
                    "ReportItem.MIN",
                    $"Số lỗi MIN không được vượt quá {model.ReportItem.SamplingSize}.");
            }
            var specEntries = model.SpecList?
                .Select(s => s?.Trim())                           // trim whitespace
                .Where(s => !string.IsNullOrWhiteSpace(s))        // bỏ chuỗi null/empty/whitespace
                .ToList();
            var specEntriesDetail = model.SpecDetailList?
                .Select(s => s?.Trim())                           // trim whitespace
                .Where(s => !string.IsNullOrWhiteSpace(s))        // bỏ chuỗi null/empty/whitespace
                .ToList();
            // 3) Nếu có lỗi, repopulate ViewModel và trả về PartialView để hiển thị validation-for
            if (!ModelState.IsValid)
            {
                // load lại những dữ liệu cần thiết cho form partial
                model.ItemName = await _context.UV_IQC_ItemNames.FindAsync(model.ReportItem.ItemID);
                model.ErrorsItemMasterList = await _context.UV_IQC_ErrorsItemMasters.ToListAsync();
                model.IsAlreadyFilled = _context.UV_IQC_ReportItems
                                         .Any(r => r.ReportID == model.ReportId
                                                && r.ItemID == model.ReportItem.ItemID);

                model.ReportItem.Spec = (specEntries != null && specEntries.Any())
                     ? string.Join(";", specEntries)
                     : null;
                model.ReportItem.SpecDetail = (specEntriesDetail != null && specEntriesDetail.Any())
                    ? string.Join(";", specEntriesDetail)
                    : null;

                // ... nếu cần lấy prev SampleSize / Standard, gán vào model.ReportItem
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return PartialView("_ItemForm", model);
            }

            try
            {
                if (report.Status.ToUpper() == "ACCEPTED" ||
                report.Status.ToUpper() == "REJECTED" ||
                report.Status.ToUpper() == "FINISHED")
                {
                    model.FinishedStatus = report.Status;
                    ViewBag.Finished = "CannotSaveData";
                    return Ok();
                }
                // Map ReportItemVM to ReportItem
                var reportItem = new UV_IQC_ReportItem
                {
                    ReportID = model.ReportItem.ReportID,
                    ErrorCodeID = model.ReportItem.ErrorCodeID != null ? model.ReportItem.ErrorCodeID : 0,
                    ItemID = model.ReportItem.ItemID,
                    ItemName = model.ReportItem.ItemName,
                    SamplingSize = model.ReportItem.SamplingSize,
                    Spec = (specEntries != null && specEntries.Any())
                     ? string.Join(";", specEntries)
                     : null,
                    SpecDetail = (specEntriesDetail != null && specEntriesDetail.Any())
                     ? string.Join(";", specEntriesDetail)
                     : null,
                    CRI = model.ReportItem.CRI,
                    MAJ = model.ReportItem.MAJ,
                    MIN = model.ReportItem.MIN,
                    NG_Total = model.ReportItem.NG_Total,
                    NG_Rate = model.ReportItem.NG_Rate,
                    Standard = model.ReportItem.Standard,
                    Judgment = model.ReportItem.Judgment,
                    Remark = model.ReportItem.Remark,
                    CreatedBy = HttpContext.Session.GetString("Username") ?? "System", // Adjust based on your auth
                    CreatedDate = DateTime.Now
                };

                // Save to database
                await _repo.SaveItemAsync(reportItem);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving item: {ex.Message}");
            }
        }

        public async Task<IActionResult> FillItems(string reportId)
        {
            var report = await _context.UV_IQC_Reports.FirstOrDefaultAsync(r => r.ReportID == reportId);
            if (report == null) return NotFound();

            var items = await _context.UV_IQC_ItemNames
                .Where(i => i.GroupID == report.InspectionGroupID)
                .ToListAsync();

            var viewModel = new FillItemsVM
            {
                Report = report,
                ReportId = reportId,
                GroupId = report.InspectionGroupID,
                Items = items,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendForApproval([FromBody] string reportId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            string username = HttpContext.Session.GetString("Username");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var report = await _context.UV_IQC_Reports.FirstOrDefaultAsync(r => r.ReportID == reportId);
            var reportItem = await _context.UV_IQC_ReportItems.FirstOrDefaultAsync(r => r.ReportID == reportId && r.ItemName.Contains("FINAL JUDGMENT"));
            int reportItemCount = await _context.UV_IQC_ReportItems.Where(r => r.ReportID == reportId && !r.ItemName.Contains("FINAL JUDGMENT")).CountAsync();
            if (report == null)
                return NotFound();
            if (report.Status != "PENDING" && report.Status == "RETURN TO INSPECTOR")
                return Json(new { status = "error", message = "Only PENDING and RETURN TO INSPECTOR reports can be sent for approval." });
            if (reportItem == null)
                return Json(new { status = "error", message = "Make Final Judgment befor send for approval." });
            if (reportItemCount < 1)
                return Json(new { status = "error", message = "At least one inspection item must be completed before sending for approval." });
            report.Status = "WAIT ACCEPT";
            report.UpdatedDate = DateTime.Now;
            report.Notes = null;
            report.TextRemark = null;
            report.UpdatedBy = HttpContext.Session.GetString("Username");
            await _repo.UpdateReportStatusAsync(report);
            return Json(new { status = "success" });
        }
    }
}