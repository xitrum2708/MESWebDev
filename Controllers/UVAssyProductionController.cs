using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.UVASSY;
using MESWebDev.Models.VMProcedure;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace MESWebDev.Controllers
{
    public class UVAssyProductionController : BaseController
    {
        private readonly IUVAssyProductionRepository _repository;
        private readonly ILoggingService _loggingService;
        private readonly IExcelExportService _excelExportService;
        private readonly IUVASSY_PPRODUCT_HISTORY _uvassyPProductHistoryService;

        public UVAssyProductionController(AppDbContext context, IUVAssyProductionRepository repository, ILoggingService loggingService, IExcelExportService excelExportService, IUVASSY_PPRODUCT_HISTORY uvassyPProductHistoryService)
            : base(context)
        {
            _repository = repository;
            _loggingService = loggingService;
            _excelExportService = excelExportService;
            _uvassyPProductHistoryService = uvassyPProductHistoryService;
        }

        public async Task<IActionResult> Index(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            ViewData["PeriodSort"] = sortOrder == "period" ? "period_desc" : "period";
            ViewData["OutputSort"] = sortOrder == "output" ? "output_desc" : "output";
            ViewData["ErrorSort"] = sortOrder == "error" ? "error_desc" : "error";

            var productionQuantities = await _repository.GetProductionQuantitiesAsync();
            switch (sortOrder)
            {
                case "period":
                    productionQuantities = productionQuantities.OrderBy(p => p.Period).ToList();
                    break;

                case "period_desc":
                    productionQuantities = productionQuantities.OrderByDescending(p => p.Period).ToList();
                    break;

                case "output":
                    productionQuantities = productionQuantities.OrderBy(p => p.OutputQuantity).ToList();
                    break;

                case "output_desc":
                    productionQuantities = productionQuantities.OrderByDescending(p => p.OutputQuantity).ToList();
                    break;

                case "error":
                    productionQuantities = productionQuantities.OrderBy(p => p.ErrorQuantity).ToList();
                    break;

                case "error_desc":
                    productionQuantities = productionQuantities.OrderByDescending(p => p.ErrorQuantity).ToList();
                    break;
            }
            // Default to current date if startDate or endDate is not provided
            startDate ??= DateTime.Today;
            endDate ??= DateTime.Today;

            // Fetch real-time production results
            var productionResults = await _repository.GetProductionResultsAsync(startDate.Value, endDate.Value);

            // Prepare data for the chart (group by Model and sum ProdQty)
            var chartData = productionResults
                .GroupBy(r => r.Model)
                .Select(g => new { Model = g.Key, TotalProdQty = g.Sum(r => r.ProdQty) })
                .ToList();
            // Pass data to the view using ViewBag or a ViewModel
            ViewBag.ProductionResults = productionResults;
            ViewBag.ChartData = chartData;
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View(productionQuantities);
            //return View(productionQuantities);
        }

        // Add the GetProductionResults action
        [HttpGet]
        public async Task<IActionResult> GetProductionResults(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Validate date range
                if (startDate == default || endDate == default)
                {
                    return BadRequest("Start date and end date are required.");
                }

                if (startDate > endDate)
                {
                    return BadRequest("Start date cannot be later than end date.");
                }

                var productionResults = await _repository.GetProductionResultsAsync(startDate, endDate);
                var summaryData = await _repository.GetProductionQuantitiesAsync();
                var chartData = productionResults
                    .GroupBy(r => r.Model)
                    .Select(g => new { Model = g.Key, TotalProdQty = g.Sum(r => r.ProdQty) })
                    .ToList();

                return Json(new { productionResults, chartData, summaryData });
            }
            catch (Exception ex)
            {
                // Log the exception
                await _loggingService.LogActionAsync(
                    actionName: "GetProductionResults",
                    actionType: "Error",
                    action: () => Task.FromResult(0),
                    createdBy: "System",
                    additionalDetails: $"Error: {ex.Message}"
                );
                return StatusCode(500, "An error occurred while fetching production results.");
            }
        }

        public async Task<IActionResult> Details(string period, int page = 1, int pageSize = 10, string searchTerm = null)
        {
            if (string.IsNullOrEmpty(period))
            {
                return NotFound("Period is required.");
            }

            var productionQuantities = await _repository.GetProductionQuantitiesAsync();
            var item = productionQuantities.FirstOrDefault(p => p.Period == period);

            if (item == null)
            {
                return NotFound();
            }

            // Fetch the output and error details
            var outputDetails = await _repository.GetOutputDetailsAsync(period);
            var errorDetails = await _repository.GetErrorDetailsAsync(period);

            // Apply search filter if searchTerm is provided
            IQueryable<UVAssyOutputDetail> outputDetailsQuery = outputDetails.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                outputDetailsQuery = outputDetailsQuery.Where(d =>
                    (d.Line != null && d.Line.ToLower().Contains(searchTerm)) ||
                    (d.Model != null && d.Model.ToLower().Contains(searchTerm)) ||
                    (d.Lot != null && d.Lot.ToLower().Contains(searchTerm)) ||
                    (d.Model_Serial != null && d.Model_Serial.ToLower().Contains(searchTerm)) ||
                    (d.Dbox_Serial != null && d.Dbox_Serial.ToLower().Contains(searchTerm)) ||
                    (d.Unit_Serial != null && d.Unit_Serial.ToLower().Contains(searchTerm)) ||
                    (d.BatchNo != null && d.BatchNo.ToLower().Contains(searchTerm)) ||
                    (d.ChangeBatchTo != null && d.ChangeBatchTo.ToLower().Contains(searchTerm)) ||
                    (d.ErrorDetail != null && d.ErrorDetail.ToLower().Contains(searchTerm)) ||
                    (d.ErrorStatus != null && d.ErrorStatus.ToLower().Contains(searchTerm)) ||
                    (d.CreatedBy != null && d.CreatedBy.ToLower().Contains(searchTerm)) ||
                    (d.Remark != null && d.Remark.ToLower().Contains(searchTerm))
                );
            }

            // Apply search filter if searchTerm is provided
            IQueryable<UVAssyErrorDetail> errorDetailsQuery = errorDetails.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                errorDetailsQuery = errorDetailsQuery.Where(d =>
                (d.Qrcode != null && d.Qrcode.ToLower().Contains(searchTerm)) ||
                (d.Model != null && d.Model.ToLower().Contains(searchTerm)) ||
                (d.Lot != null && d.Lot.ToLower().Contains(searchTerm)) ||
                (d.Partcode != null && d.Partcode.ToLower().Contains(searchTerm)) ||
                (d.Pcbtype != null && d.Pcbtype.ToLower().Contains(searchTerm)) ||
                (d.Errortype != null && d.Errortype.ToLower().Contains(searchTerm)) ||
                (d.CreatedBy != null && d.CreatedBy.ToLower().Contains(searchTerm)) ||
                (d.UserDept != null && d.UserDept.ToLower().Contains(searchTerm)) ||
                (d.Statusresult != null && d.Statusresult.ToLower().Contains(searchTerm))
                );
            }
            // Paginate the output details
            var outputDetailsPaged = outputDetailsQuery.ToPagedResult(page, pageSize, searchTerm);

            // Optionally paginate the error details (if needed in the future)
            var errorDetailsPaged = errorDetailsQuery.AsQueryable().ToPagedResult(page, pageSize, searchTerm);

            var viewModel = new UVAssyProductionViewModel
            {
                Summary = item,
                UVAssyOutputDetails = outputDetails, // Keep the full list for other uses
                UVAssyErrorDetails = errorDetails   // Keep the full list for other uses
            };

            // Ensure ViewBag.OutputDetailsPaged is always set
            ViewBag.OutputDetailsPaged = outputDetailsPaged ?? new PagedResult<UVAssyOutputDetail>();
            ViewBag.ErrorDetailsPaged = errorDetailsPaged ?? new PagedResult<UVAssyErrorDetail>();
            ViewBag.Period = period; // Ensure the period is available for the partial views
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;

            return View(viewModel);
        }

        // Action tổng quát xuất Excel cho Output Details
        public async Task<IActionResult> DownloadExcelOutputDetails(string period)
        {
            // Nếu period truyền vào, bạn có thể lấy lại dữ liệu mới
            if (string.IsNullOrEmpty(period))
            {
                return BadRequest("Period is required.");
            }

            // Lấy dữ liệu từ repository nếu cần (hoặc sử dụng _outputDetails nếu đã được set)
            var outputDetails = await _repository.GetOutputDetailsAsync(period.Trim());

            if (outputDetails == null || !outputDetails.Any())
            {
                return NotFound("No output details available for the specified period.");
            }

            // Sử dụng service để xuất Excel
            byte[] fileContents = _excelExportService.ExportToExcel(outputDetails, "OutputDetails");

            string fileName = $"OutputDetails-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet]
        public async Task<IActionResult> SearchResultByQrOrBarcode(string key)
        {
            var dt = await _uvassyPProductHistoryService.SearchResult(key);
            ViewBag.Key = key;
            //if (dt == null) return View(dt);

            //bool hasCreated = dt.Columns.Contains("CreatedDate");
            //bool hasTs = dt.Columns.Contains("Timestamp");

            //if (hasCreated && !dt.Columns.Contains("CreatedDateFmt"))
            //{
            //    var c = dt.Columns.Add("CreatedDateFmt", typeof(string));
            //    c.SetOrdinal(dt.Columns["CreatedDate"].Ordinal + 1);
            //}
            //if (hasTs && !dt.Columns.Contains("TimestampFmt"))
            //{
            //    var c = dt.Columns.Add("TimestampFmt", typeof(string));
            //    c.SetOrdinal(dt.Columns["Timestamp"].Ordinal + 1);
            //}

            //foreach (DataRow row in dt.Rows)
            //{
            //    if (hasCreated && row["CreatedDate"] != DBNull.Value)
            //    {
            //        if (row["CreatedDate"] is DateTime d1)
            //            row["CreatedDateFmt"] = d1.ToString("yyyy/MM/dd HH:mm:ss");
            //        else if (DateTime.TryParse(Convert.ToString(row["CreatedDate"]), out var d2))
            //            row["CreatedDateFmt"] = d2.ToString("yyyy/MM/dd HH:mm:ss");
            //    }

            //    if (hasTs && row["Timestamp"] != DBNull.Value && dt.Columns.Contains("TimestampFmt"))
            //    {
            //        var v = row["Timestamp"];
            //        if (v is DateTime ts)
            //            row["TimestampFmt"] = ts.ToString("yyyy/MM/dd HH:mm:ss");
            //        else if (v is byte[] rv)            // trường hợp rowversion
            //            row["TimestampFmt"] = BitConverter.ToString(rv).Replace("-", "");
            //        else if (DateTime.TryParse(Convert.ToString(v), out var ts2))
            //            row["TimestampFmt"] = ts2.ToString("yyyy/MM/dd HH:mm:ss");
            //    }
            //}

            return View(dt);
        }
        // Lấy danh sách file đính kèm (cũng trả DataTable, load vào PartialView)
        [HttpGet]
        public async Task<IActionResult> Attachments(int processId, string qrcode)
        {
            var dt = await _uvassyPProductHistoryService.GetAttachedList(processId, qrcode);
            return PartialView("_AttachmentsDt", dt);
        }

        // Download file theo Id (đọc 1 dòng, không cần DataTable)
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var dt = await _uvassyPProductHistoryService.GetAttachedFile(id);
            if (dt.Rows.Count == 0) return NotFound();
            var row = dt.Rows[0];
            var bytes = (byte[])row["Attach_File"];
            var name = row["Attach_Name"] as string ?? "attachment.bin";
            return File(bytes, "application/octet-stream", name);
        }
    }
}