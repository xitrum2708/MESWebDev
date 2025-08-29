using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Models.PE;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using static MESWebDev.Common.Export2Excel;

namespace MESWebDev.Controllers
{
    public class PEController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IPEService _peService;
        private readonly ILoggingService _loggingService;
        private readonly Export2Excel _ee;

        public PEController(AppDbContext context, ITranslationService translationService, IPEService peService,
            ILoggingService loggingService)
            : base(context)
        {
            _translationService = translationService;
            _peService = peService;
            _loggingService = loggingService;
            _ee = new();
        }

        // GET: Permission/Index
        [HttpGet]
        public async Task<IActionResult> Manpower()
        {
            DataTable dt = await _peService.GetManpower(new());
            PEViewModel pev = new();
            pev.data = dt;
            return View("Manpower/Index", pev);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file1)
        {
            PEViewModel pev =new();
            if (file1 == null)
            {
                pev.error_msg = "Please upload no file.";
                return View("Manpower/Index", pev);
            }
            try
            {                
                pev = await _peService.UploadManpower(file1);
                ViewBag.Success = "Files uploaded and processed successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred while processing the files: {ex.Message}";
            }
            return View("Manpower/Index", pev);
        }



        [HttpPost]
        public async Task<IActionResult> DownloadData()
        {
            //Process and save updated events
            DataSet ds = new();
            //ds = await _repository.ExportProdPlan(new());
            return _ee.DownloadProdPlan(ds, $"ProdPlan");
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] TableFilterRequest request)
        {

            var fileBytes = await _ee.AjaxExcelExport(request, "#,##0.0000");    
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredData.xlsx");            
        }


        // Delete
        [HttpPost]
        public async Task<IActionResult> DeleteManpower([FromBody] List<int> ids)
        {
            string msg = string.Empty;
            msg = await _peService.DeleteManpower(ids);
            if (!string.IsNullOrEmpty(msg))
            {
                return Json(new { success = false, error = msg });
            }
            return Json(new { success = true });
        }
    }

}
