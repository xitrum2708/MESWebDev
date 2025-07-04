using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.IQC;
using MESWebDev.Models.IQC.VM;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Repositories;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;

namespace MESWebDev.Controllers
{
    public class ProdPlanController : BaseController
    {
        private readonly ITranslationService _translationService;
        private readonly IProdPlanService _repository;
        private readonly ILoggingService _loggingService;
        private readonly Export2Excel _ee;

        public ProdPlanController(AppDbContext context, ITranslationService translationService, IProdPlanService repository,
            ILoggingService loggingService)
            : base(context)
        {
            _translationService = translationService;
            _repository = repository;
            _loggingService = loggingService;
            _ee = new();
        }

        // GET: Permission/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ProdPlanViewModel ppv = await _repository.ViewProdPlan(new RequestDTO());
            return View(ppv);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file1, IFormFile file2)
        {
            ProdPlanViewModel ppv=new();
            if (file1 == null || file2 == null)
            {
                ViewBag.Error = "Please upload all three files.";
                return View("Index");
            }
            try
            {
                // Process the uploaded files
                RequestDTO request = new();
                List<IFormFile> files = new() { file1, file2};
                request.Files = files;
                ppv = await _repository.GetDataFromUploadFile(request);
                ViewBag.Success = "Files uploaded and processed successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred while processing the files: {ex.Message}";
            }
            ppv.start_sch_dt = DateTime.Now;
            return View("Index",ppv);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProdPlan([FromBody] ProdPlanViewModel ppv)
        {
            // Process and save updated events
            //ppv.start_sch_dt = DateTime.Now;
            string msg = string.Empty;
            ppv = await _repository.ReloadProdPlan(ppv);
            msg = await _repository.SaveProdPlan(ppv);
            return Json(new
            {
                events = ppv.events,
                holidays = ppv.holidays,
                start_sch_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                resources = ppv.resources,
                message = msg// if needed // Add more properties as needed
            });
            //return View("Index", ppv);
        }
        [HttpPost]
        public async Task<IActionResult> ReloadProdPlan([FromBody] ProdPlanViewModel ppv)
        {
            //Process and save updated events
            //ppv.start_sch_dt = DateTime.Now;
            ppv = await _repository.ReloadProdPlan(ppv);
            return Json(new
            {
                events = ppv.events,
                holidays = ppv.holidays,
                start_sch_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                resources = ppv.resources,
                message = "Reload data successfully !"// If needed // Add more properties as needed
            });
            //return View("Index", ppv);
        }

        //[HttpPost]
        public async Task<IActionResult> DownloadProdPlan()
        {
            //Process and save updated events
            DataSet ds = new();
            ds = await _repository.ExportProdPlan(new());
            return _ee.DownloadProdPlan(ds, $"ProdPlan");
            //return View("Index", ppv);
        }


        //[HttpGet("Holiday/Get")]
        [HttpGet]
        public async Task<IActionResult> GetHolidays(int year, int month)
        {
            var holidays = await _repository.GetHoliday(new RequestDTO() { year = year, month = month });
            return Ok(holidays);
        }

        //[HttpPost("Holiday/Save")]
        [HttpPost]
        public async Task<IActionResult> SaveHolidays([FromBody]List<string> request)
        {
            // Save to DB here
            
            return Ok(await _repository.SaveHoliday(request));
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePara(ProdPlanViewModel ppv)
        {
            string msg = await _repository.SavePara(ppv);
            return Content(msg);
        }
    }
}
