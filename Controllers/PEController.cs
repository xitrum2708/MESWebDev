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
using System.Threading.Tasks;

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
            //return View("Index", ppv);
        }


        [HttpPost]
        public IActionResult ExportToExcel([FromBody] TableFilterRequest request)
        {
            // request contains the filtered rows sent from client (AJAX)
            // Example: request.Rows is a List<string[]> of visible rows

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("FilteredData");

                // Header
                for (int c = 0; c < request.Headers.Count; c++)
                {
                    worksheet.Cells[1, c + 1].Value = request.Headers[c];
                }

                // Data
                for (int r = 0; r < request.Rows.Count; r++)
                {
                    for (int c = 0; c < request.Rows[r].Count; c++)
                    {
                        worksheet.Cells[r + 2, c + 1].Value = request.Rows[r][c];
                    }
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredData.xlsx");
            }
        }
    }
    public class TableFilterRequest
    {
        public List<string> Headers { get; set; }
        public List<List<string>> Rows { get; set; }
    }
}
