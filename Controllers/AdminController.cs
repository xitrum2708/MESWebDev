using MESWebDev.Common;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;

namespace MESWebDev.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUVAssyProductionRepository _repository;
        private readonly ILoggingService _loggingService;
        private readonly IExcelExportService _excelExportService;

        public AdminController(IUVAssyProductionRepository repository, ILoggingService loggingService, IExcelExportService excelExportService)
        {
            _repository = repository;
            _loggingService = loggingService;
            _excelExportService = excelExportService;
        }

        public async Task<IActionResult> Dashboard(DateTime? date)
        {
            return View();
            ////date ??= new DateTime(2025, 4, 5);
            //date ??= DateTime.Now;

            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (!userId.HasValue)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            //var results = new List<UVAssyAllOutputResult>();
            //results = await _repository.GetAllOutputResultsAsync(date.Value);

            //// Tính tổng ProdQty theo ProdSec cho biểu đồ
            //var chartData = results
            //    .GroupBy(r => r.ProdSec)
            //    .Select(g => new { ProdSec = g.Key, TotalProdQty = g.Sum(r => r.ProdQty) })
            //    .ToList();

            //ViewBag.ChartData = chartData; // Truyền dữ liệu biểu đồ qua ViewBag
            //return View(results); // Truyền dữ liệu bảng qua model
        }
    }
}