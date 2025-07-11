using DocumentFormat.OpenXml.Wordprocessing;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Filters;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.WHS.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class MasterController : BaseController
    {
        private readonly IUV_LOTCONTROL_MASTER_Service _uvLotControlService;
        private readonly ITranslationService _translationService;
        
        public MasterController(AppDbContext context, IUV_LOTCONTROL_MASTER_Service uvLotControlService, ITranslationService translationService) : base(context)
        {
            _uvLotControlService = uvLotControlService;
            _translationService = translationService;
        }
        [AuthorizeLogin]
        [HttpGet]
        public async Task<IActionResult> LotControlMaster(DateTime? startDate, DateTime? endDate, string? SearchTerm, int page=1,int  pageSize=10)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            startDate ??= DateTime.Now.AddDays(-30);
            endDate ??= DateTime.Now;
            var reports = await _uvLotControlService.GetFilteredLotsAsync(startDate, endDate, SearchTerm, page, pageSize);
            var viewModel = new MasterViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = SearchTerm,
                LotControlMasterList = reports
            };

            ViewBag.FromDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchText = SearchTerm ?? "";

            if (reports == null || !reports.Items.Any())
            {
                ViewBag.Message = _translationService.GetTranslation("NotFoundData", languageCode);
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMasterLotControl(UV_LOTCONTROL_MASTER model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("LotControlMaster");

            var existing = await _context.UV_LOTCONTROL_MASTERs.FirstOrDefaultAsync(x => x.LotNo == model.LotNo);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(model);
            }
            else
            {
                _context.UV_LOTCONTROL_MASTERs.Add(model);
            }

            await _context.SaveChangesAsync();
            await _uvLotControlService.RegenerateSerialsAsync(model.LotNo);

            return RedirectToAction("LotControlMaster");
        }
        [HttpGet]
        public async Task<IActionResult> LoadLotControlOrSpo(string lotNo)
        {
            var model = await _uvLotControlService.LoadLotControlOrSpoAsync(lotNo);
            if (model == null)
                return NotFound();

            return Json(model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveLotControl([FromBody] LotControlViewModel model)
        {
            return await _uvLotControlService.SaveLotControlAsync(model);
        }
    }
}
