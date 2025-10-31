using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;

namespace MESWebDev.Controllers
{
    public class LanguageController2 : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public LanguageController2(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
        }

        // GET: LanguageController
        public ActionResult Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var query = _context.Languages
                .Select(t => new LanguageViewModel
                {
                    LanguageId = t.LanguageId,
                    Name = t.Name,
                    Code = t.Code,
                    IsActive = t.IsActive,
                });
            //.AsQueryable();
            var searchLanguage = new LanguageViewModel();
            query = searchLanguage.ApplySearch(query, searchTerm);
            var language = query.ToPagedResult(page, pageSize, searchTerm);
            return View(language);
        }

        // GET: LanguageController/Create
        public ActionResult Create()
        {
            var model = new LanguageViewModel();
            return View(model);
        }

        // POST: LanguageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LanguageViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var language = new Language
                    {
                        Name = model.Name,
                        Code = model.Code,
                        IsActive = model.IsActive,
                    };
                    _context.Languages.Add(language);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // GET: LanguageController/Edit/5
        public ActionResult Edit(int id)
        {
            var language = _context.Languages.Find(id);
            if (language == null)
            {
                return NotFound();
            }
            var model = new LanguageViewModel
            {
                Name = language.Name,
                Code = language.Code,
                IsActive = language.IsActive,
            };
            return View(model);
        }

        // POST: LanguageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LanguageViewModel model)
        {
            try
            {
                if (id != model.LanguageId)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    var language = _context.Languages.Find(id);
                    if (language == null)
                    {
                        return NotFound();
                    }
                    language.Code = model.Code;
                    language.IsActive = model.IsActive;
                    language.Name = model.Name;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // POST: LanguageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var langugage = _context.Languages.Find(id);
                if (langugage == null)
                {
                    return NotFound();
                }
                _context.Languages.Remove(langugage);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}