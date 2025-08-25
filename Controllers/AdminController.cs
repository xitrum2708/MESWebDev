using MESWebDev.Common;
using MESWebDev.Models;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace MESWebDev.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUVAssyProductionRepository _repository;
        private readonly ILoggingService _loggingService;
        private readonly IExcelExportService _excelExportService;
        private readonly IDashboard _dashboardService;

        public AdminController(IUVAssyProductionRepository repository, ILoggingService loggingService, IExcelExportService excelExportService, IDashboard dashboardService  )
        {
            _repository = repository;
            _loggingService = loggingService;
            _excelExportService = excelExportService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Dashboard(DateTime? date)
        {
            return View();
        }


        //-------------------->> IQC DASHBOARD <<--------------------
        public async Task<IActionResult> IQCDashboard()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@start_dt", DateTime.Now.AddMonths(-1).Date },
                { "@end_dt", DateTime.Now.Date }
            };
            DashboardViewModel model = await GetIQGDashboard(parameters);
            model.StartDate = DateTime.Now.AddMonths(-1).Date;
            model.EndDate = DateTime.Now.Date;
            return View("IQCDashboard/IQCDashboard", model);
            //return View("SMTDashboard/SMTDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> IQCDashboardSearch(DashboardViewModel model)
        {
            DateTime start_dt = model.StartDate ?? DateTime.Now.AddMonths(-1).Date;
            DateTime end_dt = model.EndDate ?? DateTime.Now.Date;
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@start_dt", start_dt },
                { "@end_dt", end_dt }
            };
            model = await GetIQGDashboard(parameters);
            model.StartDate = start_dt;
            model.EndDate = end_dt;
            return View("IQCDashboard/IQCDashboard", model);
        }
        public async Task<DashboardViewModel> GetIQGDashboard(Dictionary<string, object> parameters)
        {
            var model = new DashboardViewModel();
            DataSet ds = await _dashboardService.GetIQCDashboard(parameters);
            if(ds!= null&& ds.Tables.Count > 0)
            {
                model.sum_data = ds.Tables[0];
                model.detail_data = ds.Tables[1];

                DataTable chartData = ds.Tables[2];
                model.chart_data = chartData.AsEnumerable()
                    .Select(row => new ChartItem
                    {
                        Label = row.Field<string>("Description"),
                        Value = row.Field<int>("TotalErrors")
                    }).ToList();

            }            
            return model;
        }

        //-------------------->> SMT DASHBOARD <<--------------------
        public async Task<IActionResult> SMTDashboard()
        {
            DashboardViewModel model = await GetSMTDashboard();
            return View("SMTDashboard/SMTDashboard",model);
        }
        public async Task<DashboardViewModel> GetSMTDashboard()
        {
            var model = new DashboardViewModel();
            DataSet ds = await _dashboardService.GetSMTDashboard(new());
            if (ds != null && ds.Tables.Count > 4)
            {
                model.sum_data = ds.Tables[0];
                model.detail_data = ds.Tables[1];
                model.detail_data2 = ds.Tables[2]; // Assuming this is for SMT Dashboard
                DataTable line_chart = ds.Tables[3];
                DataTable pie_chart = ds.Tables[4];
                model.line_chart = line_chart.AsEnumerable()
                    .Select(row => new ChartItem2
                    {
                        Label = row.Field<string>(0),
                        Value = row.Field<decimal>(1)
                    }).ToList();
                model.chart_data2 = pie_chart.AsEnumerable()
                    .Select(row => new ChartItem
                    {
                        Label = row.Field<string>(1),
                        Value = row.Field<int>(2)
                    }).ToList();
            }
            return model;
        }

        //-------------------->> WHS DASHBOARD <<--------------------
        public async Task<IActionResult> WHSDashboard()
        {
            DashboardViewModel model = await GetWHSDashboard();
            return View("WHSDashboard/WHSDashboard", model);
        }
        public async Task<DashboardViewModel> GetWHSDashboard()
        {
            var model = new DashboardViewModel();
            DataSet ds = await _dashboardService.GetSMTDashboard(new());
            if (ds != null && ds.Tables.Count > 4)
            {
                model.sum_data = ds.Tables[0];
                model.sum_data2 = ds.Tables[1];
                model.detail_data = ds.Tables[2]; 
            }
            return model;
        }


        //-------------------->> SMT PRODUCTION INFO <<--------------------
        [HttpGet]
        public async Task<IActionResult> SMTLines()
        {
            DashboardViewModel model = new();
            model.detail_data = await _dashboardService.GetSMTLines();
            return View("SMTDashboard/SMTLines", model);
        }
        public async Task<IActionResult> SMTProdInfo(string line, string lot = "")
        {
            DashboardViewModel model = new();

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Line", line },
                { "@LotSMT", lot }
            };

            DataSet ds = await _dashboardService.GetSMTProdInfo(parameters);
            if (ds != null && ds.Tables.Count > 3 && ds.Tables[0].Rows.Count >0)
            {
                //table 0: Summary data
                model.line = ds.Tables[0].Rows[0][0].ToString();
                model.model = ds.Tables[0].Rows[0][1].ToString();
                model.lot = ds.Tables[0].Rows[0][2].ToString();
                model.lot_size = Convert.ToInt32(ds.Tables[0].Rows[0][3]);
                model.balance = Convert.ToInt32(ds.Tables[0].Rows[0][4]);
                model.target1H = Convert.ToDecimal(ds.Tables[0].Rows[0][5]);
                model.losttime = Convert.ToDecimal(ds.Tables[0].Rows[0][6]);
                //table 1: Lot list
                List<SelectListItem> lotList = ds.Tables[1].AsEnumerable()
                    .Select(row => new SelectListItem
                    {
                        Value = row.Field<string>("LotSMT"),
                        Text = row.Field<string>("LotSMT")
                    }).ToList();
                model.lotList = lotList;

                //table 2: Detail data
                model.detail_data = ds.Tables[2];

                //table 3: Detail data2
                model.detail_data2 = ds.Tables[3];

                model.bar_line_chart = ds.Tables[2].AsEnumerable()
                 .Select(row => new ChartItem3
                 {
                     Label = row.Field<string>(0) ?? "",
                     Value1 = row.Field<int?>(4) ?? 0,
                     Value2 = row.Field<int?>(5) ?? 0,
                     Rate = (row.Field<int?>(4) ?? 0) == 0
                         ? 0
                         : Math.Round((row.Field<int>(5) * 100.0) / row.Field<int>(4), 2)
                 }).ToList();


                //Chart data for pie chart
                model.chart_data = ds.Tables[3].AsEnumerable()
                    .Select(row => new ChartItem
                    {
                        Label = row.Field<string>(0),
                        Value = row.Field<int>(1)
                    }).ToList();                
            }
            return View("SMTDashboard/SMTProdInfo", model);
        }

        
       
    }
}