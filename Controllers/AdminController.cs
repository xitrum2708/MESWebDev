using MESWebDev.Common;
using MESWebDev.Models;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
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
            DashboardViewModel model = await GetIQGDashboard();
            return View("IQCDashboard/IQCDashboard", model);
            //return View("SMTDashboard/SMTDashboard");
        }
        public async Task<DashboardViewModel> GetIQGDashboard()
        {
            var model = new DashboardViewModel();
            DataSet ds = await _dashboardService.GetIQCDashboard(new());
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
    }
}