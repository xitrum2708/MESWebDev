using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class MenuTranslationController : BaseController
    {
        private readonly ITranslationService _translationService;

        public MenuTranslationController(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            _translationService = translationService;
        }

        // GET: MenuTranslation/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";

            var menuTranslationsQuery = _context.MenuTranslations
                .Include(mt => mt.Menu)
                .Include(mt => mt.Language)
                .Select(mt => new MenuTranslationViewModel
                {
                    MenuId = mt.MenuId,
                    MenuTitle = mt.Menu.Url, // You can adjust this to show a more meaningful field
                    LanguageId = mt.LanguageId,
                    LanguageName = mt.Language.Name,
                    Title = mt.Title,
                    Description = mt.Description
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                menuTranslationsQuery = menuTranslationsQuery.Where(mt => mt.MenuTitle.Contains(searchTerm) || mt.Title.Contains(searchTerm) || mt.Description.Contains(searchTerm));
            }

            var pagedMenuTranslations = menuTranslationsQuery.ToPagedResult(page, pageSize, searchTerm);

            return View(pagedMenuTranslations);
        }

        // GET: MenuTranslation/Create
        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Menus = await _context.Menus
                .Select(m => new { m.MenuId, DisplayValue = m.Url + "  --> " + m.PermissionKey })
                .ToListAsync();

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View();
        }

        // POST: MenuTranslation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuTranslationViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            ModelState.Remove("MenuTitle");
            ModelState.Remove("LanguageName");
            if (ModelState.IsValid)
            {
                var menuTranslation = new MenuTranslation
                {
                    MenuId = model.MenuId,
                    LanguageId = model.LanguageId,
                    Title = model.Title,
                    Description = model.Description
                };

                _context.MenuTranslations.Add(menuTranslation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Menus = await _context.Menus
                .Select(m => new { m.MenuId, m.Url })
                .ToListAsync();

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View(model);
        }

        // GET: MenuTranslation/Edit
        public async Task<IActionResult> Edit(int menuId, int languageId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var menuTranslation = await _context.MenuTranslations
                .Include(mt => mt.Menu)
                .Include(mt => mt.Language)
                .FirstOrDefaultAsync(mt => mt.MenuId == menuId && mt.LanguageId == languageId);

            if (menuTranslation == null)
            {
                return NotFound();
            }

            var model = new MenuTranslationViewModel
            {
                MenuId = menuTranslation.MenuId,
                MenuTitle = menuTranslation.Menu.Url,
                LanguageId = menuTranslation.LanguageId,
                LanguageName = menuTranslation.Language.Name,
                Title = menuTranslation.Title,
                Description = menuTranslation.Description
            };

            ViewBag.Menus = await _context.Menus
                .Select(m => new { m.MenuId, m.Url })
                .ToListAsync();

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View(model);
        }

        // POST: MenuTranslation/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int menuId, int languageId, MenuTranslationViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (menuId != model.MenuId || languageId != model.LanguageId)
            {
                return NotFound();
            }
            ModelState.Remove("MenuTitle");
            ModelState.Remove("LanguageName");
            if (ModelState.IsValid)
            {
                try
                {
                    var menuTranslation = await _context.MenuTranslations.FindAsync(menuId, languageId);
                    if (menuTranslation == null)
                    {
                        return NotFound();
                    }

                    menuTranslation.Title = model.Title;
                    menuTranslation.Description = model.Description;

                    _context.Update(menuTranslation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuTranslationExists(menuId, languageId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Menus = await _context.Menus
                .Select(m => new { m.MenuId, m.Url })
                .ToListAsync();

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View(model);
        }

        // GET: MenuTranslation/Delete
        public async Task<IActionResult> Delete(int menuId, int languageId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var menuTranslation = await _context.MenuTranslations
                .Include(mt => mt.Menu)
                .Include(mt => mt.Language)
                .FirstOrDefaultAsync(mt => mt.MenuId == menuId && mt.LanguageId == languageId);

            if (menuTranslation == null)
            {
                return NotFound();
            }

            var model = new MenuTranslationViewModel
            {
                MenuId = menuTranslation.MenuId,
                MenuTitle = menuTranslation.Menu.Url,
                LanguageId = menuTranslation.LanguageId,
                LanguageName = menuTranslation.Language.Name,
                Title = menuTranslation.Title,
                Description = menuTranslation.Description
            };

            return View(model);
        }

        // POST: MenuTranslation/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int menuId, int languageId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var menuTranslation = await _context.MenuTranslations.FindAsync(menuId, languageId);
            if (menuTranslation != null)
            {
                _context.MenuTranslations.Remove(menuTranslation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MenuTranslationExists(int menuId, int languageId)
        {
            return _context.MenuTranslations.Any(e => e.MenuId == menuId && e.LanguageId == languageId);
        }
    }
}