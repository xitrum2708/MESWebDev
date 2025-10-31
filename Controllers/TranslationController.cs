using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace MESWebDev.Controllers
{
    [Authorize]
    public class TranslationController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _langService;

        //private const int PageSize = 10;

        public TranslationController(AppDbContext context, ITranslationService translationService, ILanguageService langService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
            _langService = langService;
        }

        // GET: Translation/Index
        public async Task<IActionResult> Index()
        {
            var data = await _langService.GetDictionaryAsync();

            return View(data);
        }

        // GET: Translation/Create
        public async Task<IActionResult> Create()
        {
            MasterVM mvm = new();
            mvm.Dictionary = new();
            mvm.LanguageSL = await _langService.GetLangSL();

            return View(mvm);
        }

        // POST: Translation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterVM model)
        {

            string msg = await _langService.CreateDictionaryAsync(model.Dictionary);
            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, msg);
            model.LanguageSL = await _langService.GetLangSL();

            return View(model);
        }

        // GET: Translation/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            MasterVM mvm = new();
            mvm.Dictionary = await _langService.GetDictionaryById(id);
            mvm.LanguageSL = await _langService.GetLangSL();
            return View(mvm);
        }

        // POST: Translation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MasterVM mvm)
        {
            string msg = await _langService.UpdateDictionaryAsync(mvm.Dictionary);
            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, msg);
            mvm.LanguageSL = await _langService.GetLangSL();

            return View(mvm);
        }

        // POST: Translation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var translation = _context.Translations.Find(id);
            if (translation == null)
            {
                return NotFound();
            }

            // Lưu ngôn ngữ để xóa cache
            var languageId = translation.LanguageId;

            _context.Translations.Remove(translation);
            _context.SaveChanges();

            

            return RedirectToAction(nameof(Index));
        }
    }
}