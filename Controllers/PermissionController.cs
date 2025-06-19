using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class PermissionController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public PermissionController(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
        }

        // GET: Permission/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";

            var permissionsQuery = _context.Permissions
                .Select(p => new PermissionViewModel
                {
                    PermissionId = p.PermissionId,
                    PermissionKey = p.PermissionKey,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                permissionsQuery = permissionsQuery.Where(p => p.PermissionKey.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            var pagedPermissions = permissionsQuery.ToPagedResult(page, pageSize, searchTerm);

            return View(pagedPermissions);
        }

        // GET: Permission/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckPermissionKey(string permissionKey)
        {
            var exists = await _context.Permissions
                .AnyAsync(p => p.PermissionKey == permissionKey);
            return Json(!exists); // Return true if the key does NOT exist (valid), false if it does (invalid)
        }

        // POST: Permission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PermissionViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var permission = new Permission
                {
                    PermissionKey = model.PermissionKey,
                    Description = model.Description,
                    CreatedAt = DateTime.Now
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Permission/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            var model = new PermissionViewModel
            {
                PermissionId = permission.PermissionId,
                PermissionKey = permission.PermissionKey,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt
            };

            return View(model);
        }

        // POST: Permission/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PermissionViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.PermissionId)
            {
                return NotFound();
            }
            //// Check if Description is empty and add a translated error message
            //if (string.IsNullOrEmpty(model.Description))
            //{
            //    var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            //    ModelState.AddModelError("Description", _translationService.GetTranslation("DescriptionRequired", languageCode));
            //}
            if (ModelState.IsValid)
            {
                try
                {
                    var permission = await _context.Permissions.FindAsync(id);
                    if (permission == null)
                    {
                        return NotFound();
                    }

                    //permission.PermissionKey = model.PermissionKey;
                    permission.Description = model.Description;

                    _context.Update(permission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Permission/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            var model = new PermissionViewModel
            {
                PermissionId = permission.PermissionId,
                PermissionKey = permission.PermissionKey,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt
            };

            return View(model);
        }

        // POST: Permission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.PermissionId == id);
        }
    }
}