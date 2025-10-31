using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MESWebDev.Controllers
{
    public class TranslationController2 : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        //private const int PageSize = 10;

        public TranslationController2(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
        }

        // GET: Translation/Index
        public IActionResult Index(int page = 1, string searchTerm = null, int pageSize = 10)
        {
            // Lấy dữ liệu và chuyển đổi thành TranslationViewModel trước khi phân trang
            var query = _context.Translations
                .Select(t => new TranslationViewModel
                {
                    TranslationId = t.TranslationId,
                    Keyvalue = t.Keyvalue,
                    LanguageId = t.LanguageId,
                    LanguageCode = _context.Languages
                        .Where(l => l.LanguageId == t.LanguageId)
                        .Select(l => l.Code)
                        .FirstOrDefault(),
                    Value = t.Value
                })
                .AsQueryable();

            // Áp dụng tìm kiếm nếu có
            var searchModel = new TranslationViewModel();
            query = searchModel.ApplySearch(query, searchTerm);

            // Áp dụng phân trang
            var result = query.ToPagedResult(page, pageSize, searchTerm);

            return View(result);
        }

        // GET: Translation/Create
        public IActionResult Create()
        {
            var model = new TranslationViewModel
            {
                AvailableLanguages = _context.Languages
                    .Select(l => new SelectListItem
                    {
                        Value = l.LanguageId.ToString(),
                        Text = l.Code
                    })
                    .ToList()
            };

            return View(model);
        }

        // POST: Translation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TranslationViewModel model)
        {
            ModelState.Remove("LanguageCode");
            ModelState.Remove("AvailableLanguages");
            if (ModelState.IsValid)
            {
                var translation = new Translation
                {
                    Keyvalue = model.Keyvalue,
                    LanguageId = model.LanguageId,
                    Value = model.Value
                };

                _context.Translations.Add(translation);
                _context.SaveChanges();

                // Xóa cache cho ngôn ngữ tương ứng
                var languageCode = _context.Languages
                    .Where(l => l.LanguageId == model.LanguageId)
                    .Select(l => l.Code)
                    .FirstOrDefault();
                _translationService.ClearCache(languageCode);

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, tải lại danh sách ngôn ngữ
            model.AvailableLanguages = _context.Languages
                .Select(l => new SelectListItem
                {
                    Value = l.LanguageId.ToString(),
                    Text = l.Code
                })
                .ToList();

            return View(model);
        }

        // GET: Translation/Edit/5
        public IActionResult Edit(int id)
        {
            var translation = _context.Translations.Find(id);
            if (translation == null)
            {
                return NotFound();
            }

            var model = new TranslationViewModel
            {
                TranslationId = translation.TranslationId,
                Keyvalue = translation.Keyvalue,
                LanguageId = translation.LanguageId,
                LanguageCode = _context.Languages
                    .Where(l => l.LanguageId == translation.LanguageId)
                    .Select(l => l.Code)
                    .FirstOrDefault(),
                Value = translation.Value,
                AvailableLanguages = _context.Languages
                    .Select(l => new SelectListItem
                    {
                        Value = l.LanguageId.ToString(),
                        Text = l.Code,
                        Selected = l.LanguageId == translation.LanguageId
                    })
                    .ToList()
            };

            return View(model);
        }

        // POST: Translation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TranslationViewModel model)
        {
            if (id != model.TranslationId)
            {
                return NotFound();
            }
            // Bỏ qua validation cho LanguageCode và AvailableLanguages
            ModelState.Remove("LanguageCode");
            ModelState.Remove("AvailableLanguages");
            if (ModelState.IsValid)
            {
                var translation = _context.Translations.Find(id);
                if (translation == null)
                {
                    return NotFound();
                }

                // Lưu ngôn ngữ cũ để xóa cache
                var oldLanguageId = translation.LanguageId;

                translation.Keyvalue = model.Keyvalue;
                translation.LanguageId = model.LanguageId;
                translation.Value = model.Value;

                _context.SaveChanges();

                // Xóa cache cho ngôn ngữ cũ và mới (nếu thay đổi ngôn ngữ)
                var oldLanguageCode = _context.Languages
                    .Where(l => l.LanguageId == oldLanguageId)
                    .Select(l => l.Code)
                    .FirstOrDefault();
                _translationService.ClearCache(oldLanguageCode);

                if (oldLanguageId != model.LanguageId)
                {
                    var newLanguageCode = _context.Languages
                        .Where(l => l.LanguageId == model.LanguageId)
                        .Select(l => l.Code)
                        .FirstOrDefault();
                    _translationService.ClearCache(newLanguageCode);
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, tải lại danh sách ngôn ngữ
            model.AvailableLanguages = _context.Languages
                .Select(l => new SelectListItem
                {
                    Value = l.LanguageId.ToString(),
                    Text = l.Code,
                    Selected = l.LanguageId == model.LanguageId
                })
                .ToList();

            return View(model);
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

            // Xóa cache cho ngôn ngữ tương ứng
            var languageCode = _context.Languages
                .Where(l => l.LanguageId == languageId)
                .Select(l => l.Code)
                .FirstOrDefault();
            _translationService.ClearCache(languageCode);

            return RedirectToAction(nameof(Index));
        }
    }
}