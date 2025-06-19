using Dapper;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.DTO;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.REPAIR;
using MESWebDev.Models.UVASSY.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MESWebDev.Controllers
{
    public class RepairReportController : BaseController
    {
        public readonly ITranslationService _translationService;
        private readonly IRepairResultService _svc;
        private readonly DownloadExcelExportService _excelExportService;
        private readonly IExcelExportService _excelExportSvr;
        private readonly SqlHelperService _sqlHelperService;

        public RepairReportController(AppDbContext context, ITranslationService translationService, IRepairResultService svc, DownloadExcelExportService excelExportService, SqlHelperService sqlHelperService, IExcelExportService excelExportSvr)
         : base(context)
        {
            _translationService = translationService;
            _svc = svc;
            _excelExportService = excelExportService;
            _sqlHelperService = sqlHelperService;
            _excelExportSvr = excelExportSvr;
        }

        public async Task<IActionResult> Index(DateTime? startDate,
                                                DateTime? endDate,
                                                string? SearchTerm,
                                                string? UserDept,
                                                int page = 1, int pageSize = 10)
        {
            //var from = startDate ?? DateTime.UtcNow.AddDays(-6);
            //var to = endDate ?? DateTime.UtcNow;
            //var UserDeptList = await _context.Set<UV_REPAIRRESULT>()
            //    .Where(x => !string.IsNullOrEmpty(x.UserDept))
            //    .GroupBy(x => x.UserDept)
            //    .Select(g => g.Key!)
            //    .OrderBy(name => name)
            //    .ToListAsync();
            var UserDeptList = await _context.Set<UV_REPAIRRESULT>()
                .Where(x => !string.IsNullOrEmpty(x.UserDept))
                .GroupBy(x => x.UserDept)
                .Select(g => g.Key == "UNIDEN" ? "ASSY" : g.Key)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();

            // Lấy danh sách UVRepairResult để feed keys
            if (UserDept == "ASSY")
            {
                UserDept = "UNIDEN";
            }
            var rawKeys = await _svc.GetWithBulkCopyAsync(
                new List<(string, string)>(),
                startDate, endDate, SearchTerm, UserDept);

            // 3. Chuẩn bị viewModel chung
            var viewModel = new RepairVM
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                UserDept = UserDept,
                UserDeptList = UserDeptList,
            };
            //viewModel.UserDept = "UNIDEN";
            if (rawKeys == null || !rawKeys.Any())
            {
                // Nếu không có dữ liệu, trả về view với thông báo
                ViewBag.Message = "Không có dữ liệu nào trong khoảng thời gian này.";
                viewModel.RepairResult = new PagedResult<RepairResultDto>();
                return View(viewModel);
            }
            var resultPage = rawKeys.AsQueryable()
                .OrderByDescending(x => x.CreatedDate)
                .ToPagedResult(page, pageSize);

            viewModel.RepairResult = resultPage;
            // Đưa lại giá trị cho form giữ trạng thái
            ViewBag.FromDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchText = SearchTerm ?? "";
            ViewBag.UserDept = UserDept ?? "";

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate, string? searchText, string? UserDept)
        {
            if (UserDept == "ASSY")
            {
                UserDept = "UNIDEN";
            }
            var results = await _svc.GetWithBulkCopyAsync(null, fromDate, toDate, searchText, UserDept);
            if (results == null || !results.Any())
            {
                // Nếu không có dữ liệu, trả về view với thông báo
                ViewBag.Message = "Không có dữ liệu nào trong khoảng thời gian này.";
                return View(new DownloadButtonModel());
            }
            // Define column mappings for user-friendly headers
            var columnMappings = new Dictionary<string, string>
        {
            { "Qrcode", "QR Code" },
            { "Model", "Model" },
            { "Lot", "Lot" },
            { "DailyOutput", "Daily Output" },
            { "PcbCode", "PCB Code" },
            { "Pcbtype", "PCB Type" },
            { "Process", "Process" },
            { "Errorposition", "Error Position" },
            { "Partcode", "Part Code" },
            { "Errortype", "Error Type" },
            { "Causetype", "Cause Type" },
            { "DeptError", "Dept Error" },
            { "Repairmethod", "Repair Method" },
            { "Statusresult", "Status Result" },
            { "UserDept", "User Dept" },
            { "Linename", "Line Name" },
            { "Qty", "Quantity" },
            { "CreatedDate", "Created Date" },
            { "CreatedBy", "Created By" },
            { "Remark", "Remark" },
            { "Soldermachine", "Solder Machine" },
            { "Tinwire", "Tin Wire" },
            { "Flux", "Flux" },
            { "Alcohol", "Alcohol" },
            { "Other", "Other" },
            { "DDRDate", "DDR Date" },
            { "DDRKeyin", "DDR Keyin" },
            { "DDRCHECK", "DDR Check" },
            { "DDRDailyUpdate", "DDR Daily Update" },
            { "DateCode", "Date Code" }
        };

            var bytes = _excelExportService.DownloadExportToExcel(results, columnMappings);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Repair_UV_Report_{UserDept}_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // KPI REPORT
        public async Task<IActionResult> KPIReport()
        {
            var model = new ReportVM();

            // Ensure YearList is loaded
            string sqlQuery = "SELECT DISTINCT Year(CreatedDate) AS Year FROM UVASSY_CBSAVEDATA ORDER BY Year DESC";
            model.YearList = await _sqlHelperService.ExecuteSqlQueryAsync<int>(sqlQuery);

            // Set default selected year if not yet picked
            model.SelectedYear = model.YearList.FirstOrDefault();

            await LoadReportDataAsync(model);
            //model.MonthlyData = new List<MonthlyData>();
            //model.WeeklyData = new List<WeeklyData>();
            //model.MonthlyDetailTable = new DataTable();
            //model.WeeklyDetailTable = new DataTable();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> KPIReport(ReportVM model)
        {
            string sqlQuery = "SELECT DISTINCT Year(CreatedDate) AS Year FROM UVASSY_CBSAVEDATA ORDER BY Year DESC";
            model.YearList = await _sqlHelperService.ExecuteSqlQueryAsync<int>(sqlQuery);
            await LoadReportDataAsync(model);
            return View(model);
        }

        private async Task LoadReportDataAsync(ReportVM model)
        {
            var year = model.SelectedYear > 0 ? model.SelectedYear : DateTime.Now.Year;
            var startWeek = model.startWeek;
            var endWeek = model.endWeek;

            var monthChartParams = new DynamicParameters();
            monthChartParams.Add("@Year", year);
            model.MonthlyData = await _sqlHelperService.ExecuteStoredProcedureAsync<MonthlyData>("sp_web_Repair_Uniden_Report_Month_Chart", monthChartParams);

            var weekChartParams = new DynamicParameters();
            weekChartParams.Add("@Year", year);
            weekChartParams.Add("@StartWeek", startWeek);
            weekChartParams.Add("@EndWeek", endWeek);
            model.WeeklyData = await _sqlHelperService.ExecuteStoredProcedureAsync<WeeklyData>("sp_web_Repair_Uniden_Report_Week_Chart", weekChartParams);

            model.MonthlyDetailTable = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_Report_Month", monthChartParams);
            model.WeeklyDetailTable = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_Report_Week", weekChartParams);
        }

        // NG MODEL REPORT
        public async Task<IActionResult> NGModelReport()
        {
            var model = new ReportVM
            {
                fromDate = DateTime.Today.AddMonths(-1), // Default: 7 days ago
                toDate = DateTime.Today,               // Default: today
                ModelData = new List<ModelData>(),
                ModelDetailTable = new DataTable()
            };
            //await LoadNGModelReportDataAsync(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NGModelReport(IFormCollection form)
        {
            var model = new ReportVM
            {
                Model = form["Model"],
                fromDate = DateTime.TryParse(form["fromDate"], out var from) ? from : (DateTime?)null,
                toDate = DateTime.TryParse(form["toDate"], out var to) ? to : (DateTime?)null
            };

            await LoadNGModelReportDataAsync(model);
            return View(model);
        }

        private async Task LoadNGModelReportDataAsync(ReportVM model)
        {
            var Model = model.Model;
            var startDate = model.fromDate;
            var endDate = model.toDate;

            var dateChartParams = new DynamicParameters();
            dateChartParams.Add("@StartDate", startDate);
            dateChartParams.Add("@EndDate", endDate);

            var dateDataParams = new DynamicParameters();
            dateDataParams.Add("@StartDate", startDate);
            dateDataParams.Add("@EndDate", endDate);
            dateDataParams.Add("@Model", Model);
            var ModelListData = new List<ModelData>();
            //model.ModelData = await _sqlHelperService.ExecuteStoredProcedureAsync<ModelData>("sp_web_Repair_Uniden_ByModelDate_Chart1", dateChartParams);
            model.ModelDetailTable = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_ByModelDate1", dateDataParams);
            if (model.ModelDetailTable.Rows.Count > 0)
            {
                for (int i = 0; i < Math.Min(10, model.ModelDetailTable.Rows.Count); i++)
                {
                    var modelData = new ModelData
                    {
                        Model = model.ModelDetailTable.Rows[i]["Model"].ToString(),
                        InputQty = int.Parse(model.ModelDetailTable.Rows[i]["InputQty"].ToString()),
                        NGQty = int.Parse(model.ModelDetailTable.Rows[i]["NGQty"].ToString()),
                        NGRate = decimal.Parse(model.ModelDetailTable.Rows[i]["NGRate"].ToString()),
                    };
                    ModelListData.Add(modelData);
                }
                if (ModelListData.Count > 0)
                {
                    model.ModelData = ModelListData;
                }
            }
        }

        public async Task<IActionResult> TopErrorReport()
        {
            var model = new ReportVM
            {
                fromDate = DateTime.Today.AddMonths(-1), // Default: 7 days ago
                toDate = DateTime.Today,               // Default: today
                DeptData = new List<DeptData>(),
                ErrorData = new List<ErrorData>(),
                PartData = new List<PartData>(),
                DeptDetailTable = new DataTable(),
                ErrorDetailTable = new DataTable(),
                PartDetailTable = new DataTable(),
                ExportTopError = new DataTable(),
            };
            //await LoadNGModelReportDataAsync(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TopErrorReport(IFormCollection form)
        {
            var model = new ReportVM
            {
                fromDate = DateTime.TryParse(form["fromDate"], out var from) ? from : (DateTime?)null,
                toDate = DateTime.TryParse(form["toDate"], out var to) ? to : (DateTime?)null
            };

            await LoadTopErrorReportDataAsync(model);
            return View(model);
        }

        private async Task LoadTopErrorReportDataAsync(ReportVM model)
        {
            var startDate = model.fromDate;
            var endDate = model.toDate;

            var dateParams = new DynamicParameters();
            dateParams.Add("@StartDate", startDate);
            dateParams.Add("@EndDate", endDate);

            var partDatas = new List<PartData>();
            var deptDatas = new List<DeptData>();
            var errorDatas = new List<ErrorData>();

            model.DeptDetailTable = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_TopDeptDefected_new", dateParams);
            model.ErrorDetailTable = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_TopErrorTypeDefected_new", dateParams);
            model.PartDetailTable = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_TopPartDefected_new", dateParams);
            model.ExportTopError = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_TopDeptDefected_Export", dateParams);

            if (model.DeptDetailTable.Rows.Count > 0)
            {
                for (int i = 0; i < Math.Min(10, model.DeptDetailTable.Rows.Count); i++)
                {
                    var modelData = new DeptData
                    {
                        DeptError = model.DeptDetailTable.Rows[i]["DeptError"].ToString(),
                        InputQty = int.Parse(model.DeptDetailTable.Rows[i]["InputQty"].ToString()),
                        NGQty = int.Parse(model.DeptDetailTable.Rows[i]["NGQty"].ToString()),
                        NGRate = decimal.Parse(model.DeptDetailTable.Rows[i]["NGRate"].ToString()),
                    };
                    deptDatas.Add(modelData);
                }
                if (deptDatas.Count > 0)
                {
                    model.DeptData = deptDatas;
                }
            }
            if (model.ErrorDetailTable.Rows.Count > 0)
            {
                for (int i = 0; i < Math.Min(10, model.ErrorDetailTable.Rows.Count); i++)
                {
                    var modelData = new ErrorData
                    {
                        Errortype = model.ErrorDetailTable.Rows[i]["Errortype"].ToString(),
                        InputQty = int.Parse(model.ErrorDetailTable.Rows[i]["InputQty"].ToString()),
                        NGQty = int.Parse(model.ErrorDetailTable.Rows[i]["NGQty"].ToString()),
                        NGRate = decimal.Parse(model.ErrorDetailTable.Rows[i]["NGRate"].ToString()),
                    };

                    errorDatas.Add(modelData);
                }
                if (errorDatas.Count > 0)
                {
                    model.ErrorData = errorDatas;
                }
            }
            if (model.PartDetailTable.Rows.Count > 0)
            {
                for (int i = 0; i < Math.Min(10, model.PartDetailTable.Rows.Count); i++)
                {
                    var modelData = new PartData
                    {
                        Partcode = model.PartDetailTable.Rows[i]["Partcode"].ToString(),
                        InputQty = int.Parse(model.PartDetailTable.Rows[i]["InputQty"].ToString()),
                        NGQty = int.Parse(model.PartDetailTable.Rows[i]["NGQty"].ToString()),
                        NGRate = decimal.Parse(model.PartDetailTable.Rows[i]["NGRate"].ToString()),
                    };
                    partDatas.Add(modelData);
                }
                if (partDatas.Count > 0)
                {
                    model.PartData = partDatas;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportTopErrorReport(DateTime? fromDate, DateTime? toDate)
        {
            var dateParams = new[]
            {
                new SqlParameter("@StartDate", fromDate ?? (object)DBNull.Value),
                new SqlParameter("@EndDate", toDate ?? (object)DBNull.Value)
            };
            DataTable exportData = await _sqlHelperService.ExecuteStoredProcedureToDataTableAsync("sp_web_Repair_Uniden_TopDeptDefected_Export", dateParams);
            var excelBytes = _excelExportSvr.DatatableExportToExcel(exportData, "Error Detail");
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ErrorDetail.xlsx");
        }
    }
}