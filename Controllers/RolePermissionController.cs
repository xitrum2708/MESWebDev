using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class RolePermissionController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public RolePermissionController(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
        }

        // GET: RolePermission/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";

            var rolePermissionsQuery = _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Select(rp => new RolePermissionViewModel
                {
                    RoleId = rp.RoleId,
                    RoleName = rp.Role.RoleName,
                    PermissionId = rp.PermissionId,
                    PermissionKey = rp.Permission.PermissionKey,
                    GrantedAt = rp.GrantedAt
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                rolePermissionsQuery = rolePermissionsQuery.Where(rp => rp.RoleName.Contains(searchTerm) || rp.PermissionKey.Contains(searchTerm));
            }

            var pagedRolePermissions = rolePermissionsQuery.ToPagedResult(page, pageSize, searchTerm);

            return View(pagedRolePermissions);
        }

        // GET: RolePermission/Create
        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Roles = await _context.Roles
                .Select(r => new { r.RoleId, r.RoleName })
                .ToListAsync();

            ViewBag.Permissions = await _context.Permissions
                .Select(p => new { p.PermissionId, p.PermissionKey })
                .ToListAsync();

            return View();
        }

        // POST: RolePermission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolePermissionViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            ModelState.Remove("PermissionKey");
            ModelState.Remove("RoleName");
            if (ModelState.IsValid)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = model.RoleId,
                    PermissionId = model.PermissionId,
                    GrantedAt = DateTime.Now
                };

                _context.RolePermissions.Add(rolePermission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = await _context.Roles
                .Select(r => new { r.RoleId, r.RoleName })
                .ToListAsync();

            ViewBag.Permissions = await _context.Permissions
                .Select(p => new { p.PermissionId, p.PermissionKey })
                .ToListAsync();

            return View(model);
        }

        // GET: RolePermission/Delete
        public async Task<IActionResult> Delete(int roleId, int permissionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var rolePermission = await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission == null)
            {
                return NotFound();
            }

            var model = new RolePermissionViewModel
            {
                RoleId = rolePermission.RoleId,
                RoleName = rolePermission.Role.RoleName,
                PermissionId = rolePermission.PermissionId,
                PermissionKey = rolePermission.Permission.PermissionKey,
                GrantedAt = rolePermission.GrantedAt
            };

            return View(model);
        }

        // POST: RolePermission/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int roleId, int permissionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var rolePermission = await _context.RolePermissions.FindAsync(roleId, permissionId);
            if (rolePermission != null)
            {
                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}