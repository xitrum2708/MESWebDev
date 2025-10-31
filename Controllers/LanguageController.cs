using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MESWebDev.Controllers
{
    public class LanguageController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _langService;

        public LanguageController(AppDbContext context, ITranslationService translationService, ILanguageService langService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
            _langService = langService;
        }

        // GET: LanguageController
        public async Task<ActionResult> Index()
        {
            var data = await _langService.GetAllLanguagesAsync();
            return View(data);
        }

        // GET: LanguageController/Create
        public ActionResult Create()
        {
            var model = new LanguageModel();
            return View(model);
        }

        // POST: LanguageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LanguageModel model)
        {
            string msg = await _langService.CreateLanguageAsync(model);
            if (string.IsNullOrEmpty(msg)) {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, msg);
            return View(model);
        }

        // GET: LanguageController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _langService.GetLanguageById(id);
            return View(model);
        }

        // POST: LanguageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LanguageModel model)
        {
            string msg = await _langService.UpdateLanguageAsync(model);
            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, msg);
            return View(model);
        }

        // POST: LanguageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            bool isDeleted = await _langService.DeleteLanguageAsync(id);
            if (isDeleted) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, "Cannot delete the language. It may be in use.");
            var model = await _langService.GetLanguageById(id);
            return View(model);
        }
    }
}