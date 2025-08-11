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

        public async Task<IActionResult> IQCDashboard()
        {
            DashboardViewModel model = await GetIQGDashboard();
            return View("IQCDashboard/IQCDashboard", model);
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
    }
}